using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Win;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KP_4_PuppyHouse1.Pagess
{
    public partial class KassirTovarPage : UserControl
    {
        private BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public ObservableCollection<Tovar> FilteredProducts { get; set; }
        public ObservableCollection<SpisokTovar> SpisokTovar { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> Brands { get; set; }
        public ObservableCollection<string> Countries { get; set; }
        public ICommand AddProductCommand { get; private set; }
        private int _quantity = 1; // Инициализируем начальное количество
        private Tovar _tovar;
        private User _currentUser;
        public KassirTovarPage(User currentUser)
        {
            InitializeComponent();
            FilteredProducts = new ObservableCollection<Tovar>();
            SpisokTovar = new ObservableCollection<SpisokTovar>();
            Categories = new ObservableCollection<string>();
            Brands = new ObservableCollection<string>();
            Countries = new ObservableCollection<string>();
            DataContext = this;
            LoadData();
            _currentUser = currentUser;
        }

        private void LoadData()
        {

            var products = bd.Tovars.ToList();

            // Инициализация фильтров
            CategoryFilter.ItemsSource = products
                .Where(p => p.CategoryTovar != null)
                .Select(p => p.CategoryTovar.Name)
                .Distinct()
                .ToList();

            BrandFilter.ItemsSource = products
                .Where(p => p.Brand != null)
                .Select(p => p.Brand.Name)
                .Distinct()
                .ToList();

            CountryFilter.ItemsSource = products
                .Where(p => p.Country != null)
                .Select(p => p.Country.Name)
                .Distinct()
                .ToList();

            // Установка начального списка товаров
            FilteredProducts.Clear();
            foreach (var product in products)
            {
                var availableQuantity = bd.Postavkas
                    .Where(p => p.ID_Tovar == product.ID)
                    .Sum(p => p.Count) ?? 0;

                if (availableQuantity > 0)
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        private void ApplyFilters()
        {
            string searchText = SearchBox.Text.ToLower();
            string selectedCategory = CategoryFilter.SelectedItem?.ToString();
            string selectedBrand = BrandFilter.SelectedItem?.ToString();
            string selectedCountry = CountryFilter.SelectedItem?.ToString();

            var filtered = bd.Tovars.Where(p =>
                (string.IsNullOrEmpty(searchText) || p.Name.ToLower().Contains(searchText)) &&
                (string.IsNullOrEmpty(selectedCategory) || p.CategoryTovar.Name == selectedCategory) &&
                (string.IsNullOrEmpty(selectedBrand) || p.Brand.Name == selectedBrand) &&
                (string.IsNullOrEmpty(selectedCountry) || p.Country.Name == selectedCountry));

            FilteredProducts.Clear();
            foreach (var product in filtered)
            {
                var availableQuantity = bd.Postavkas
                    .Where(p => p.ID_Tovar == product.ID)
                    .Sum(p => p.Count) ?? 0;

                if (availableQuantity > 0)
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BrandFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CountryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Обработчики кнопок для изменения количества товара
        private void IncreaseQuantity(object sender, RoutedEventArgs e)
        {
            _tovar = (Tovar)dataGrid.SelectedItem;
            if (_tovar != null)
            {
                _quantity++;
                QuantityTextBox.Text = _quantity.ToString();
            }
        }

        private void DecreaseQuantity(object sender, RoutedEventArgs e)
        {
            _tovar = (Tovar)dataGrid.SelectedItem;
            if (_tovar != null)
            {
                if (_quantity > 1)
                {
                    _quantity--;
                    QuantityTextBox.Text = _quantity.ToString();
                }
            }
        }

        // Обработчик кнопки добавления товара в корзину
        private void AddToCart(object sender, RoutedEventArgs e)
        {
            _tovar = (Tovar)dataGrid.SelectedItem;
            if (_tovar != null)
            {
                var cartItem = SpisokTovar.FirstOrDefault(ci => ci.ID_Tovar == _tovar.ID);
                if (cartItem != null)
                {
                    cartItem.Count += _quantity;
                }
                else
                {
                    cartItem = new SpisokTovar
                    {
                        ID_Tovar = _tovar.ID,
                        Count = _quantity,
                    };
                    SpisokTovar.Add(cartItem);
                }

                bd.SpisokTovars.Add(cartItem);
                bd.SaveChanges();

                MessageBox.Show("Товар добавлен в корзину!","Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите товар для добавления в корзину.");
            }
        }

        private void CartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SpisokTovar.Count > 0)
            {
                var cartWindow = new CartWindow(_currentUser);
                cartWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Корзина пуста. Добавьте товары в корзину.");
            }
        }


    }
}
