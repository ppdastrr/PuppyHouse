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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : UserControl
    {
        BD.BD_PuppyHouseEntities bd = new BD.BD_PuppyHouseEntities();
        private List<BD.User> allEmployees;
        public EmployeesPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            // Загружаем данные из базы
            allEmployees = bd.Users
                .Where(u => u.ID_Role == 1 || u.ID_Role == 2 || u.ID_Role == 3)
                .ToList();

            // Наполняем фильтры
            PositionFilter.ItemsSource = allEmployees
                .Select(e => e.Role.Name) // Предполагается, что у Role есть Name
                .Distinct()
                .ToList();

            StatusFilter.ItemsSource = allEmployees
                .Select(e => e.StatusEmployee != null ? e.StatusEmployee.Name : string.Empty)
                .Distinct()
                .ToList();
            // Отображаем данные
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            // Получаем значения фильтров
            string searchText = SearchBox.Text.ToLower();
            string selectedPosition = PositionFilter.SelectedItem?.ToString();
            string selectedStatus = StatusFilter.SelectedItem?.ToString();

            // Фильтруем данные
            var filtered = allEmployees.Where(e =>
                (string.IsNullOrEmpty(searchText) || e.FIO.ToLower().Contains(searchText)) &&
                (string.IsNullOrEmpty(selectedPosition) || e.Role.Name == selectedPosition) &&
                (string.IsNullOrEmpty(selectedStatus) || e.StatusEmployee.Name == selectedStatus));

            // Обновляем таблицу
            dataGrid.ItemsSource = filtered.ToList();
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PositionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }


        private void AddEmployy_Btn_Click(object sender, RoutedEventArgs e)
        {
            RegisterUserWindow registerUserWindow = new RegisterUserWindow();
            registerUserWindow.AddPetTab.Visibility = Visibility.Collapsed;
            registerUserWindow.StatysEmpComboBox.Visibility = Visibility.Collapsed;
            registerUserWindow.StatusLbl.Visibility = Visibility.Collapsed;
            registerUserWindow.RegisterClient_Btn.Visibility = Visibility.Collapsed;
            registerUserWindow.RoleComboBox.Visibility = Visibility.Visible;
            registerUserWindow.EditAddLbl.Text = "Добавление сотрудника";
            registerUserWindow.RegisterEmploye_Btn.Content = "Добавить сотрудника";
            registerUserWindow.Show();
        }

        private void EditEmploye_Btn_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmp = dataGrid.SelectedItem as User;
            if (selectedEmp != null)
            {
                selectedEmp = (User)dataGrid.SelectedItem;
                var addEditEmp = new RegisterUserWindow(selectedEmp); // Передаем выбранный товар в конструктор
                addEditEmp.RegisterEmploye_Btn.Content = "Редактировать сотрудника";
                addEditEmp.EditAddLbl.Text = "Редактирование сотрудника";
                addEditEmp.FullNameTextBox.Text = selectedEmp.FIO;
                addEditEmp.IDTxt.Text = selectedEmp.ID.ToString();
                addEditEmp.PhoneTextBox.Text = selectedEmp.Phone;
                addEditEmp.EmailTextBox.Text = selectedEmp.Email.ToString();
                addEditEmp.LoginTextBox.Text = selectedEmp.Login.ToString();
                addEditEmp.PasswordTextBox.Text = selectedEmp.Password.ToString();
                addEditEmp.RoleComboBox.Text = selectedEmp.Role?.Name ?? "Не определена";
                addEditEmp.StatysEmpComboBox.Text = selectedEmp.StatusEmployee?.Name ?? "Не определена";
                addEditEmp.ShowDialog();
                LoadData();

            }
        }
    }
}
