using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0; 

        public AuthPage() => InitializeComponent();

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string hash = GetHash(TxtPassword.Password); 
            
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                var user = db.User.FirstOrDefault(u => u.Login == TxtLogin.Text && u.PasswordHash == hash);

                if (user != null)
                {
                    failedAttempts = 0;

                    if (user.Role.Name == "Admin")
                    {
                        NavigationService.Navigate(new AdminPage()); 
                    }
                    else
                    {
                        MessageBox.Show("Вы успешно вошли как пользователь!");
                        NavigationService.Navigate(new UserPage()); 
                    }
                }
                else
                {
                    failedAttempts++;
                    MessageBox.Show("Неверные данные!");
                    if (failedAttempts >= 3) ShowCaptcha(); 
                }
            }
        }

        private void ShowCaptcha()
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            CaptchaPanel.Visibility = Visibility.Visible;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            LblCaptcha.Text = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray()); 
        }

        private void BtnCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCaptchaInput.Text == LblCaptcha.Text)
            {
                failedAttempts = 0;
                CaptchaPanel.Visibility = Visibility.Collapsed;
                LoginPanel.Visibility = Visibility.Visible; 
            }
            else ShowCaptcha();
        }
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }
    }

}