using Newtonsoft.Json;
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
    public partial class AllReviews : ContentPage
    {
        public AllReviews()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ShowReviews();

        }
        public async void ShowReviews()
        {
            int a = 0;
            RestaurantPage.toReview.Clear();
            var content_reviews = Registration.PostCreateFoundation("reviews").GetAwaiter().GetResult();
            if (content_reviews != null &&
            Regex.Match(content_reviews, "Connecting error").Success == false)
            {
                if (Regex.Match(content_reviews, "Review does not exist ").Success == true)
                {
                    await DisplayAlert("Reviews", "No reviews here", "OK");
                }
                else
                {
                    PageOfRestaurants.reviews.Clear();
                    PageOfRestaurants.reviews = JsonConvert.DeserializeObject<List<Review>>(content_reviews);
                    a++;
                }
            }
            else
            {
                await DisplayAlert("Getting review data","Connection error, please try again later", "OK");
            }
            if(a != 0)
            {
                var usersPhotos = Registration.PostCreateFoundation("usersPhoto").GetAwaiter().GetResult();
                if (usersPhotos != null &&
                Regex.Match(usersPhotos, "Connecting error").Success == false &&
                Regex.Match(usersPhotos, "user does not exist").Success == false)
                {
                    RestaurantPage.usersPhotos1.Clear();
                    RestaurantPage.usersPhotos1 = JsonConvert.DeserializeObject<List<RestaurantPage.UsersPhotos>>(usersPhotos);
                }
            }
            Image image = new Image();
            foreach (Review review1 in PageOfRestaurants.reviews)
            {
                if (PageOfRestaurants.restaurantmain.Id == review1.RestaurantId)
                {
                    RestaurantPage.FullReview full = new RestaurantPage.FullReview();
                    full.Id = review1.Id;
                    foreach (RestaurantPage.UsersPhotos usersPhoto in RestaurantPage.usersPhotos1)
                    {
                        if (usersPhoto.Id == review1.UserId)
                        {
                            full.Name = usersPhoto.Name;
                            if (usersPhoto.Photo != null)
                            {
                                PageOfRestaurants.ToImgFromArr(usersPhoto.Photo, image);
                                full.Photo = image.Source;
                            }
                            else
                            {
                                full.Photo = ImageSource.FromFile("username.png");
                            }
                        }
                    }
                    full.Header = review1.Header;
                    full.Text = review1.Text;
                    full.Rating = review1.Rating;
                    full.Date = review1.Date;
                    RestaurantPage.toReview.Add(full);
                }
            }
            RestaurantsReviewCollection.ItemsSource = RestaurantPage.toReview;
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
        public async void ReviewCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                if(MainPage.user.Role == "admin")
                {
                    bool result = await DisplayAlert("Reviews control", "Do you want to delete user or review?", "User", "Review");
                    if (result == true)
                    {
                        bool userresult = await DisplayAlert("Delete user", "Do you really want to delete user?", "Yes", "No");
                        if (userresult == true)
                        {
                            RestaurantPage.FullReview fullReview = e.CurrentSelection[0] as RestaurantPage.FullReview;
                            int userId = 0;
                            foreach (Review review in PageOfRestaurants.reviews) 
                            {
                                if(review.Id == fullReview.Id)
                                {
                                    userId = review.UserId;
                                }                                
                            }
                            if (userId != 0)
                            {
                                if (userId != MainPage.user.Id)
                                {
                                    var deleteUser = Registration.PostCreateFoundation("deleteUser/?userID=" + userId).GetAwaiter().GetResult();
                                    if (deleteUser != null &&
                                    Regex.Match(deleteUser, "Connecting error").Success == false &&
                                    Regex.Match(deleteUser, "User deleted").Success == true)
                                    {
                                        await DisplayAlert("User", "User has been deleted", "OK");
                                        ShowReviews();
                                        RestaurantsReviewCollection.SelectedItem = null;
                                    }
                                    else
                                    {
                                        await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                                        RestaurantsReviewCollection.SelectedItem = null;
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Error", "You can't delete yourself", "OK");
                                    RestaurantsReviewCollection.SelectedItem = null;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Error", "UserId is not found", "OK");
                                RestaurantsReviewCollection.SelectedItem = null;
                            }
                        }
                    }
                    else
                    {
                        bool reviewresult = await DisplayAlert("Delete review", "Do you really want to delete review?", "Yes", "No");
                        if (reviewresult == true)
                        {
                            RestaurantPage.FullReview fullReview = e.CurrentSelection[0] as RestaurantPage.FullReview;
                            var deleteRest = Registration.PostCreateFoundation("deleteReview/?reviewID=" + fullReview.Id).GetAwaiter().GetResult();
                            if (deleteRest != null &&
                            Regex.Match(deleteRest, "Connecting error").Success == false &&
                            Regex.Match(deleteRest, "Review deleted").Success == true)
                            {
                                await DisplayAlert("Review", "Review has been deleted", "OK");
                                ShowReviews();
                                RestaurantsReviewCollection.SelectedItem = null;
                            }
                            else
                            {
                                await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                                RestaurantsReviewCollection.SelectedItem = null;
                            }
                        }
                    }
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
    }
}