using KP_4_PuppyHouse1.BD;
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
using System.Windows.Shapes;

namespace KP_4_PuppyHouse1.Win
{
    /// <summary>
    /// Логика взаимодействия для MainWin.xaml
    /// </summary>
    public partial class MainWin : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public User CurrentUser { get; set; }
        public MainWin(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            UserName = user.FIO;
            UserRole = GetRoleName((int)user.ID_Role);
            SetVisibility((int)user.ID_Role);
            this.DataContext = this;
        }
        private void SetVisibility(int roleId)
        {
            switch (roleId)
            {
                case 1: // Администратор
                    СотрудникиК.Visibility = Visibility.Visible;
                    КлиентыК.Visibility = Visibility.Visible;
                    ТоварыК.Visibility = Visibility.Visible;
                    УслугиК.Visibility = Visibility.Visible;
                    ПоставкаК.Visibility = Visibility.Visible;
                    ПродажаК.Visibility = Visibility.Visible;
                    РасписаниеК.Visibility = Visibility.Visible;
                    ОтчетыК.Visibility = Visibility.Visible;
                    break;
                case 2: // Кассир-консультант
                    СотрудникиК.Visibility = Visibility.Collapsed;
                    КлиентыК.Visibility = Visibility.Collapsed;
                    ТоварыК.Visibility = Visibility.Collapsed;
                    УслугиК.Visibility = Visibility.Collapsed;
                    ПоставкаК.Visibility = Visibility.Collapsed;
                    ПродажаК.Visibility = Visibility.Visible;
                    РасписаниеК.Visibility = Visibility.Visible;
                    ОтчетыК.Visibility= Visibility.Collapsed;
                    break;
                case 3: // Мастер
                    СотрудникиК.Visibility = Visibility.Collapsed;
                    КлиентыК.Visibility = Visibility.Collapsed;
                    ТоварыК.Visibility = Visibility.Collapsed;
                    УслугиК.Visibility = Visibility.Collapsed;
                    ПоставкаК.Visibility = Visibility.Collapsed;
                    ПродажаК.Visibility = Visibility.Collapsed;
                    РасписаниеК.Visibility = Visibility.Visible;
                    ОтчетыК.Visibility = Visibility.Collapsed;
                    break;
                default:
                    СотрудникиК.Visibility = Visibility.Collapsed;
                    КлиентыК.Visibility = Visibility.Collapsed;
                    ТоварыК.Visibility = Visibility.Collapsed;
                    УслугиК.Visibility = Visibility.Collapsed;
                    ПоставкаК.Visibility = Visibility.Collapsed;
                    ПродажаК.Visibility = Visibility.Collapsed;
                    РасписаниеК.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private string GetRoleName(int roleId)
        {
            switch (roleId)
            {
                case 1:
                    return "Администратор";
                case 2:
                    return "Кассир-консультант";
                case 3:
                    return "Мастер";
                default:
                    return "Неизвестная роль";
            }
        }
        private void ShowEmployeesView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.EmployeesPage();
        }

        private void ShowClientsView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.ClientsPage();
        }

        private void ShowProductsView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.ProductsPage();
        }

        private void ShowServicesView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.ServicesPage();
        }

        private void ShowReportsView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.PostavkaPage();
        }

        private void ShowProdazhaView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.KassirTovarPage(CurrentUser);
        }

        private void ShowSheduleView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.SchedulePage(CurrentUser);
        }

        private void ShowOtchetView(object sender, RoutedEventArgs e)
        {
            ContentRegion.Content = new Pagess.ReportsPage();
        }
    }
}
