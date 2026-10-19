using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class RentalsPage : Page
    {
        public RentalsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                DGridRentals.ItemsSource = db.RentalAgreement.Include("RealEstate").Include("User").ToList();
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DGridRentals.SelectedItem as RentalAgreement;

            if (selectedItem == null)
            {
                MessageBox.Show("Сначала выберите договор в таблице!");
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите завершить эту аренду и вернуть объект в каталог?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Mulkamonov_LeaseEntities())
                {
                    var rental = db.RentalAgreement.Find(selectedItem.ID);
                    rental.Status = "Завершен";

                    var property = db.RealEstate.Find(rental.RealEstateID);
                    if (property != null)
                    {
                        property.IsActive = true;
                    }

                    db.SaveChanges(); 
                    LoadData();      
                    MessageBox.Show("Аренда успешно завершена, объект снова доступен для пользователей!");
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = DGridRentals.SelectedItems.Cast<RentalAgreement>().ToList();
            if (selectedItems.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить {selectedItems.Count} записей об аренде?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Mulkamonov_LeaseEntities())
                {
                    foreach (var item in selectedItems)
                    {
                        var objToRemove = db.RentalAgreement.Find(item.ID);
                        db.RentalAgreement.Remove(objToRemove);
                    }
                    db.SaveChanges();
                }
                LoadData();
                MessageBox.Show("Данные удалены!");
            }
        }
    }
}