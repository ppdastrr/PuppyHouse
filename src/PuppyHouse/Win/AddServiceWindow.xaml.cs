using KP_4_PuppyHouse1.BD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для AddServiceWindow.xaml
    /// </summary>
    public partial class AddServiceWindow : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public Service newService = new Service();
        private bool isNew;
        public AddServiceWindow(Service service = null)
        {
            InitializeComponent();
            newService = service ?? new Service();
            isNew = service == null;
            DataContext = newService;
            LoadFilters();
        }
        private void LoadFilters()
        {
            CategoryCB.ItemsSource = bd.CategoryServices.ToList();
            CategoryCB.SelectedItem = newService.CategoryService;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SaveBtn.Content.ToString() == "Добавить услугу")
            {
                if (!double.TryParse(PriceTxt.Text, out double price))
                {
                    MessageBox.Show("Неверный формат цены. Пожалуйста, введите число.");
                    return;
                }

                newService.Name = NameTxt.Text;
                newService.Description = DescTxt.Text;
                newService.Price = price;
                newService.Time = TimeTxt.Text;
                newService.CategoryService = CategoryCB.SelectedItem as CategoryService;

                bd.Services.Add(newService);
                bd.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            else if (SaveBtn.Content.ToString() == "Редактировать услугу")
            {
                if (!double.TryParse(PriceTxt.Text, out double price))
                {
                    MessageBox.Show("Неверный формат цены. Пожалуйста, введите число.");
                    return;
                }

                int id = Convert.ToInt32(IDTxt.Text);
                var service = bd.Services.Where(x => x.ID == id).FirstOrDefault();
                if (service != null)
                {
                    service.Name = NameTxt.Text;
                    service.Description = DescTxt.Text;
                    service.Price = price;
                    service.Time = TimeTxt.Text;
                    service.CategoryService = CategoryCB.SelectedItem as CategoryService;
                    bd.SaveChanges();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Услуга не найдена!");
                }
            }
            ClearFields();
        }

        private void ClearFields()
        {
            NameTxt.Text = string.Empty;
            DescTxt.Text = string.Empty;
            PriceTxt.Text = string.Empty;
            TimeTxt.Text = string.Empty;
            CategoryCB.SelectedItem = null;
        }
    }
}
