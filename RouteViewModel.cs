using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBusStation
{
    public class RouteViewModel
    {
        public Routes Route { get; set; }
        public Schedules Schedule { get; set; }
        public System.Collections.Generic.List<Buses> Buses { get; set; }
        public Buses SelectedBus { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public bool IsEditMode { get; set; }

        public RouteViewModel(Routes selectedRoute)
        {
            var context = BusTicketDBEntities1.GetContext();
            Buses = context.Buses.ToList();

            if (selectedRoute != null)
            {
                Route = selectedRoute;
                Schedule = context.Schedules.FirstOrDefault(s => s.RouteID == Route.Id);
                SelectedBus = Schedule?.Buses;
                IsEditMode = true;

                if (Schedule != null)
                {
                    DepartureTime = Schedule.DepartureTime.ToString(@"hh\:mm");
                    ArrivalTime = Schedule.ArrivalTime.ToString(@"hh\:mm");
                }
            }
            else
            {
                Route = new Routes();
                Schedule = new Schedules();
                DepartureTime = "08:00";
                ArrivalTime = "12:00";
                IsEditMode = false;
            }
        }
    }
}
