using KP_4_PuppyHouse1.BD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : UserControl
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();

        public ReportsPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            // Загрузка данных для таблицы поставки
            PostavkaDataGrid.ItemsSource = bd.Postavkas.ToList();

            // Загрузка данных для таблицы продажи
            ProdazhaDataGrid.ItemsSource = bd.Prodazhas.ToList();
        }


        private void ProdazhaDataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (ProdazhaDataGrid.SelectedItem is Prodazha selectedProdazha)
            {
                var products = selectedProdazha.SpisokTovars.Select(t => new
                {
                    Название = t.Tovar.Name,
                    Количество = t.Count
                }).ToList();

                string productList = string.Join("\n", products.Select(p => $"{p.Название} - {p.Количество}"));
                MessageBox.Show($"Список товаров:\n{productList}", "Детали продажи", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchByNamePostavka_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchByNamePostavka.Text.ToLower();
            PostavkaDataGrid.ItemsSource = bd.Postavkas
                .Where(p => p.Tovar.Name.ToLower().Contains(searchQuery))
                .ToList();
        }

        private void SearchByNameKass_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchByNameKass.Text.ToLower();
            ProdazhaDataGrid.ItemsSource = bd.Prodazhas
                .Where(pr => pr.User.FIO.ToLower().Contains(searchQuery))
                .ToList();
        }
    }
}
