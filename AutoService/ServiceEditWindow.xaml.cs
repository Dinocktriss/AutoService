using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutoService
{
    public partial class ServiceEditWindow : Window
    {
        private Service service;
        private string mainImagePath;

        public ServiceEditWindow(Service service)
        {
            InitializeComponent();
            this.service = service;

            if (service != null)
            {
                TxtTitle.Text = service.Title;
                TxtCost.Text = service.Cost.ToString();
                TxtDuration.Text = service.DurationInMinutes.ToString();
                TxtDescription.Text = service.Description;
                TxtDiscount.Text = (service.Discount * 100).ToString();
                if (!string.IsNullOrEmpty(service.MainImage))
                {
                    ImgMain.Source = new BitmapImage(new Uri(service.MainImage, UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                mainImagePath = dlg.FileName;
                ImgMain.Source = new BitmapImage(new Uri(mainImagePath));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Название услуги не может быть пустым.");
                return;
            }

            decimal cost;
            if (!decimal.TryParse(TxtCost.Text, out cost) || cost < 0)
            {
                MessageBox.Show("Некорректная стоимость.");
                return;
            }

            int duration;
            if (!int.TryParse(TxtDuration.Text, out duration) || duration <= 0 || duration > 240)
            {
                MessageBox.Show("Длительность должна быть от 1 до 240 минут.");
                return;
            }

            float discount;
            if (!float.TryParse(TxtDiscount.Text, out discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть от 0% до 100%.");
                return;
            }

            using (var context = new AutoServiceContext())
            {
                if (service == null)
                {
                    service = new Service();
                    context.Services.Add(service);
                }
                else
                {
                    var existingService = context.Services.FirstOrDefault(s => s.ServiceID == service.ServiceID);
                    if (existingService != null)
                    {
                        service = existingService;
                    }
                    else
                    {
                        MessageBox.Show("Услуга не найдена.");
                        return;
                    }
                }

                service.Title = TxtTitle.Text;
                service.Cost = cost;
                service.DurationInMinutes = duration;
                service.Description = TxtDescription.Text;
                service.Discount = discount / 100;

                if (!string.IsNullOrEmpty(mainImagePath))
                {
                    string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    string fileName = Path.GetFileName(mainImagePath);
                    string destPath = Path.Combine(imagesFolder, fileName);
                    File.Copy(mainImagePath, destPath, true);
                    service.MainImage = destPath;
                }

                try
                {
                    context.SaveChanges();
                    DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
