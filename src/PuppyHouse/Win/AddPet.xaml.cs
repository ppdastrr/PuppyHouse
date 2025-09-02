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
using System.Windows.Shapes;

namespace KP_4_PuppyHouse1.Win
{
    /// <summary>
    /// Логика взаимодействия для AddPet.xaml
    /// </summary>
    public partial class AddPet : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private User _selectedUser;
        public AddPet(User selectedUser)
        {
            InitializeComponent();
            _selectedUser = selectedUser;

        }

        private void AddPetButton_Click(object sender, RoutedEventArgs e)
        {
            var petName = PetNameTextBox.Text;
            var petPoroda = PetPorodaTextBox.Text;
            var petAge = int.TryParse(PetAgeTextBox.Text, out var age) ? age : 0;

            if (string.IsNullOrEmpty(petName) || string.IsNullOrEmpty(petPoroda) || petAge <= 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Создаем нового питомца, привязываем его к выбранному пользователю
            var newDog = new Dog
            {
                Name = petName,
                Poroda = petPoroda,
                Age = petAge,
                ID_User = _selectedUser.ID  // Привязываем собаку к выбранному пользователю
            };

            // Сохраняем собаку в базе данных
            bd.Dogs.Add(newDog);
            bd.SaveChanges();

            MessageBox.Show("Питомец успешно добавлен!");

            // Закрытие окна
            this.Close();
        }
    }
}
