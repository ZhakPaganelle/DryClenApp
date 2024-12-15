using DryCleanApp.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DryCleanApp.Pages
{
    public partial class CartPage : ContentPage
    {
        private DbHelper _dbHelper;
        private List<Order> _cartItems;

        public CartPage()
        {
            InitializeComponent();
            _dbHelper = new DbHelper();
            RefreshCart();
        }

        private void RefreshCart()
        {
            _cartItems = _dbHelper.GetOrders().Where(o => o.Status == "Pending").ToList();

            // Получаем названия услуг из базы данных
            foreach (var order in _cartItems)
            {
                var service = _dbHelper.GetServices().FirstOrDefault(s => s.ID == order.ServiceID);
                if (service != null)
                {
                    order.ServiceName = service.Name;
                    order.TotalPrice = service.Price * order.Quantity;
                }
            }

            CartListView.ItemsSource = _cartItems;

            // Обновляем суммарную стоимость в рублях
            TotalPriceLabel.Text = _cartItems.Sum(o => o.TotalPrice).ToString("C", new CultureInfo("ru-RU"));
        }

        private async void OnCheckoutClicked(object sender, EventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                await DisplayAlert("Ошибка", "Корзина пуста. Добавьте услуги перед оформлением заказа.", "OK");
                return;
            }

            // Создаем один общий заказ
            var newOrderId = _dbHelper.CreateCombinedOrder(_cartItems);

            // Обновляем статус всех позиций в корзине
            foreach (var order in _cartItems)
            {
                order.Status = "Completed";
                _dbHelper.UpdateOrder(order);
            }

            await DisplayAlert("Успешно", $"Заказ №{newOrderId} оформлен.", "OK");
            RefreshCart();
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Order order)
            {
                // Показываем диалог для изменения количества
                var quantity = await DisplayPromptAsync("Изменить количество", $"Текущее количество: {order.Quantity}", initialValue: order.Quantity.ToString(), keyboard: Keyboard.Numeric);
                if (int.TryParse(quantity, out int newQuantity) && newQuantity > 0)
                {
                    order.Quantity = newQuantity;
                    order.TotalPrice = _dbHelper.GetServices().First(s => s.ID == order.ServiceID).Price * newQuantity;
                    _dbHelper.UpdateOrder(order);
                    RefreshCart();
                }
            }
        }
    }
}