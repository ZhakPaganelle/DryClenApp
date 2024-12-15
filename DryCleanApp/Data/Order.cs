using SQLite;

namespace DryCleanApp.Data
{
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Первичный ключ
        public int ServiceID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // "Pending", "Completed", "Cancelled"

        // Дополнительное свойство для отображения названия услуги
        public string ServiceName { get; set; }
    }
}