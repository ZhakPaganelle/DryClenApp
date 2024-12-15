using DryCleanApp.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DryCleanApp.Pages
{
    public partial class OrdersPage : ContentPage
    {
        private DbHelper _dbHelper;
        private List<Order> _orders;

        public OrdersPage()
        {
            InitializeComponent();
            _dbHelper = new DbHelper();
            RefreshOrders();
        }

        private void RefreshOrders()
        {
            _orders = _dbHelper.GetOrders().Where(o => o.Status == "Completed").ToList();

            // Очищаем предыдущие карточки
            OrdersStackLayout.Children.Clear();

            // Группируем заказы по их идентификатору
            var groupedOrders = _orders
                .GroupBy(o => o.ID)
                .Select(g => new GroupedOrders
                {
                    Key = g.Key,
                    TotalPrice = g.Sum(o => o.TotalPrice),
                    Orders = g.ToList() // Добавляем список заказов в группу
                })
                .ToList();

            // Получаем названия услуг из базы данных
            foreach (var group in groupedOrders)
            {
                foreach (var order in group.Orders)
                {
                    var service = _dbHelper.GetServices().FirstOrDefault(s => s.ID == order.ServiceID);
                    if (service != null)
                    {
                        order.ServiceName = service.Name;
                        order.TotalPrice = service.Price * order.Quantity;
                    }
                }

                // Создаем карточку для каждого заказа
                var orderCard = CreateOrderCard(group);
                OrdersStackLayout.Children.Add(orderCard);
            }

            // Обновляем общую сумму всех заказов в рублях
            TotalOrdersPriceLabel.Text = groupedOrders.Sum(g => g.TotalPrice).ToString("C", new CultureInfo("ru-RU"));
        }

        private StackLayout CreateOrderCard(GroupedOrders group)
        {
            var card = new StackLayout
            {
                Padding = 10,
                Margin = new Thickness(0, 0, 0, 10),
                BackgroundColor = Color.FromHex("#f0f0f0"),
                Spacing = 5
            };

            // Заголовок заказа
            card.Children.Add(new Label
            {
                Text = $"Заказ №{group.Key}",
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            });

            // Детали каждого товара в заказе
            foreach (var order in group.Orders)
            {
                card.Children.Add(new Label
                {
                    Text = $"{order.ServiceName} (Количество: {order.Quantity})",
                    FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label))
                });
                card.Children.Add(new Label
                {
                    Text = $"Стоимость: {order.TotalPrice.ToString("C", new CultureInfo("ru-RU"))}",
                    FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label))
                });
            }

            // Общая стоимость заказа
            card.Children.Add(new Label
            {
                Text = $"Общая стоимость заказа: {group.TotalPrice.ToString("C", new CultureInfo("ru-RU"))}",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.End
            });

            return card;
        }
    }

    // Вспомогательный класс для группировки заказов
    public class GroupedOrders
    {
        public int Key { get; set; } // Идентификатор заказа
        public decimal TotalPrice { get; set; } // Общая стоимость заказа
        public List<Order> Orders { get; set; } // Список заказов в группе
    }
}