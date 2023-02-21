using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPasswordPage : ContentPage
    {
        public NewPasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void Recover_Button(object sender, System.EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                int a = 0;
                NewPassword_label.TextColor = Color.Transparent;
                if (NewPassword_textInput.Text != null)
                {
                    if (Registration.IsValidPassword(NewPassword_textInput.Text) == false)
                    {
                        NewPassword_label.TextColor = Color.Red;
                        NewPassword_label.Text = "Password must contains at least 1 alphabet character, 1 number, 1 symbol";
                        a = 1;
                    }
                    if (NewPassword_textInput.Text.Length <= 7)
                    {
                        NewPassword_label.TextColor = Color.Red;
                        NewPassword_label.Text = "Password must be longer than 7 characters";
                        a = 1;
                    }
                    if (NewPassword_textInput.Text == MainPage.user.Password)
                    {
                        NewPassword_label.TextColor = Color.Red;
                        NewPassword_label.Text = "The password must be different from the old one";
                        a = 1;
                    }
                    if (a == 0)
                    {
                        var content = Registration.PostCreateFoundation("passrecovery/passchange/?userID=" + MainPage.user.Id 
                            + "&newPass=" + NewPassword_textInput.Text.Trim().ToLowerInvariant() + "&oldPass=" + MainPage.user.Password).GetAwaiter().GetResult();
                        if (content != null &&
                        Regex.Match(content, "Connecting error").Success == false &&
                        Regex.Match(content, "No internet connection").Success == false &&
                        Regex.Match(content, "Error: ").Success == false &&
                        Regex.Match(content, "Password changed").Success == true)
                        {
                            MainPage.user.Password = NewPassword_textInput.Text.Trim().ToLowerInvariant();
                            await Navigation.PushModalAsync(new PageOfRestaurants());
                            App.Current.MainPage = new NavigationPage(new PageOfRestaurants());
                        }
                        else
                        {
                            await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                        }
                    }
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
    }
}