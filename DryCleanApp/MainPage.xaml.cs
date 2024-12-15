using DryCleanApp.Pages;

namespace DryCleanApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnServicesClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ServicesPage());
        }

        private void OnCartClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CartPage());
        }

        private void OnOrdersClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OrdersPage());
        }
    }
}