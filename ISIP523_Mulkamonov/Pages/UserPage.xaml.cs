using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ISIP523_Mulkamonov
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            LoadCategories();
            UpdateData();
        }

        private void LoadCategories()
        {
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                var categories = db.Category.ToList();
                categories.Insert(0, new Category { Name = "Все категории" });
                ComboCategory.ItemsSource = categories;
                ComboCategory.DisplayMemberPath = "Name";
                ComboCategory.SelectedIndex = 0;
            }
        }

        private void UpdateData()
        {
            if (!IsInitialized) return;

            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                var currentData = db.RealEstate.Include("Category").Where(r => r.IsActive == true).ToList();

                if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
                {
                    currentData = currentData.Where(r => r.Title.ToLower().Contains(TxtSearch.Text.ToLower())).ToList();
                }

                if (ComboCategory.SelectedIndex > 0)
                {
                    var selectedCategory = ComboCategory.SelectedItem as Category;
                    if (selectedCategory != null)
                    {
                        currentData = currentData.Where(r => r.CategoryID == selectedCategory.ID).ToList();
                    }
                }

                if (ComboSort.SelectedIndex == 1)
                {
                    currentData = currentData.OrderBy(r => r.PricePerMonth).ToList();
                }
                else if (ComboSort.SelectedIndex == 2)
                {
                    currentData = currentData.OrderByDescending(r => r.PricePerMonth).ToList();
                }

                ListRealEstate.ItemsSource = currentData;
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }
    }
}