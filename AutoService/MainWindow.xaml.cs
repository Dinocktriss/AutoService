using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoService
{
    public partial class MainWindow : Window
    {
        private bool isAdminMode = false;
        private List<Service> services;

        public MainWindow()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            using (var context = new AutoServiceContext())
            {
                services = context.Services.ToList();
                if (services == null)
                {
                    services = new List<Service>();
                }

                LvServices.ItemsSource = services;
                UpdateStatistics();
            }
        }


        private void UpdateServices()
        {
            if (services == null)
            {
                services = new List<Service>();
            }

            if (TxtSearch == null)
            {
                TxtSearch = new TextBox();
            }

            var filteredServices = services.Where(s =>
                (s.Title != null && s.Title.Contains(TxtSearch.Text)) ||
                (s.Description != null && s.Description.Contains(TxtSearch.Text))
            );

            if (CmbDiscountFilter != null)
            {
                switch (CmbDiscountFilter.SelectedIndex)
                {
                    case 1:
                        filteredServices = filteredServices.Where(s => s.Discount >= 0 && s.Discount < 0.05);
                        break;
                    case 2:
                        filteredServices = filteredServices.Where(s => s.Discount >= 0.05 && s.Discount < 0.15);
                        break;
                    case 3:
                        filteredServices = filteredServices.Where(s => s.Discount >= 0.15 && s.Discount < 0.30);
                        break;
                    case 4:
                        filteredServices = filteredServices.Where(s => s.Discount >= 0.30 && s.Discount < 0.70);
                        break;
                    case 5:
                        filteredServices = filteredServices.Where(s => s.Discount >= 0.70 && s.Discount <= 1.0);
                        break;
                }
            }

            if (CmbSort != null && CmbSort.SelectedIndex == 0)
            {
                filteredServices = filteredServices.OrderBy(s => s.Cost);
            }
            else
            {
                filteredServices = filteredServices.OrderByDescending(s => s.Cost);
            }

            if (LvServices != null)
            {
                LvServices.ItemsSource = filteredServices.ToList();
                UpdateStatistics();
            }
        }

        private void UpdateStatistics()
        {
            TxtCount.Text = $"{LvServices.Items.Count} из {services.Count}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void CmbDiscountFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (isAdminMode)
            {
                isAdminMode = false;
                BtnAdmin.Content = "Режим администратора";
            }
            else
            {
                var authWindow = new AuthWindow();
                authWindow.ShowDialog();
                if (authWindow.IsAuthorized) // Проверка флага авторизации
                {
                    isAdminMode = true;
                    BtnAdmin.Content = "Выход из режима администратора";
                }
            }
        }

        private void LvServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isAdminMode && LvServices.SelectedItem != null)
            {
                var service = LvServices.SelectedItem as Service;
                var editWindow = new ServiceEditWindow(service);
                editWindow.ShowDialog();
                LoadServices();
            }
        }

        // Обработчики событий для элементов меню отладки
        private void OpenAuthWindow_Click(object sender, RoutedEventArgs e)
        {
            var authWindow = new AuthWindow();
            authWindow.ShowDialog();
        }

        private void OpenServiceEditWindow_Click(object sender, RoutedEventArgs e)
        {
            var service = new Service();
            var editWindow = new ServiceEditWindow(service);
            editWindow.ShowDialog();
        }

        private void OpenAppointmentWindow_Click(object sender, RoutedEventArgs e)
        {
            var service = new Service(); // Замените на нужную услугу для тестирования
            var appointmentWindow = new AppointmentWindow(service);
            appointmentWindow.ShowDialog();
        }

        private void OpenUpcomingAppointmentsWindow_Click(object sender, RoutedEventArgs e)
        {
            var upcomingAppointmentsWindow = new UpcomingAppointmentsWindow();
            upcomingAppointmentsWindow.ShowDialog();
        }
    }
}
