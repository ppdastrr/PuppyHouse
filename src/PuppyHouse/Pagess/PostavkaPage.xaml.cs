using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KP_4_PuppyHouse1.Pagess
{
    public partial class PostavkaPage : UserControl
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private string _searchText;
        private Dictionary<int, bool> _notifiedProducts = new Dictionary<int, bool>();

        public PostavkaPage()
        {
            InitializeComponent();
            LoadPostavki();
        }

        private void LoadPostavki()
        {
            var postavki = bd.Postavkas.ToList();

            // Проверка наличия товаров на складе
            foreach (var postavka in postavki)
            {
                var availableQuantity = bd.Postavkas
                    .Where(p => p.ID_Tovar == postavka.ID_Tovar)
                    .Sum(p => p.Count) ?? 0;

                if (availableQuantity <= 0 && !_notifiedProducts.ContainsKey(postavka.ID_Tovar.Value))
                {
                    MessageBox.Show($"Товар '{postavka.Tovar.Name}' закончился на складе.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _notifiedProducts[postavka.ID_Tovar.Value] = true;
                }
            }

            // Фильтрация поставок, где товар есть в наличии
            postavki = postavki.Where(p => p.Count > 0).ToList();

            dataGrid.ItemsSource = postavki;
        }

        private void ApplyFilters()
        {
            var query = bd.Postavkas.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(p => p.Tovar.Name.Contains(_searchText));
            }

            dataGrid.ItemsSource = query.ToList();
        }

        private void AddPostavka_Btn_Click_1(object sender, RoutedEventArgs e)
        {
            var addPostavkaWindow = new AddPostavkaWindow();
            addPostavkaWindow.ShowDialog();
            LoadPostavki();
        }

        private void SearchBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
            ApplyFilters();
        }
    }
}