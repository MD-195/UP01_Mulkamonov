using ISIP523_Mulkamonov;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                DGridRealEstate.ItemsSource = db.RealEstate.Include("Category").Include("User").ToList();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = DGridRealEstate.SelectedItems.Cast<RealEstate>().ToList();
            if (selectedItems.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить {selectedItems.Count} объектов? Все их фотографии и договоры тоже удалятся (каскадно)!", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Mulkamonov_LeaseEntities())
                {
                    foreach (var item in selectedItems)
                    {
                        var objToRemove = db.RealEstate.Find(item.ID);
                        db.RealEstate.Remove(objToRemove);
                    }
                    db.SaveChanges();
                }
                LoadData(); 
                MessageBox.Show("Данные удалены!");
            }
        }
    }
}