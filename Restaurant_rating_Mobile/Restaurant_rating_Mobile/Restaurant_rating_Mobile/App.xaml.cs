using Plugin.Connectivity;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
    public partial class App : Application
    {
        public static string Device_OS;
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public App()
        {
            InitializeComponent();
            Check_OS();
            MainPage = new NavigationPage(new MainPage());
        }

        public static async Task<string> CheckConnection()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return "NetworkAccess error";
            }
            else
            {
                return "NetworkAccess success";
            }
        }
        public void Check_OS()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    Device_OS = "Android";
                    break;
                case Device.iOS:
                    Device_OS = "iOS";
                    break;
                case Device.UWP:
                    Device_OS = "UWP";
                    break;
                case Device.WPF:
                    Device_OS = "WPF";
                    break;
                case Device.GTK:
                    Device_OS = "GTK";
                    break;
                default:
                    Device_OS = "Undefined";
                    break;
            }
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {

        }
    }
}
