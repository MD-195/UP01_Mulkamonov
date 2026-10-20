using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ISIP523_Mulkamonov
{
    public partial class AddRealEstatePage : Page
    {
        private RealEstate _currentRealEstate = new RealEstate();

        public AddRealEstatePage(RealEstate selectedRealEstate)
        {
            InitializeComponent();

            if (selectedRealEstate != null)
            {
                _currentRealEstate = selectedRealEstate;
                Title = "Редактирование объекта";
            }

            DataContext = _currentRealEstate;

            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                ComboCategory.ItemsSource = db.Category.ToList();
                ComboCategory.DisplayMemberPath = "Name";

                ComboOwner.ItemsSource = db.User.Where(u => u.Role.Name == "User").ToList();
                ComboOwner.DisplayMemberPath = "FIO";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверки на пустоту полей
            if (string.IsNullOrWhiteSpace(_currentRealEstate.Title)) errors.AppendLine("Укажите название!");
            if (_currentRealEstate.CategoryID == 0) errors.AppendLine("Выберите категорию!");
            if (_currentRealEstate.OwnerID == 0) errors.AppendLine("Выберите владельца!");
            if (string.IsNullOrWhiteSpace(_currentRealEstate.Address)) errors.AppendLine("Укажите адрес!");
            if (_currentRealEstate.Area <= 0) errors.AppendLine("Укажите корректную площадь!");
            if (_currentRealEstate.PricePerMonth <= 0) errors.AppendLine("Укажите цену больше нуля!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                if (_currentRealEstate.ID == 0)
                {
                    _currentRealEstate.IsActive = true;
                    _currentRealEstate.CreatedAt = DateTime.Now;
                    db.RealEstate.Add(_currentRealEstate);
                }
                else
                {
                    db.Entry(_currentRealEstate).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(selectedPhotoPath))
                    {
                        var photo = new RealEstatePhoto
                        {
                            RealEstateID = _currentRealEstate.ID,
                            PhotoPath = selectedPhotoPath,
                            IsMain = true
                        };
                        db.RealEstatePhoto.Add(photo);

                        db.SaveChanges();
                    }

                    MessageBox.Show("Данные успешно сохранены!");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }
        private string selectedPhotoPath = "";

        private void BtnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedPhotoPath = openFileDialog.FileName;
                LblPhotoPath.Text = "Фото выбрано";
            }
        }

    
    }
}