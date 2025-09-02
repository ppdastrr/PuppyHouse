using KP_4_PuppyHouse1.BD;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace KP_4_PuppyHouse1.Win
{
    public partial class RegisterUserWindow : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public User newUser = new User();
        private bool isNew;

        public RegisterUserWindow(User user = null)
        {
            InitializeComponent();
            newUser = user ?? new User();
            isNew = user == null;
            DataContext = newUser;
            LoadFilters();
        }

        private void LoadFilters()
        {
            RoleComboBox.ItemsSource = bd.Roles.ToList();
            RoleComboBox.DisplayMemberPath = "Name";
            RoleComboBox.SelectedItem = newUser.Role;

            StatysEmpComboBox.ItemsSource = bd.StatusEmployees.ToList();
            StatysEmpComboBox.SelectedItem = newUser.StatusEmployee;
        }

        private void RegisterClient_Btn_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на наличие введенных данных
            if (string.IsNullOrEmpty(FullNameTextBox.Text) || string.IsNullOrEmpty(PhoneTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            // Создание нового пользователя
            var newUser = new User
            {
                FIO = FullNameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Email = EmailTextBox.Text,
                ID_Role = 4
            };

            // Добавляем нового пользователя в контекст базы данных
            bd.Users.Add(newUser);
            bd.SaveChanges();

            // Создание нового питомца для этого пользователя
            if (!string.IsNullOrEmpty(PetNameTextBox.Text) && !string.IsNullOrEmpty(PetAgeTextBox.Text) && !string.IsNullOrEmpty(PetPorodaTextBox.Text))
            {
                if (!int.TryParse(PetAgeTextBox.Text, out var petAge))
                {
                    MessageBox.Show("Неверный формат возраста питомца. Пожалуйста, введите число.");
                    return;
                }

                var newDog = new Dog
                {
                    Name = PetNameTextBox.Text,
                    Age = petAge,
                    Poroda = PetPorodaTextBox.Text,
                    ID_User = newUser.ID // Связываем питомца с пользователем
                };

                // Добавляем питомца в контекст базы данных
                bd.Dogs.Add(newDog);
                bd.SaveChanges();
            }

            MessageBox.Show("Пользователь успешно зарегистрирован!");

            // Закрываем окно
            this.Close();
        }

        private void RegisterEmploye_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterEmploye_Btn.Content.ToString() == "Добавить сотрудника")
            {
                // Проверка на наличие введенных данных
                if (string.IsNullOrEmpty(FullNameTextBox.Text) || string.IsNullOrEmpty(PhoneTextBox.Text) ||
                    string.IsNullOrEmpty(EmailTextBox.Text) || string.IsNullOrEmpty(LoginTextBox.Text) ||
                    string.IsNullOrEmpty(PasswordTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на уникальность логина
                if (bd.Users.Any(u => u.Login == LoginTextBox.Text))
                {
                    MessageBox.Show("Логин уже занят. Пожалуйста, выберите другой логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка пароля
                if (!IsValidPassword(PasswordTextBox.Text))
                {
                    MessageBox.Show("Пароль должен содержать не менее 8 символов, включая хотя бы одну строчную букву, одну заглавную букву и одну цифру.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создание нового пользователя
                var newUser = new User
                {
                    FIO = FullNameTextBox.Text,
                    Phone = PhoneTextBox.Text,
                    Email = EmailTextBox.Text,
                    Login = LoginTextBox.Text,
                    Password = PasswordTextBox.Text,
                    ID_Role = (RoleComboBox.SelectedItem as Role)?.ID ?? 1,
                    ID_StatusEmployee = (StatysEmpComboBox.SelectedItem as StatusEmployee)?.ID ?? 1
                };

                // Добавляем нового пользователя в контекст базы данных
                bd.Users.Add(newUser);
                bd.SaveChanges();
                MessageBox.Show("Пользователь успешно зарегистрирован!");
                this.Close();
            }
            else if (RegisterEmploye_Btn.Content.ToString() == "Редактировать сотрудника")
            {
                var employee = newUser; // Используем текущего пользователя
                if (employee != null)
                {
                    employee.FIO = FullNameTextBox.Text;
                    employee.Login = LoginTextBox.Text;
                    employee.Password = PasswordTextBox.Text;
                    employee.Email = EmailTextBox.Text;
                    employee.Phone = PhoneTextBox.Text;
                    employee.StatusEmployee = StatysEmpComboBox.SelectedItem as StatusEmployee;
                    employee.Role = RoleComboBox.SelectedItem as Role;

                    // Проверка пароля
                    if (!IsValidPassword(PasswordTextBox.Text))
                    {
                        MessageBox.Show("Пароль должен содержать не менее 8 символов, включая хотя бы одну строчную букву, одну заглавную букву и одну цифру.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    bd.SaveChanges();
                    MessageBox.Show("Пользователь успешно изменен!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Сотрудник не найден!");
                }
            }
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return false;
            }

            return true;
        }
    }
}