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

namespace KP_4_PuppyHouse1.Pagess
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : UserControl
    {
        private BD.BD_PuppyHouseEntities bd = new BD.BD_PuppyHouseEntities();
        public ClientsPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            // Загружаем данные для таблицы
            var clientsWithPets = bd.Dogs
                .Where(d => d.User.ID_Role == 4) // Фильтруем только клиентов
                .Select(d => new
                {
                    FullName = d.User.FIO,
                    Phone = d.User.Phone,
                    Email = d.User.Email,
                    PetName = d.Name,
                    Breed = d.Poroda,
                    Age = d.Age
                })
                .ToList();

            // Устанавливаем данные в DataGrid
            dataGrid.ItemsSource = clientsWithPets;
        }
        private void ApplyFilters()
        {
            // Получаем значения фильтров
            string searchText = SearchBox.Text.ToLower();
     

            // Фильтруем данные
            var filteredClients = bd.Dogs
                .Where(d => d.User.ID_Role == 4 &&
                            (string.IsNullOrEmpty(searchText) || d.User.FIO.ToLower().Contains(searchText)))
                .Select(d => new
                {
                    FullName = d.User.FIO,
                    Phone = d.User.Phone,
                    Email = d.User.Email,
                    PetName = d.Name,
                    Breed = d.Poroda,
                    Age = d.Age
                })
                .ToList();

            // Обновляем таблицу
            dataGrid.ItemsSource = filteredClients;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BreedFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}
