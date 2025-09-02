using KP_4_PuppyHouse1.BD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KP_4_PuppyHouse1.Win
{
    public partial class AddNote : Window
    {
        private BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private List<Dog> _dogs;
        private List<Service> _services;
        private NoteService _noteService;
        public AddNote(NoteService noteService)
        {
            InitializeComponent();
            _noteService = noteService;
            LoadData();
        }
        private void LoadData()
        {
            // Загрузка собак с загрузкой связанных данных о владельце
            _dogs = bd.Dogs.Include("User").ToList();

            // Привязка данных к DataGrid
            dataGridClients.ItemsSource = _dogs;

            // Загрузка услуг
            _services = bd.Services.ToList();
            dataGridServices.ItemsSource = _services;

            // Установка выбранного времени и мастера
            SelectedTimeTextBlock.Text = "Время: " + _noteService.Date.ToString();
            SelectedMasterTextBlock.Text = "Мастер: " + _noteService.User.FIO;

            // Загрузка категорий
            CategoryFilter.ItemsSource = bd.CategoryServices.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            // Фильтрация собак по имени владельца
            var filteredDogs = _dogs
                               .Where(d => d.User.FIO.ToLower().Contains(searchText))
                               .ToList();

            // Привязка отфильтрованных данных к DataGrid
            dataGridClients.ItemsSource = filteredDogs;
        }

        private void AddNote_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridServices.SelectedItem != null && dataGridClients.SelectedItem != null)
            {
                var selectedDog = dataGridClients.SelectedItem as Dog;
                var selectedService = dataGridServices.SelectedItem as Service;

                // Используем новый контекст для обновления записи
                using (var newBd = new BD_PuppyHouseEntities())
                {
                    // Найдем запись по ID
                    var noteServiceToUpdate = newBd.NoteServices.Find(_noteService.ID);

                    if (noteServiceToUpdate != null)
                    {
                        // Обновление записи
                        noteServiceToUpdate.ID_Dog = selectedDog.ID;
                        noteServiceToUpdate.ID_Service = selectedService.ID;
                        noteServiceToUpdate.ID_Status = 2; // Установка статуса 2

                        newBd.SaveChanges();

                        MessageBox.Show("Запись успешно обновлена.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Запись не найдена.");
                    }
                }
            }
        }

        private void SearchBoxServices_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBoxServices.Text.ToLower();

            // Фильтрация услуг по имени
            var filteredServices = _services
                                   .Where(s => s.Name.ToLower().Contains(searchText))
                                   .ToList();

            // Привязка отфильтрованных данных к DataGrid
            dataGridServices.ItemsSource = filteredServices;
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = CategoryFilter.SelectedItem as CategoryService;
            if (selectedCategory != null)
            {
                // Фильтрация услуг по выбранной категории
                dataGridServices.ItemsSource = _services.Where(s => s.ID_Category == selectedCategory.ID).ToList();
            }
            else
            {
                // Если категория не выбрана, показываем все услуги
                dataGridServices.ItemsSource = _services;
            }
        }

        private void AddClient_Btn_Click(object sender, RoutedEventArgs e)
        {
            RegisterUserWindow registerUserWindow = new RegisterUserWindow();
            registerUserWindow.LoginTextBox.Visibility = Visibility.Collapsed;
            registerUserWindow.PasswordTextBox.Visibility = Visibility.Collapsed;
            registerUserWindow.RoleComboBox.Visibility = Visibility.Collapsed;
            registerUserWindow.RoleLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.PasswordLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.LoginLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.StatysEmpComboBox.Visibility = Visibility.Collapsed;
            registerUserWindow.LoginLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.StatusLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.ShowDialog();

        }

        private void AddPet_Btn_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранного пользователя (клиента) из dataGrid
            var selectedUser = dataGridClients.SelectedItem as Dog;

            if (selectedUser == null)
            {
                MessageBox.Show("Пожалуйста, выберите клиента.");
                return;
            }

            // Получаем пользователя, к которому привязана собака
            var user = selectedUser.User;  // Собака связана с пользователем через свойство User

            // Открываем окно добавления питомца и передаем выбранного пользователя
            AddPet addPetWindow = new AddPet(user);  // Передаем пользователя (вместо собаки)
            addPetWindow.ShowDialog();
        }
    }
}