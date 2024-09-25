using HealthCare.Views;

namespace HealthCare.Components
{
    public partial class Navbar : ContentView
    {
        public Navbar()
        {
            InitializeComponent();
        }

        private async void WelcomePage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WelcomePage());
        }
        private async void ServicePage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ServicesPage());
        }

    }
}