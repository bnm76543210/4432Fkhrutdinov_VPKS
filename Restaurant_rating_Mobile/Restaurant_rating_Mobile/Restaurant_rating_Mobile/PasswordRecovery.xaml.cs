using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordRecovery : ContentPage
    {
        public PasswordRecovery()
        {
            InitializeComponent();
        }
        private async void Find_Account_Button(object sender, System.EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                SearchUser_label.TextColor = Color.Transparent;
                if (SearchUser_textInput.Text != null) {
                    var content = Registration.PostCreateFoundation("passrecovery/?finduser=" + SearchUser_textInput.Text).GetAwaiter().GetResult();
                    if (content != null &&
                    Regex.Match(content, "Connecting error").Success == false &&
                    Regex.Match(content, "No internet connection").Success == false)
                    {
                        if (Regex.Match(content, "user does not exist").Success == true)
                        {
                            SearchUser_label.TextColor = Color.Red;
                            SearchUser_label.Text = "user does not exist";
                        }
                        if (Regex.Match(content, "user exist").Success == true)
                        {
                            var jsonSTR = content.Substring(content.IndexOf('%') + 1);
                            MainPage.user = JsonConvert.DeserializeObject<Users>(jsonSTR);
                            EmailConfirmation.IsPassRecovery = true;
                            try
                            {
                                Registration.Send_to_email(MainPage.user.Email);
                                await Navigation.PushAsync(new EmailConfirmation());
                                MainPage._canTap = true;
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Mail sending", ex.Message, "OK");
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                    }
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
    }
}