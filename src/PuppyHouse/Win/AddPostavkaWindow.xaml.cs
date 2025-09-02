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
    /// Логика взаимодействия для AddPostavkaWindow.xaml
    /// </summary>
    public partial class AddPostavkaWindow : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public Postavka newPostavka = new Postavka();
        public AddPostavkaWindow()
        {
            InitializeComponent();
            DataContext = newPostavka;
            LoadFilters();
        }
        private void LoadFilters()
        {
            TovarCB.ItemsSource = bd.Tovars.ToList();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

            if (!int.TryParse(CountTxt.Text, out int count))
            {
                MessageBox.Show("Неверный формат количества. Пожалуйста, введите целое число.");
                return;
            }

            // Проверка на отрицательное число
            if (count < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным.");
                return;
            }

            // Проверка на число больше 1 000 000
            if (count > 1000000)
            {
                MessageBox.Show("Количество не может превышать 1 000 000.");
                return;
            }

            newPostavka.Tovar = TovarCB.SelectedItem as Tovar;
            newPostavka.Count = count;
            newPostavka.Date = DateTime.Now;

            bd.Postavkas.Add(newPostavka);
            bd.SaveChanges();
            MessageBox.Show("Поставка добавлена!", "Успех!");
            this.DialogResult = true;
            this.Close();
        }
    }
}
