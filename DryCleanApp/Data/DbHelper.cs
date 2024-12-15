using SQLite;
using System.Collections.Generic;
using System.IO;

namespace DryCleanApp.Data
{
    public class DbHelper
    {
        private SQLiteConnection _db;

        public DbHelper()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "dryclean.db");
            bool exists = File.Exists(dbPath);

            _db = new SQLiteConnection(dbPath);

            if (!exists)
            {
                // Создаем таблицы, если база данных не существует
                _db.CreateTable<Service>();
                _db.CreateTable<Order>();

                // Добавляем примерные данные
                _db.Insert(new Service { Name = "Химчистка куртки", Price = 1500 });
                _db.Insert(new Service { Name = "Чистка обуви", Price = 800 });
                _db.Insert(new Service { Name = "Чистка ковров", Price = 2000 });
            }
        }

        // Services CRUD operations
        public IEnumerable<Service> GetServices()
        {
            return _db.Table<Service>();
        }

        public void AddService(Service service)
        {
            _db.Insert(service);
        }

        // Orders CRUD operations
        public IEnumerable<Order> GetOrders()
        {
            return _db.Table<Order>();
        }

        public void AddOrder(Order order)
        {
            _db.Insert(order);
        }

        public void UpdateOrder(Order order)
        {
            _db.Update(order);
        }

        public void DeleteOrder(int id)
        {
            _db.Delete<Order>(id);
        }

        // Метод для создания одного общего заказа
        public int CreateCombinedOrder(List<Order> orders)
        {
            // Создаем новый заказ с уникальным ID
            var newOrderId = _db.Table<Order>().OrderByDescending(o => o.ID).FirstOrDefault()?.ID + 1 ?? 1;

            foreach (var order in orders)
            {
                order.ID = newOrderId; // Устанавливаем одинаковый ID для всех позиций
                _db.Insert(order);
            }

            return newOrderId;
        }
    }
}