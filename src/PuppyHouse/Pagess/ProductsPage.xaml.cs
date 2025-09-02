using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Win;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KP_4_PuppyHouse1.Pagess
{
    public partial class ProductsPage : UserControl
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private string _searchText;
        private CategoryTovar _selectedCategory;
        private Brand _selectedBrand;
        private Country _selectedCountry;

        public ProductsPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadFilters();
        }

        private void LoadProducts()
        {
            var products = bd.Tovars.ToList();
            dataGridTovar.ItemsSource = products;
        }

        private void LoadFilters()
        {
            CategoryFilter.ItemsSource = bd.CategoryTovars.ToList();
            BrandFilter.ItemsSource = bd.Brands.ToList();
            CountryFilter.ItemsSource = bd.Countries.ToList();
        }

        private void ApplyFilters()
        {
            var query = bd.Tovars.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(p => p.Name.Contains(_searchText));
            }

            if (_selectedCategory != null)
            {
                query = query.Where(p => p.CategoryTovar.ID == _selectedCategory.ID);
            }

            if (_selectedBrand != null)
            {
                query = query.Where(p => p.Brand.ID == _selectedBrand.ID);
            }

            if (_selectedCountry != null)
            {
                query = query.Where(p => p.Country.ID == _selectedCountry.ID);
            }

            dataGridTovar.ItemsSource = query.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
            ApplyFilters();
        }


        private void AddProduct_Btn_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow();
            addProductWindow.SaveBtn.Content = "Добавить товар";
            addProductWindow.EditAddTxt.Text = "Добавление товар";
            addProductWindow.ShowDialog();
            LoadProducts();
        }

        private void EditTovar_Btn_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dataGridTovar.SelectedItem as Tovar;
            if (selectedProduct != null)
            {
                selectedProduct = (Tovar)dataGridTovar.SelectedItem;
                var addEditTovar = new AddProductWindow(selectedProduct); // Передаем выбранный товар в конструктор
                addEditTovar.SaveBtn.Content = "Редактировать товар";
                addEditTovar.EditAddTxt.Text = "Редактирование товара";
                addEditTovar.NameTxt.Text = selectedProduct.Name;
                addEditTovar.IDTxt.Text = selectedProduct.ID.ToString();
                addEditTovar.DescTxt.Text = selectedProduct.Description;
                addEditTovar.PriceTxt.Text = selectedProduct.Price.ToString();
                addEditTovar.WeightTxt.Text = selectedProduct.Size.ToString();
                addEditTovar.BrendCB.Text = selectedProduct.Brand?.Name ?? "Не определена";
                addEditTovar.CategoryCB.Text = selectedProduct.CategoryTovar?.Name ?? "Не определена";
                addEditTovar.CountryCB.Text = selectedProduct.Country?.Name ?? "Не определена";

                // Установка изображения
                string put = Environment.CurrentDirectory.ToString();
                put = put.Remove(put.Length - 10, 10);
                string Put = put + (selectedProduct.PhotoFull ?? "");
                addEditTovar.LargeProductImage.Source = string.IsNullOrEmpty(Put) ? null : new BitmapImage(new Uri(Put));
                addEditTovar.ShowDialog();
                LoadProducts();
            }
        }

        private void CategoryFilter_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoryFilter.SelectedItem as CategoryTovar;
            ApplyFilters();
        }

        private void BrandFilter_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _selectedBrand = BrandFilter.SelectedItem as Brand;
            ApplyFilters();

        }

        private void CountryFilter_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            _selectedCountry = CountryFilter.SelectedItem as Country;
            ApplyFilters();
        }
    }
}