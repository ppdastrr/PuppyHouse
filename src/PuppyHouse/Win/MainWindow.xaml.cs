using KP_4_PuppyHouse1.BD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace KP_4_PuppyHouse1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTxt.Text;
            string password = PasswordTxt.Password;

            // Хешируем введенный пароль
            SHA256 sha256Hash = SHA256.Create();
            byte[] passwordHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            string hash = Convert.ToBase64String(passwordHash);

            var user = bd.Users.FirstOrDefault(u => u.Login == username && u.Password == password);

            if (user != null)
            {
                // Проверяем статус сотрудника
                if (user.ID_StatusEmployee == 2)
                {
                    MessageBox.Show("Ваш аккаунт уволен. Вход в систему запрещен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Открываем соответствующее окно в зависимости от роли пользователя
                switch (user.ID_Role)
                {
                    case 1:
                        MessageBox.Show("Здравствуй администратор " + user.FIO, "Успех!");
                        var adminWindow = new Win.MainWin(user);
                        adminWindow.Show();
                        break;
                    case 2:
                        MessageBox.Show("Здравствуй кассир-консультант " + user.FIO, "Успех!");
                        var cashierWindow = new Win.MainWin(user);
                        cashierWindow.Show();
                        break;
                    case 3:
                        MessageBox.Show("Здравствуй мастер " + user.FIO, "Успех!");
                        var managerWindow = new Win.MainWin(user);
                        managerWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя.");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
