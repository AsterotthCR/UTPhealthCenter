﻿using Microsoft.Maui.Controls;
namespace HealthCare
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.WelcomePage());
        }
    }
}
