
using SinchCallDemo.Interface;
using SinchCallDemo.InterfaceImplements;
using SinchCallDemo.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace SinchCallDemo.Views
{
	public partial class ItemDetailPage : ContentPage
	{
		ItemDetailViewModel viewModel;
        SinchService sinchService;
        string tt;

        // Note - The Xamarin.Forms Previewer requires a default, parameterless constructor to render a page.
        public ItemDetailPage()
        {
            InitializeComponent();
            sinchService = new SinchService();
            

            
        }

        public ItemDetailPage(ItemDetailViewModel viewModel)
		{
			InitializeComponent();
            sinchService = new SinchService();
            BindingContext = this.viewModel = viewModel;
		}


        void ClickedButton(object sender, EventArgs args)
        {
            tt = enterName.Text;
            sinchService.StartSinchClient(tt);
        }

        void OnButtonClicked(object sender, EventArgs args)
        {
            if (sinchService.IsSinchClientStarted())
            {
                Debug.WriteLine("SinchClient started");
                Label label = new Label
                {
                    Text = "Sinch Client Started",
                    FontSize = 12
                };
                stack.Children.Add(label);
                
            }

            sinchService.CallVoice("xmodedevs@gmail.com");
        }
    }
}
