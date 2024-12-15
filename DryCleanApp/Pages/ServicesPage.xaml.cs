using DryCleanApp.Data;
using System.Collections.Generic;

namespace DryCleanApp.Pages
{
    public partial class ServicesPage : ContentPage
    {
        private DbHelper _dbHelper;
        private List<Service> _services;

        public ServicesPage()
        {
            InitializeComponent();
            _dbHelper = new DbHelper();
            _services = _dbHelper.GetServices().ToList();
            ServicesListView.ItemsSource = _services;
        }

        private async void OnServiceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Service service)
            {
                // Показываем диалог для выбора количества
                var quantity = await DisplayPromptAsync("Количество", "Введите количество:", initialValue: "1", keyboard: Keyboard.Numeric);
                if (int.TryParse(quantity, out int quantityInt) && quantityInt > 0)
                {
                    // Добавляем услугу в корзину
                    var order = new Order
                    {
                        ServiceID = service.ID,
                        Quantity = quantityInt,
                        TotalPrice = service.Price * quantityInt,
                        Status = "Pending"
                    };
                    _dbHelper.AddOrder(order);
                    DisplayAlert("Успешно", $"Услуга '{service.Name}' добавлена в корзину.", "OK");
                }
            }
        }
    }
}