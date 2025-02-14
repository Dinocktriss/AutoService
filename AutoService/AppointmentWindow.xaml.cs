using System;
using System.Linq;
using System.Windows;

namespace AutoService
{
    public partial class AppointmentWindow : Window
    {
        private Service service;

        public AppointmentWindow(Service service)
        {
            InitializeComponent();
            this.service = service;
            TxtServiceTitle.Text = $"{service.Title} ({service.DurationInMinutes} минут)";
            LoadClients();
        }

        private void LoadClients()
        {
            using (var context = new AutoServiceContext())
            {
                var clients = context.Clients.ToList();
                CmbClients.ItemsSource = clients;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClients.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }

            if (DpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.");
                return;
            }

            DateTime date = DpDate.SelectedDate.Value;
            TimeSpan time;
            if (!TimeSpan.TryParse(TxtStartTime.Text, out time))
            {
                MessageBox.Show("Введите корректное время.");
                return;
            }

            DateTime startTime = date.Date + time;
            DateTime endTime = startTime.AddMinutes(service.DurationInMinutes);

            using (var context = new AutoServiceContext())
            {
                var appointment = new Appointment
                {
                    ServiceID = service.ServiceID,
                    ClientID = ((Client)CmbClients.SelectedItem).ClientID,
                    StartTime = startTime,
                    
                };

                context.Appointments.Add(appointment);

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Запись успешно добавлена.");
                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }
    }
}
