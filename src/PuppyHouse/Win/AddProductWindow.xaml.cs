using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Pagess;
using Microsoft.Win32;
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
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        BD.BD_PuppyHouseEntities bd = new BD.BD_PuppyHouseEntities();
        public Tovar newTovar = new Tovar();
        private bool isNew;
        public AddProductWindow(Tovar tovar = null)
        {
            InitializeComponent();
            newTovar = tovar ?? new Tovar();
            isNew = tovar == null;
            DataContext = newTovar;
            LoadFilters();
        }

        private void LoadFilters()
        {
            CategoryCB.ItemsSource = bd.CategoryTovars.ToList();
            CategoryCB.SelectedItem = newTovar.CategoryTovar;
            BrendCB.ItemsSource = bd.Brands.ToList();
            BrendCB.SelectedItem = newTovar.Brand;
            CountryCB.ItemsSource = bd.Countries.ToList();
            CountryCB.SelectedItem = newTovar.Country;
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

            if (SaveBtn.Content.ToString() == "Добавить товар")
            {
                if (!double.TryParse(PriceTxt.Text, out double price))
                {
                    MessageBox.Show("Неверный формат цены. Пожалуйста, введите число.");
                    return;
                }

                newTovar.Name = NameTxt.Text;
                newTovar.Description = DescTxt.Text;
                newTovar.Price = price;
                newTovar.Size = WeightTxt.Text;
                newTovar.CategoryTovar = CategoryCB.SelectedItem as CategoryTovar;
                newTovar.Country = CountryCB.SelectedItem as Country;
                newTovar.Brand = BrendCB.SelectedItem as Brand;
                newTovar.Photo = System.IO.Path.GetFileName(((BitmapImage)LargeProductImage.Source).UriSource.ToString());

                bd.Tovars.Add(newTovar);
                bd.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            else if (SaveBtn.Content.ToString() == "Редактировать товар")
            {
                if (!double.TryParse(PriceTxt.Text, out double price))
                {
                    MessageBox.Show("Неверный формат цены. Пожалуйста, введите число.");
                    return;
                }
 
                int id = Convert.ToInt32(IDTxt.Text);
                var tovar = bd.Tovars.Where(x => x.ID == id).FirstOrDefault();
                if (tovar != null)
                {
                    tovar.Name = NameTxt.Text;
                    tovar.Description = DescTxt.Text;
                    tovar.Price = price;
                    tovar.Size = WeightTxt.Text;
                    tovar.Brand = BrendCB.SelectedItem as Brand;
                    tovar.CategoryTovar = CategoryCB.SelectedItem as CategoryTovar;
                    tovar.Country = CountryCB.SelectedItem as Country;
                    if (!string.IsNullOrEmpty(newTovar.Photo))
                    {
                        tovar.Photo = newTovar.Photo;
                    }
                    bd.SaveChanges();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Товар не найден!");
                }
            }
            ClearFields();
        }
        private void ClearFields()
        {
            NameTxt.Text = string.Empty;
            DescTxt.Text = string.Empty;
            PriceTxt.Text = string.Empty;
            WeightTxt.Text = string.Empty;
            CategoryCB.SelectedItem = null;
            BrendCB.SelectedItem = null;
            CountryCB.SelectedItem = null;
        }

        private void PhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем полный путь к выбранному изображению
                string filePath = openFileDialog.FileName.Trim();
                // Получаем только имя файла для сохранения в БД
                newTovar.Photo = System.IO.Path.GetFileName(filePath);

                // Отображаем изображение в элементе интерфейса
                LargeProductImage.Source = new BitmapImage(new Uri(filePath));
            }
        }
    }
}
