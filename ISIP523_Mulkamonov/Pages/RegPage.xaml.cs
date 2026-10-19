using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                ComboRole.ItemsSource = db.Role.ToList();
                ComboRole.SelectedValue = 2;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLogin.Text) || string.IsNullOrWhiteSpace(TxtFIO.Text) ||
                string.IsNullOrWhiteSpace(TxtPassword.Password) || string.IsNullOrWhiteSpace(TxtConfirmPassword.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (TxtPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!");
                return;
            }

            if (TxtPassword.Password != TxtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                if (db.User.Any(u => u.Login == TxtLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                User newUser = new User
                {
                    Login = TxtLogin.Text,
                    FIO = TxtFIO.Text,
                    PasswordHash = AuthPage.GetHash(TxtPassword.Password),
                    RoleID = (int)ComboRole.SelectedValue,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                db.User.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Пользователь успешно зарегистрирован!");
                NavigationService.GoBack(); 
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}