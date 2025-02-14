using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AutoService
{
    public partial class UpcomingAppointmentsWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();

        public UpcomingAppointmentsWindow()
        {
            InitializeComponent();
            LoadAppointments();

            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void LoadAppointments()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            DateTime now = DateTime.Now;

            using (var context = new AutoServiceContext())
            {
                var appointments = context.Appointments
                    .Where(a => a.StartTime >= today && a.StartTime < tomorrow)
                    .Include(a => a.Client) // Убедитесь, что данные клиента загружены
                    .Include(a => a.Service) // Если необходимо, загрузите данные об услуге
                    .OrderBy(a => a.StartTime)
                    .ToList(); // Загрузка данных из базы данных

                var appointmentList = appointments.Select(a => new
                {
                    a.Service,
                    ClientFullName = $"{a.Client.LastName} {a.Client.FirstName} {a.Client.MiddleName}",
                    a.Client,
                    a.StartTime,
                    TimeRemaining = (a.StartTime - now).TotalMinutes,
                    TimeRemainingFormatted = GetTimeRemainingFormatted(a.StartTime)
                }).ToList();

                DgAppointments.ItemsSource = appointmentList;
            }
        }


        private string GetTimeRemainingFormatted(DateTime startTime)
        {
            TimeSpan timeRemaining = startTime - DateTime.Now;
            if (timeRemaining.TotalSeconds <= 0)
                return "Уже началось";
            else
            {
                string result = "";
                if (timeRemaining.Hours > 0)
                    result += $"{timeRemaining.Hours} час(а) ";
                if (timeRemaining.Minutes > 0)
                    result += $"{timeRemaining.Minutes} минут";
                if (timeRemaining.TotalMinutes < 60)
                    result = $"[!] {result}";
                return result;
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            LoadAppointments();
        }
    }
}
