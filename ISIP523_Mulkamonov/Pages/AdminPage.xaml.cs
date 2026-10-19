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
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                using (var db = new DB_Mulkamonov_LeaseEntities())
                {
                    DGridRealEstate.ItemsSource = db.RealEstate.Include("Category").Include("User").ToList();
                }
            }
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

            if (MessageBox.Show($"Вы точно хотите удалить {selectedItems.Count} объектов? Все их фотографии и договоры тоже удалятся!", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DGridRealEstate.SelectedItem as RealEstate;

            if (selectedItem == null)
            {
                MessageBox.Show("Сначала выберите объект в таблице для редактирования!");
                return;
            }

            NavigationService.Navigate(new AddRealEstatePage(selectedItem));
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRealEstatePage(null));
        }
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }
    }

}