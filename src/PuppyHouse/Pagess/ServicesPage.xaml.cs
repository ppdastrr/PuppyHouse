using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Win;
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
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : UserControl
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private string _searchText;
        private CategoryService _selectedCategory;
        public ServicesPage()
        {
            InitializeComponent();
            LoadServices();
            LoadFilters();
        }

        private void LoadServices()
        {
            var services = bd.Services.ToList();
            dataGridServices.ItemsSource = services;
        }

        private void LoadFilters()
        {
            CategoryFilter.ItemsSource = bd.CategoryServices.ToList();
        }

        private void ApplyFilters()
        {
            var query = bd.Services.AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(s => s.Name.Contains(_searchText));
            }

            if (_selectedCategory != null)
            {
                query = query.Where(s => s.ID_Category == _selectedCategory.ID);
            }

            dataGridServices.ItemsSource = query.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
            ApplyFilters();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoryFilter.SelectedItem as CategoryService;
            ApplyFilters();
        }

        private void AddService_Btn_Click(object sender, RoutedEventArgs e)
        {
            var addServiceWindow = new AddServiceWindow();
            addServiceWindow.SaveBtn.Content = "Добавить услугу";
            addServiceWindow.EditAddTxt.Text = "Добавление услуги";
            addServiceWindow.ShowDialog();
            LoadServices();
        }


        private void EditService_Btn_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedService = dataGridServices.SelectedItem as Service;
            if (selectedService != null)
            {
                var addEditService = new AddServiceWindow(selectedService);
                addEditService.SaveBtn.Content = "Редактировать услугу";
                addEditService.EditAddTxt.Text = "Редактирование услуги";
                addEditService.NameTxt.Text = selectedService.Name;
                addEditService.IDTxt.Text = selectedService.ID.ToString();
                addEditService.DescTxt.Text = selectedService.Description;
                addEditService.PriceTxt.Text = selectedService.Price.ToString();
                addEditService.TimeTxt.Text = selectedService.Time;
                addEditService.CategoryCB.Text = selectedService.CategoryService?.Name ?? "Не определена";
                addEditService.ShowDialog();
                LoadServices();
            }
        }
    }
}
