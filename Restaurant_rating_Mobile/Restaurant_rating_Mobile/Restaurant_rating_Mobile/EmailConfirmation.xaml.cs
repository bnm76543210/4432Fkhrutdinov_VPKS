using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmailConfirmation : ContentPage
    {
        public static bool IsPassRecovery = false;
        public EmailConfirmation()
        {
            InitializeComponent();
            BeginCounting();
            Resending_label.TextColor = Color.Gray;
            Resending_label.IsEnabled = false;
        }
        private async void Check_emailCode(object sender, System.EventArgs e)
        {
            if (EmailCode_textInput.Text == Registration.code.ToString())
            {
                if (IsPassRecovery == false)
                {
                    var content = Registration.PostCreateFoundation("reg/?name=" + MainPage.user.Name
                        + "& password=" + MainPage.user.Password
                        + "& email=" + MainPage.user.Email).GetAwaiter().GetResult();
                    if (content != null && 
                    Regex.Match(content, "Connecting error").Success == false &&
                    Regex.Match(content, "No internet connection").Success == false &&
                    Regex.Match(content, "User created").Success == true)
                    {
                        await Navigation.PushModalAsync(new MainPage());
                        App.Current.MainPage = new NavigationPage(new MainPage());
                    }
                    else
                    {
                        await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                    }
                }
                else
                {
                    await Navigation.PushModalAsync(new NewPasswordPage());
                    Navigation.RemovePage(this);
                }
            }
        }
        private async void Code_resending(object sender, System.EventArgs e)
        {
            Resending_label.IsEnabled = false;
            if (await App.CheckConnection() != "NetworkAccess error")
            {
                try
                {
                    Registration.Send_to_email(MainPage.user.Email);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Mail sending", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
            }
            Resending_label.TextColor = Color.Red;
            Resending_label.Text = "Code sent";
            await Task.Delay(4000);
            BeginCounting();
            Resending_label.TextColor = Color.Gray;
        }
        private async void BeginCounting()
        {
            for (int _second = 20; _second >= 0; _second--)
            {
                Resending_label.Text = "Code resending available in " + _second + " seconds";
                await Task.Delay(1000);
            }
            Resending_label.Text = "Code resending is available";
            Resending_label.TextColor = Color.Blue;
            Resending_label.IsEnabled = true;
        }
    }
}