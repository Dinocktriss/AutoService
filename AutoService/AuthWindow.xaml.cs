using System.Windows;

namespace AutoService
{
    public partial class AuthWindow : Window
    {
        public bool IsAuthorized { get; private set; } = false;

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PwdCode.Password == "0000")
            {
                IsAuthorized = true; // Установка флага авторизации
                this.Close(); // Закрытие окна
            }
            else
            {
                MessageBox.Show("Неверный код доступа!");
            }
        }
    }
}
