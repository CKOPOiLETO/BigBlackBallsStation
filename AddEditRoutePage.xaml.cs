using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*namespace BigBusStation
{
    /// <summary>
    /// Логика взаимодействия для AddEditRoutePage.xaml
    /// </summary>
    public partial class AddEditRoutePage : Page
    {
        private Routes _currentRoute = new Routes();
        private bool _editMode = false;

        public AddEditRoutePage(Routes selectedRoute)
        {
            InitializeComponent();

            if (selectedRoute != null)
            {
                _currentRoute = selectedRoute;
                _editMode = true;
            }

            DataContext = _currentRoute;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentRoute.DepartuePoint))
                errors.AppendLine("Укажите пункт отправления");
            if (string.IsNullOrWhiteSpace(_currentRoute.Destination))
                errors.AppendLine("Укажите пункт назначения");
            if (_currentRoute.DistanceKM <= 0)
                errors.AppendLine("Укажите корректное расстояние");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (!_editMode)
            {
                var routesList = BusTicketDBEntities1.GetContext().Routes.ToList();
                for (int i = 0; true; i++)
                {
                    if (routesList.All(r => r.Id != i))
                    {
                        _currentRoute.Id = i;
                        break;
                    }
                }
                BusTicketDBEntities1.GetContext().Routes.Add(_currentRoute);
            }

            try
            {
                BusTicketDBEntities1.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}*/
namespace BigBusStation
{
    public partial class AddEditRoutePage : Page
    {
        private RouteViewModel _viewModel;

        public AddEditRoutePage(Routes selectedRoute)
        {
            InitializeComponent();

            _viewModel = new RouteViewModel(selectedRoute);
            DataContext = _viewModel;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_viewModel.Route.DepartuePoint))
                errors.AppendLine("Укажите пункт отправления");
            if (string.IsNullOrWhiteSpace(_viewModel.Route.Destination))
                errors.AppendLine("Укажите пункт назначения");
            if (_viewModel.Route.DistanceKM <= 0)
                errors.AppendLine("Укажите корректное расстояние");
            if (_viewModel.SelectedBus == null)
                errors.AppendLine("Выберите автобус");
            if (!TimeSpan.TryParse(_viewModel.DepartureTime, out _))
                errors.AppendLine("Укажите корректное время отправления");
            if (!TimeSpan.TryParse(_viewModel.ArrivalTime, out _))
                errors.AppendLine("Укажите корректное время прибытия");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                // Сохраняем маршрут
                if (!_viewModel.IsEditMode)
                {
                    BusTicketDBEntities1.GetContext().Routes.Add(_viewModel.Route);
                }

                // Сохраняем расписание
                var schedule = _viewModel.Schedule;
                schedule.DepartureTime = TimeSpan.Parse(_viewModel.DepartureTime);
                schedule.ArrivalTime = TimeSpan.Parse(_viewModel.ArrivalTime);
                
                schedule.BusId = _viewModel.SelectedBus.Id;
                schedule.RouteID = _viewModel.Route.Id;

                if (!_viewModel.IsEditMode)
                {
                    BusTicketDBEntities1.GetContext().Schedules.Add(schedule);
                }

                BusTicketDBEntities1.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}

