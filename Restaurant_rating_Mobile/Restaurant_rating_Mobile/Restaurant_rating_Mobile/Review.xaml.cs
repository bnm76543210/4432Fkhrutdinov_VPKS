using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Review : ContentPage
	{
		public Review ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public async void Back_ToRestaurant(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false; 
                App.Current.MainPage = new NavigationPage(new RestaurantPage());
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public async void Add_review(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                int a = 0;
                string onlySumbols = "12345";
                if (Mark_textInput.Text != null && onlySumbols.Contains(Mark_textInput.Text)) 
                {
                    a++;
                }
                else
                {
                    await DisplayAlert("Incorrect mark", "Mark must be from", "OK");
                }
                if (Header_textInput.Text != null && Header_textInput.Text.Length > 5 &&
                    Text_textInput.Text != null && Text_textInput.Text.Length > 5)
                {
                    if (a > 0)
                    {
                        var createReview = Registration.PostCreateFoundation("sendreview/?header="+ Header_textInput.Text + "& text="+ Text_textInput.Text 
                            + "& rating=" + int.Parse(Mark_textInput.Text) + "& date=" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                        + "& userId=" + MainPage.user.Id + "& restId="+ PageOfRestaurants.restaurantmain.Id).GetAwaiter().GetResult();
                        if (createReview != null &&
                        Regex.Match(createReview, "Connecting error").Success == false &&
                        Regex.Match(createReview, "Review created").Success == true)
                        {
                            await DisplayAlert("Check connection", "Review added", "OK");
                            App.Current.MainPage = new NavigationPage(new RestaurantPage());
                        }
                        else
                        {
                            await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Fill all fields", "Your fields wasn't fill or their text less than 5 letters", "OK");
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
    }
}