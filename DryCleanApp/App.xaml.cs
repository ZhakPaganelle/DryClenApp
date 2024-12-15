using DryCleanApp.Pages;
using DryCleanApp.Data;

namespace DryCleanApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Инициализация базы данных
            var dbHelper = new DbHelper();

            // Устанавливаем стартовую страницу
            MainPage = new NavigationPage(new MainPage());
        }
    }
}