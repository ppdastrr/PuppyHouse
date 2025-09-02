using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using KP_4_PuppyHouse1.BD;
using KP_4_PuppyHouse1.Pagess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace KP_4_PuppyHouse1.Win
{
    /// <summary>
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : System.Windows.Window
    {
        BD.BD_PuppyHouseEntities bd = new BD.BD_PuppyHouseEntities();
        private List<SpisokTovar> _cart;
        private User _currentUser;
        public decimal TotalPrice { get; private set; }
        public CartWindow(User currentUser)
        {
            InitializeComponent();
            DataContext = this;
            LoadCart();
            _currentUser = currentUser;
        }
        private void LoadCart()
        {
            // Загрузка товаров, у которых ID_Prodazha равно null
            _cart = bd.SpisokTovars
                        .Where(st => st.ID_Prodazha == null)
                        .ToList();

            // Расчет общей суммы
            TotalPrice = (decimal)_cart.Sum(st => st.Tovar.Price * st.Count ?? 0);

            // Привязка данных к ListBox
            cartDataGrid.ItemsSource = _cart;

            // Обновление отображения общей суммы
            TotalPriceTextBlock.Text = "Общая сумма: " + TotalPrice.ToString("C");
        }
        private void ShowCheckOptions()
        {
            // Создаем окно с вариантами чека
            var result = MessageBox.Show(
                "Выберите тип чека:\n\nДа - Напечатать бумажный чек\nНет - Электронный чек\nОтмена - Без чека",
                "Выбор типа чека",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    PrintPaperCheck();
                    break;
                case MessageBoxResult.No:
                    GenerateElectronicCheck();
                    break;
                case MessageBoxResult.Cancel:
                    CompleteTransactionWithoutCheck();
                    break;
            }
        }
        private void PrintPaperCheck()
        {
            GeneratePaperCheck();
            CompleteTransaction();
        }
        private void GeneratePaperCheck()
        {
            var application = new Microsoft.Office.Interop.Word.Application();
            var document = application.Documents.Add();
            // Шапка документа
            var header = document.Paragraphs.Add();
            header.Range.Text = "ООО \"Цветучено\"\nДобро пожаловать\nККМ 00075411    #3969\nИНН 1087746942040\nЭКЛЗ 3851495566\nЧек № : " + GenerateRandomNumber(6) + "\n0 СИС.\n" + DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            header.Range.Font.Bold = 1;
            header.Range.Font.Color = WdColor.wdColorBlueGray; // Темно-фиолетовый цвет
            header.Range.InsertParagraphAfter();
            // Таблица с товарами
            var table = document.Tables.Add(document.Paragraphs.Last.Range, _cart.Count + 1, 4);
            table.Borders.Enable = 1;
            table.Range.Font.Color = WdColor.wdColorBlueGray; // Темно-фиолетовый цвет
            // Заголовки таблицы
            table.Cell(1, 1).Range.Text = "Наименование товара";
            table.Cell(1, 2).Range.Text = "Количество";
            table.Cell(1, 3).Range.Text = "Размер скидки";
            table.Cell(1, 4).Range.Text = "Итого";
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.Font.Color = WdColor.wdColorBlueGray; // Темно-фиолетовый цвет
            for (int i = 0; i < _cart.Count; i++)
            {
                double sum = (double)(_cart[i].Tovar.Price * _cart[i].Count);
                table.Cell(i + 2, 1).Range.Text = _cart[i].Tovar.Name;
                table.Cell(i + 2, 2).Range.Text = _cart[i].Count.ToString();
                table.Cell(i + 2, 3).Range.Text = "0%"; // Пример скидки
                table.Cell(i + 2, 4).Range.Text = sum.ToString("C");
            }
            // Подвал документа
            var footer = document.Paragraphs.Add();
            decimal totalSum = (decimal)_cart.Sum(st => st.Tovar.Price * st.Count ?? 0);
            footer.Range.Text = $"Итого: {totalSum:C}";
            footer.Range.Font.Bold = 1;
            footer.Range.Font.Color = WdColor.wdColorBlueGray; // Темно-фиолетовый цвет
            // Сохранение документа
            string projectRootPath = AppContext.BaseDirectory;
            string fileName = $"Check_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            string fullPath = System.IO.Path.Combine(projectRootPath, fileName);
            document.SaveAs2(fullPath);
            document.Close();
            application.Quit();
            // Сообщение о сохранении чека
            MessageBox.Show("Чек успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("0123456789", length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void GenerateElectronicCheck()
        {
            // Создаем окно для ввода почты
            var emailInputWindow = new EmailInputWindow();
            if (emailInputWindow.ShowDialog() == true)
            {
                string recipientEmail = emailInputWindow.Email;
                // Отправляем электронный чек
                bool emailSent = SendEmail(recipientEmail);
                if (emailSent)
                {
                    MessageBox.Show("Электронный чек отправлен на email клиента!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    CompleteTransaction();
                }
                else
                {
                    MessageBox.Show("Ошибка при отправке электронного чека.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool SendEmail(string recipientEmail)
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
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(from, to)
                {
                    Subject = "Электронный чек",
                    IsBodyHtml = true,
                    Body = $@"
        <h2>Электронный чек</h2>
        <p><strong>Уважаемый клиент,</strong></p>
        <p>Спасибо за использование наших услуг! Надеемся ваш питомец и Вы остались довольными!</p>
        <table>
            <tr><td><strong>Наименование товара</strong></td><td>Количество</td><td>Сумма</td></tr>
            {string.Join("", _cart.Select(item => $"<tr><td>{item.Tovar.Name}</td><td>{item.Count}</td><td>{(item.Tovar.Price * item.Count):C}</td></tr>"))}
        </table>
        <p>Итоговая сумма: {_cart.Sum(st => st.Tovar.Price * st.Count ?? 0):C}</p>
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
        private void CompleteTransactionWithoutCheck()
        {
            // Завершение без чека
            MessageBox.Show("Продажа завершена без чека!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            CompleteTransaction();
        }
        private void CompleteTransaction()
        {
            // Текущая логика оформления продажи
            var prodazha = new Prodazha
            {
                Date = DateTime.Now,
                ID_Employee = _currentUser.ID,
            };
            bd.Prodazhas.Add(prodazha);
            bd.SaveChanges();
            foreach (var item in _cart)
            {
                item.ID_Prodazha = prodazha.ID;

                var postavki = bd.Postavkas
                    .Where(p => p.ID_Tovar == item.ID_Tovar)
                    .OrderBy(p => p.Date)
                    .ToList();
                int remainingQuantity = (int)item.Count;
                foreach (var postavka in postavki)
                {
                    if (remainingQuantity <= 0) break;

                    if (postavka.Count >= remainingQuantity)
                    {
                        postavka.Count -= remainingQuantity;
                        remainingQuantity = 0;
                    }
                    else
                    {
                        remainingQuantity -= (int)postavka.Count;
                        postavka.Count = 0;
                    }
                }
            }
            bd.SaveChanges();
            LoadCart();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверка наличия товаров на складе (остается без изменений)
            var insufficientItems = new List<SpisokTovar>();
            foreach (var item in _cart)
            {
                var availableQuantity = bd.Postavkas
                    .Where(p => p.ID_Tovar == item.ID_Tovar)
                    .Sum(p => p.Count) ?? 0;

                if (item.Count > availableQuantity)
                {
                    insufficientItems.Add(item);
                }
            }
            if (insufficientItems.Any())
            {
                var message = "Недостаточно товаров на складе для следующих позиций:\n";
                foreach (var item in insufficientItems)
                {
                    message += $"{item.Tovar.Name} - Требуется: {item.Count}, Доступно: {bd.Postavkas.Where(p => p.ID_Tovar == item.ID_Tovar).Sum(p => p.Count) ?? 0}\n";
                }
                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Отображение вариантов чека
            ShowCheckOptions();
        }
        private void RemoveFromCart(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var cartItem = button.DataContext as SpisokTovar;
                if (cartItem != null)
                {
                    bd.SpisokTovars.Remove(cartItem);
                    bd.SaveChanges();
                    LoadCart();
                }
            }
        }

    }
}