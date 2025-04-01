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

namespace BigBusStation
{
    /// <summary>
    /// Логика взаимодействия для RoutesPage.xaml
    /// </summary>
    public partial class RoutesPage : Page
    {
        public RoutesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DGridRoutes.ItemsSource = BusTicketDBEntities1.GetContext().Routes
                .Include("Schedules.Buses")
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditRoutePage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoute = (sender as Button).DataContext as Routes;
            Manager.MainFrame.Navigate(new AddEditRoutePage(selectedRoute));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var routesForDelete = DGridRoutes.SelectedItems.Cast<Routes>().ToList();

            if (MessageBox.Show($"Удалить {routesForDelete.Count} маршрутов?", "Внимание",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //BusTicketDBEntities1.GetContext().Routes.RemoveRange(routesForDelete);
                    BusTicketDBEntities1.GetContext().SaveChanges();
                    MessageBox.Show("Маршруты удалены!");
                    DGridRoutes.ItemsSource = BusTicketDBEntities1.GetContext().Routes.ToList();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                BusTicketDBEntities1.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridRoutes.ItemsSource = BusTicketDBEntities1.GetContext().Routes.ToList();
            }
        }
    }
}
