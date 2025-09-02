using KP_4_PuppyHouse1.BD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace KP_4_PuppyHouse1.Win
{
    public partial class AddRecordWindow : Window
    {
        BD_PuppyHouseEntities bd = new BD_PuppyHouseEntities();
        private List<User> _masters;
        private List<TimeSpan> _availableTimes;
        private NoteService _noteService;
        public AddRecordWindow(NoteService noteService)
        {
            InitializeComponent();
            LoadMasters();
            LoadAvailableTimes();
            _noteService = noteService;
        }
        private void LoadMasters()
        {
            // Загружаем мастеров с ролью 3 из базы данных
            _masters = bd.Users
                         .Where(u => u.ID_Role == 3)
                         .ToList(); // Получаем сущности User напрямую
            MasterComboBox.ItemsSource = _masters;
        }
        private void LoadAvailableTimes()
        {
            // Заполняем доступные временные интервалы
            _availableTimes = new List<TimeSpan>
            {
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0),
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                new TimeSpan(14, 0, 0),
                new TimeSpan(15, 0, 0),
                new TimeSpan(16, 0, 0),
                new TimeSpan(17, 0, 0)
            };
            TimeComboBox.ItemsSource = _availableTimes;
        }
        private void AddSchedule_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate.HasValue && TimeComboBox.SelectedItem != null && MasterComboBox.SelectedItem != null)
            {
                // Получаем выбранное время
                TimeSpan selectedTime = (TimeSpan)TimeComboBox.SelectedItem;
                DateTime fullDateTime = DatePicker.SelectedDate.Value.Date + selectedTime;
                // Получаем ID мастера
                int masterId = ((User)MasterComboBox.SelectedItem).ID;
                // Проверяем, не занято ли время для выбранного мастера
                bool isTimeOccupied = IsTimeOccupied(fullDateTime, masterId);
                if (isTimeOccupied)
                {
                    MessageBox.Show("Время уже занято для выбранного мастера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Если время не занято, добавляем или обновляем запись
                    if (_noteService != null)
                    {
                        // Проверяем статус записи
                        if (_noteService.ID_Status == 2 || _noteService.ID_Status == 3)
                        {
                            MessageBox.Show("Редактирование записи с статусом 'Занято' или 'Выполнено' не разрешено.");
                        }
                        else
                        {
                            UpdateRecordInDatabase(fullDateTime, masterId);
                        }
                    }
                    else
                    {
                        AddRecordToDatabase(fullDateTime, masterId);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату, время и мастера.");
            }
        }
        private void UpdateRecordInDatabase(DateTime dateTime, int masterId)
        {
            _noteService.Date = dateTime;
            _noteService.ID_Master = masterId;

            bd.SaveChanges();

            MessageBox.Show("Запись успешно обновлена.");
            this.Close();
        }
        private bool IsTimeOccupied(DateTime dateTime, int masterId)
        {
            // Проверка в базе данных на занятость времени
            return bd.NoteServices.Any(ns => ns.Date == dateTime && ns.ID_Master == masterId);
        }
        private void AddRecordToDatabase(DateTime dateTime, int masterId)
        {
            var newRecord = new NoteService
            {
                Date = dateTime,
                ID_Master = masterId,
                ID_Status = 1
            };

            bd.NoteServices.Add(newRecord);
            bd.SaveChanges();

            MessageBox.Show("Запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

    }
}
