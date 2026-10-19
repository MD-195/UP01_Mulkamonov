using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) || string.IsNullOrWhiteSpace(TxtOldPass.Password) ||
                string.IsNullOrWhiteSpace(TxtNewPass.Password) || string.IsNullOrWhiteSpace(TxtConfirmPass.Password))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            if (TxtNewPass.Password.Length < 6)
            {
                MessageBox.Show("Новый пароль должен содержать минимум 6 символов!");
                return;
            }

            if (TxtNewPass.Password != TxtConfirmPass.Password)
            {
                MessageBox.Show("Новые пароли не совпадают!");
                return;
            }

            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                string oldHash = AuthPage.GetHash(TxtOldPass.Password);

                var user = db.User.FirstOrDefault(u => u.Login == TxtLogin.Text && u.PasswordHash == oldHash);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или старый пароль!");
                    return;
                }

                user.PasswordHash = AuthPage.GetHash(TxtNewPass.Password);
                db.SaveChanges(); 

                MessageBox.Show("Пароль успешно изменен!");
                NavigationService.Navigate(new AuthPage()); 
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage()); 
        }
    }
}