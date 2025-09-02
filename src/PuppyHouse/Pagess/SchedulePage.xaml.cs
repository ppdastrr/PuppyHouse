using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Win;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KP_4_PuppyHouse1.Pagess
{
    /// <summary>
    /// Логика взаимодействия для SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : UserControl
    {
        private BD.BD_PuppyHouseEntities bd = new BD.BD_PuppyHouseEntities();
        private List<NoteService> _scheduleEntries;
        private User _currentUser;

        public Visibility AdminEditVisibility { get; set; }
        public Visibility CashierEditVisibility { get; set; }
        public Visibility MasterEditVisibility { get; set; }
        public SchedulePage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            SetButtonVisibility(); // Устанавливаем видимость кнопок
            this.DataContext = this;
            LoadData();
        }
        private void SetButtonVisibility()
        {
            switch (_currentUser.ID_Role)
            {
                case 1: // Администратор
                    AdminEditVisibility = Visibility.Visible;
                    CashierEditVisibility = Visibility.Collapsed;
                    MasterEditVisibility = Visibility.Collapsed;
                    AddShedule_Btn.Visibility = Visibility.Visible;
                    break;
                case 2: // Кассир-консультант
                    AdminEditVisibility = Visibility.Collapsed;
                    CashierEditVisibility = Visibility.Visible;
                    MasterEditVisibility = Visibility.Collapsed;
                    AddShedule_Btn.Visibility = Visibility.Collapsed;
                    break;
                case 3: // Мастер
                    AdminEditVisibility = Visibility.Collapsed;
                    CashierEditVisibility = Visibility.Collapsed;
                    MasterEditVisibility = Visibility.Visible;
                    AddShedule_Btn.Visibility = Visibility.Collapsed;
                    break;
                default:
                    AdminEditVisibility = Visibility.Collapsed;
                    CashierEditVisibility = Visibility.Collapsed;
                    MasterEditVisibility = Visibility.Collapsed;
                    AddShedule_Btn.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void LoadData()
        {
            // Загрузка всех данных
            _scheduleEntries = bd.NoteServices.ToList();

            // Загрузка мастеров и услуг для ComboBox
            MasterComboBox.ItemsSource = bd.Users.Where(u => u.ID_Role == 3).ToList();
            ServiceComboBox.ItemsSource = bd.StatusNotes.ToList();

            // Привязка данных к ListBox с фильтрацией
            FilterRecords();
        }

        private void FilterRecords()
        {
            var selectedMaster = MasterComboBox.SelectedItem as User;
            var selectedService = ServiceComboBox.SelectedItem as StatusNote;
            var startDate = StartDatePicker.SelectedDate;
            var endDate = EndDatePicker.SelectedDate;

            var filteredEntries = _scheduleEntries.AsQueryable();
            // Если текущий пользователь - мастер, показываем только его записи
            if (_currentUser.ID_Role == 3)
            {
                filteredEntries = filteredEntries.Where(ns => ns.ID_Master == _currentUser.ID);
            }
            // Если не выбран период, показываем записи включая сегодняшний день
            if (startDate == null && endDate == null)
            {
                filteredEntries = filteredEntries.Where(ns => ns.Date >= DateTime.Today); // Показываем записи начиная с сегодняшнего дня
            }
            else
            {
                // Если выбран период, фильтруем по этому диапазону
                if (startDate != null)
                {
                    filteredEntries = filteredEntries.Where(ns => ns.Date >= startDate);
                }

                if (endDate != null)
                {
                    filteredEntries = filteredEntries.Where(ns => ns.Date <= endDate);
                }
            }

            // Фильтрация по мастеру и услуге
            if (selectedMaster != null)
            {
                filteredEntries = filteredEntries.Where(ns => ns.ID_Master == selectedMaster.ID);
            }

            if (selectedService != null)
            {
                filteredEntries = filteredEntries.Where(ns => ns.ID_Status == selectedService.ID);
            }

            // Сортировка записей по дате по возрастанию
            filteredEntries = filteredEntries.OrderBy(ns => ns.Date);

            // Обновляем источники для отображения
            ScheduleItemsControl.ItemsSource = filteredEntries.ToList();
        }

        private void ApplyFilters()
        {
            FilterRecords();

        }

        private void MasterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var noteService = DataContext as NoteService;
            AddRecordWindow addNote = new AddRecordWindow(noteService);
            addNote.Show();
        }

        private void EditNoteKassir_Btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var noteService = button.DataContext as NoteService;

            // Проверка статуса записи
            if (noteService.ID_Status == 2 || noteService.ID_Status == 3 || noteService.ID_Status == 4) // 3 - "Выполнено", 4 - "Оплачено"
            {
                MessageBox.Show("Редактирование невозможно, так как статус записи 'Занято'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var editWindow = new AddNote(noteService);
            editWindow.ShowDialog();

            // Обновление данных после закрытия окна редактирования
            LoadData();
        }

        private void EditNoteAdmin_Btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var noteService = button.DataContext as NoteService;

            var editWindow = new AddRecordWindow(noteService);
            editWindow.MasterComboBox.Text = noteService.User?.FIO ?? "Не указан";
            editWindow.TimeComboBox.Text = noteService.Date?.ToString();
            editWindow.ShowDialog();

            // Обновление данных после закрытия окна редактирования
            LoadData();
        }

        private void EditNoteMaster_Btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var noteService = button.DataContext as NoteService;
            // Проверка статуса записи
            if (noteService.ID_Status == 1 ||  noteService.ID_Status == 4) 
            {
                MessageBox.Show("Можно выполнить только заявки с клиентом!.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Изменяем статус записи на код 3
            noteService.ID_Status = 3;

            // Сохраняем изменения в базе данных
            bd.SaveChanges();

            // Обновляем данные после изменения статуса
            LoadData();

            MessageBox.Show("Статус записи успешно изменен.");
        }




        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Oplata_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleItemsControl.SelectedItem is NoteService selectedNote)
            {
                // Проверка статуса
                if (selectedNote.StatusNote.Name == "Выполнено")
                {
                    // Запрос подтверждения оплаты
                    var result = MessageBox.Show("Вы хотите отправить подтверждение оплаты?", "Подтверждение оплаты", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Вызываем метод для отправки письма и обновления статуса
                        bool paymentConfirmed = SendPaymentConfirmation(selectedNote);

                        if (paymentConfirmed)
                        {
                            MessageBox.Show("Электронный чек отправлен на почту клиента!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при отправке подтверждения оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Оплату можно произвести только для выполненных записей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool SendPaymentConfirmation(NoteService noteService)
        {
            // Получаем данные из записи NoteService
            User user = noteService.User; // Клиент
            Dog dog = noteService.Dog; // Питомец
            Service service = noteService.Service; // Услуга
            User master = noteService.ID_Master.HasValue ? bd.Users.FirstOrDefault(u => u.ID == noteService.ID_Master) : null; // Мастер

            // Получаем данные для письма
            string recipientEmail = user.Email;

            // Проверяем, что email клиента не пустой
            if (string.IsNullOrEmpty(recipientEmail))
            {
                MessageBox.Show("У клиента не указан email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Прерываем выполнение, если email пустой
            }

            string userName = user.FIO;
            string serviceName = service.Name;
            string serviceDate = noteService.Date?.ToString("dd.MM.yyyy HH:mm") ?? "Дата не указана";

            // Проверка на null для цены
            double? servicePrice = service.Price;

            // Проверка на количество записей для собаки
            int dogServiceCount = bd.NoteServices.Count(ns => ns.ID_Dog == dog.ID);
            double discountPercentage = 0;
            string discountMessage = "";

            if (dogServiceCount > 10)
            {
                discountPercentage = 0.10; // 10% скидка
                discountMessage = "У вас есть скидка 10%! Новая цена: ";
            }
            else if (dogServiceCount > 5)
            {
                discountPercentage = 0.05; // 5% скидка
                discountMessage = "У вас есть скидка 5%! Новая цена: ";
            }

            // Расчет скидки и новой цены
            if (discountPercentage > 0 && servicePrice.HasValue)
            {
                double discountAmount = servicePrice.Value * discountPercentage;
                double discountedPrice = servicePrice.Value - discountAmount;
                noteService.Discount = discountAmount; // Сохраняем скидку в базе данных
                noteService.DiscountedPrice = discountedPrice; // Сохраняем новую цену в базе данных

                // Логирование для отладки
                MessageBox.Show($"Скидка: {discountAmount}, Старая цена: {servicePrice.Value}, Новая цена: {discountedPrice}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            string dogName = dog?.Name ?? "Питомец не указан"; // Имя собаки (если есть)
            string masterName = master?.FIO ?? "Мастер не указан"; // Имя мастера (если есть)

            // Форматирование цены с учетом скидки
            string formattedServicePrice = noteService.DiscountedPrice?.ToString("F2") ?? "Не указана";
            string formattedOldPrice = servicePrice?.ToString("F2") ?? "Не указана";

            // Сообщение о скидке
            if (!string.IsNullOrEmpty(discountMessage))
            {
                discountMessage += formattedServicePrice + " руб.";
            }

            // Отправляем email
            bool emailSent = SendEmail(recipientEmail, userName, serviceName, serviceDate, formattedServicePrice, dogName, masterName, discountMessage, formattedOldPrice);

            if (emailSent)
            {
                // Меняем статус записи на "Оплачено"
                noteService.ID_Status = 4;

                // Сохраняем изменения в базе данных
                bd.SaveChanges();

                return true; // Успешно отправлено и статус изменен
            }

            return false; // Ошибка при отправке письма
        }

        private bool SendEmail(string recipientEmail, string userName, string serviceName, string serviceDate, string servicePrice, string dogName, string masterName, string discountMessage, string oldPrice)
        {
            try
            {
                // Убедитесь, что email получателя не пуст
                if (string.IsNullOrEmpty(recipientEmail))
                {
                    throw new ArgumentException("Получатель не указан.");
                }

                // Настройка отправителя и получателя
                MailAddress from = new MailAddress("julia.fedotova.04@mail.ru", "PuppyHouse");
                MailAddress to = new MailAddress(recipientEmail);

                // Создание письма
                MailMessage mailMessage = new MailMessage(from, to)
                {
                    Subject = "Электронный чек",
                    IsBodyHtml = true,
                    Body = $@"
        <h2>Электронный чек</h2>
        <p><strong>Уважаемый клиент,</strong></p>
        <p>Спасибо за использование наших услуг! Надеемся ваш питомец и Вы остались довольными!</p>
        <table>
            <tr><td><strong>Услуга:</strong></td><td>{serviceName}</td></tr>
            <tr><td><strong>Дата:</strong></td><td>{serviceDate}</td></tr>
            <tr><td><strong>Старая цена:</strong></td><td>{oldPrice} руб.</td></tr>
            <tr><td><strong>Новая цена:</strong></td><td>{servicePrice} руб.</td></tr>
            <tr><td><strong>Питомец:</strong></td><td>{dogName}</td></tr>
            <tr><td><strong>Мастер:</strong></td><td>{masterName}</td></tr>
        </table>
        <p>{discountMessage}</p>
        <p>С уважением,<br />Салон для собак 'PuppyHouse'</p>"
                };

                // Настройка SMTP клиента для Mail.ru
                SmtpClient smtpClient = new SmtpClient("smtp.mail.ru", 587) // или 465 для SSL
                {
                    Credentials = new NetworkCredential("julia.fedotova.04@mail.ru", "gKvY1srGsnBd5SiGtyCQ"), // Замените на ваши данные
                    EnableSsl = true // Используем TLS
                };

                // Отправка письма
                smtpClient.Send(mailMessage);

                return true; // Успешно отправлено
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show($"Ошибка при отправке письма: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Ошибка при отправке письма
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ScheduleItemsControl.SelectedItem is NoteService selectedNote)
            {
                // Проверка, что запись выбрана
                if (selectedNote == null)
                {
                    MessageBox.Show("Выберите запись для отмены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка статуса записи
                if (selectedNote.ID_Status != 1)
                {
                    MessageBox.Show("Отменить можно только свободную запись'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Запрос подтверждения отмены
                var result = MessageBox.Show("Вы уверены, что хотите отменить запись?", "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Удаляем запись из базы данных
                    bd.NoteServices.Remove(selectedNote);
                    bd.SaveChanges();

                    // Обновляем данные после удаления записи
                    LoadData();

                    MessageBox.Show("Запись успешно отменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
