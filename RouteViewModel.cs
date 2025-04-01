using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BigBusStation
{
    public class RouteViewModel : INotifyPropertyChanged
    {
        private readonly BusTicketDBEntities1 _context;
        private string _departureTime;
        private string _arrivalTime;
        private Buses _selectedBus;
        private DateTime _selectedDate = DateTime.Today;

        public Routes Route { get; private set; }
        public Schedules Schedule { get; private set; }
        public List<Buses> AvailableBuses { get; private set; }
        

        public bool IsEditMode { get; private set; }




        public Buses SelectedBus
        {
            get => _selectedBus;
            set
            {
                _selectedBus = value;
                OnPropertyChanged();
                if (Schedule != null && value != null)
                {
                    Schedule.Buses = value;
                    Schedule.BusId = value.Id;
                }
            }
        }

        public string DepartureTime
        {
            get => _departureTime;
            set
            {
                if (TimeSpan.TryParse(value, out _))
                {
                    _departureTime = value;
                    OnPropertyChanged();
                    UpdateScheduleTimes();
                }
            }
        }

        public string ArrivalTime
        {
            get => _arrivalTime;
            set
            {
                if (TimeSpan.TryParse(value, out _))
                {
                    _arrivalTime = value;
                    OnPropertyChanged();
                    UpdateScheduleTimes();
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                if (Schedule != null)
                {
                    Schedule.DepartureData = value;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RouteViewModel(Routes route = null)
        {
            _context = BusTicketDBEntities1.GetContext();
            InitializeCollections();
            LoadRouteData(route);
        }

        private void InitializeCollections()
        {
            try
            {
                Debug.WriteLine("Загрузка списка автобусов...");

                // Вариант 1: С явным указанием загрузки
                AvailableBuses = _context.Buses
                    .Include(b => b.Schedules)
                    .AsNoTracking()
                    .ToList();

                // ИЛИ Вариант 2: С проверкой данных
                var buses = _context.Buses.ToList(); // Сначала без Include
                Debug.WriteLine($"Найдено автобусов в БД: {buses.Count}");

                AvailableBuses = buses;

                // Проверка связей
                if (AvailableBuses.Any())
                {
                    var testBus = AvailableBuses.First();
                    _context.Entry(testBus).Collection(b => b.Schedules).Load();
                    Debug.WriteLine($"Тестовый автобус {testBus.Number} имеет {testBus.Schedules?.Count ?? 0} рейсов");
                }

                Debug.WriteLine($"Успешно загружено автобусов: {AvailableBuses.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки автобусов: {ex.ToString()}");
                AvailableBuses = new List<Buses>();
            }
        }


        private void LoadRouteData(Routes route)
        {
            Debug.WriteLine("=== Начало загрузки данных ===");

            if (route != null)
            {
                Debug.WriteLine($"Режим редактирования маршрута ID: {route.Id.ToString()}");

                Route = route;
                IsEditMode = true;

                // Загрузка расписания
                Schedule = _context.Schedules
                    .Include("Buses")
                    .FirstOrDefault(s => s.RouteID == route.Id);

                Debug.WriteLine(Schedule != null ? "Загружено расписание: найдено" : "Загружено расписание: не найдено");

                if (Schedule == null)
                {
                    Schedule = new Schedules { RouteID = route.Id };
                    Debug.WriteLine("Создано новое расписание");
                }

                // Проверка автобусов
                Debug.WriteLine($"Доступно автобусов: {AvailableBuses.Count.ToString()}");
                SelectedBus = Schedule.Buses ?? AvailableBuses.FirstOrDefault();
                Debug.WriteLine(SelectedBus != null ? $"Выбран автобус: {SelectedBus.Number}" : "Автобус не выбран");

                // Установка времени
                DepartureTime = Schedule.DepartureTime.ToString(@"hh\:mm");
                ArrivalTime = Schedule.ArrivalTime.ToString(@"hh\:mm");
                SelectedDate = Schedule.DepartureData;

                Debug.WriteLine($"Время отправления: {DepartureTime}, прибытия: {ArrivalTime}");
            }
            else
            {
                Debug.WriteLine("Режим создания нового маршрута");

                Route = new Routes();
                Schedule = new Schedules();
                SelectedBus = AvailableBuses.FirstOrDefault();
                DepartureTime = "08:00";
                ArrivalTime = "12:00";
                SelectedDate = DateTime.Today;
                IsEditMode = false;

                Debug.WriteLine(SelectedBus != null ? $"Создан новый маршрут, автобус по умолчанию: {SelectedBus.Number}" : "Автобус не выбран");
            }

            Debug.WriteLine("=== Завершение загрузки данных ===");
        }

        private void UpdateScheduleTimes()
        {
            if (Schedule != null)
            {
                if (TimeSpan.TryParse(DepartureTime, out TimeSpan depTime))
                {
                    Schedule.DepartureTime = depTime;
                }

                if (TimeSpan.TryParse(ArrivalTime, out TimeSpan arrTime))
                {
                    Schedule.ArrivalTime = arrTime;
                }
            }
        }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Route.DepartuePoint))
                return false;

            if (string.IsNullOrWhiteSpace(Route.Destination))
                return false;

            if (Route.DistanceKM <= 0)
                return false;

            if (SelectedBus == null)
                return false;

            if (!TimeSpan.TryParse(DepartureTime, out _) || !TimeSpan.TryParse(ArrivalTime, out _))
                return false;

            return true;
        }

        public void SaveChanges()
        {
            if (!Validate())
                throw new InvalidOperationException("Данные не прошли валидацию");

            // Обновление связей
            Schedule.Routes = Route;
            Schedule.Buses = SelectedBus;

            if (!IsEditMode)
            {
                _context.Routes.Add(Route);
            }
            else
            {
                _context.Entry(Route).State = EntityState.Modified;
            }

            if (Schedule.Id == 0)
            {
                _context.Schedules.Add(Schedule);
            }
            else
            {
                _context.Entry(Schedule).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
