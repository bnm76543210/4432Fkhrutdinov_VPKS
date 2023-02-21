using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Restaurant_rating_Mobile
{
    public partial class MainPage : ContentPage
    {
        public static string HostLink = "http://restaurantsrating-com.f107642.hostru12.fornex.host/";
        public static bool _canTap = true;
        public static Users user = new Users();

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Register_Button(object sender, System.EventArgs e)
        {
            if (_canTap)
            {
                _canTap = false;
                await Navigation.PushAsync(new Registration());
                await Task.Delay(200);
                _canTap = true;
            }
        }

        private async void Auth_Button(object sender, System.EventArgs e)
        {
            if (_canTap)
            {
                _canTap = false;
                UsernameMain_label.TextColor = Color.Transparent;
                PasswordMain_label.TextColor = Color.Transparent;
                if (User_textInput.Text != null && Password_textInput.Text != null)
                {
                    var content = Registration.PostCreateFoundation("auth/?userdata=" + User_textInput.Text
                       + "& password=" + Password_textInput.Text).GetAwaiter().GetResult();
                    if (content != null &&
                    Regex.Match(content, "Connecting error").Success == false &&
                    Regex.Match(content, "No internet connection").Success == false)
                    {
                        if (Regex.Match(content, "user does not exist").Success == true)
                        {
                            UsernameMain_label.TextColor = Color.Red;
                            UsernameMain_label.Text = "User does not exist";
                        }
                        if (Regex.Match(content, "wrong password").Success == true)
                        {
                            PasswordMain_label.TextColor = Color.Red;
                            PasswordMain_label.Text = "Wrong password";
                        }
                        if (Regex.Match(content, "userdata exist").Success == true)
                        {
                            var jsonSTR = content.Substring(content.IndexOf('%') + 1);
                            user = JsonConvert.DeserializeObject<Users>(jsonSTR);
                            await Navigation.PushModalAsync(new PageOfRestaurants());
                            App.Current.MainPage = new NavigationPage(new PageOfRestaurants());
                        }
                    }
                    else
                    {
                        await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                    }
                }
                await Task.Delay(200);
                _canTap = true;
            }
        }
        private async void Button_passwordRecovery(object sender, System.EventArgs e)
        {
            if (_canTap)
            {
                _canTap = false;
                await Navigation.PushAsync(new PasswordRecovery());
                await Task.Delay(200);
                _canTap = true;
            }
        }

        private async void Skip_Button(object sender, EventArgs e)
        {
            if (_canTap)
            {
                _canTap = false;
                await Navigation.PushModalAsync(new PageOfRestaurants());
                App.Current.MainPage = new NavigationPage(new PageOfRestaurants());
                await Task.Delay(200);
                _canTap = true;
            }
        }
        public void Alert_OS()
        {
            DisplayAlert("Тип ОС", App.Device_OS, "ОK");
        }
    }
}
