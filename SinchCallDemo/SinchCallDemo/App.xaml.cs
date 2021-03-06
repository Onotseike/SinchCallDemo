﻿using SinchCallDemo.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SinchCallDemo
{
	public partial class App : Application
	{
        public App()
		{
			InitializeComponent();

			SetPage();
		}

		public static void SetMainPage()
		{
            //Current.MainPage = new TabbedPage
            //{
            //    Children =
            //    {
            //        new NavigationPage(new ItemsPage())
            //        {
            //            Title = "Browse",
            //            Icon = Device.OnPlatform<string>("tab_feed.png",null,null)
            //        },
            //        new NavigationPage(new AboutPage())
            //        {
            //            Title = "About",
            //            Icon = Device.OnPlatform<string>("tab_about.png",null,null)
            //        },
            //    }
            //};
        }

        public static void SetPage()
        {
            Current.MainPage = new ReciepientPage();
        }
	}
}
