using Newtonsoft.Json;
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
    public partial class RestaurantPage : ContentPage
    {
        public class ToRestImgImport
        {
            public int Id { get; set; }
            public int RestaurantId { get; set; }
            public ImageSource Photo { get; set; }
        }
        public class ToRestMenuImport
        {
            public int Id { get; set; }
            public int RestaurantId { get; set; }
            public ImageSource Menu { get; set; }
        }
        public class UsersPhotos
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Photo { get; set; }
        }

        public class FullReview
        {
            public int Id { get; set; }
            public string Name { get; set; }         
            public string Header { get; set; }
            public string Text { get; set; }
            public int Rating { get; set; }
            public DateTime Date { get; set; }
            public ImageSource Photo { get; set; }
        }


        public List<ToRestImgImport> toRestImgImports = new List<ToRestImgImport>();
        public List<ToRestMenuImport> toRestMenuImports = new List<ToRestMenuImport>();
        public List<FullReview> toReview = new List<FullReview>();
        public List<Timeofwork> timeofworks = new List<Timeofwork>();
        public List<UsersPhotos> usersPhotos1 = new List<UsersPhotos>();

        public RestaurantPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            string r = PageOfRestaurants.LoadRestaurants().GetAwaiter().GetResult();
            if (r == "succsess")
            {
                LoadRestaurant();
            }
            else
            {
                DisplayAlert("Сообщение об ошибке", r, "OK");
            }
        }

        public async void LoadRestaurant()
        {
            Specialization.Text = "";
            RestaurantsPhotoCollection.ItemsSource = null;
            bool hasTimeTable = false;
            var content = Registration.PostCreateFoundation("gettimetable/?restID=" + PageOfRestaurants.restaurantmain.Id).GetAwaiter().GetResult();
            if (content != null &&
            Regex.Match(content, "Connecting error").Success == false)
            {
                if (Regex.Match(content, "restaurants don't have timetable").Success == false)
                {
                    hasTimeTable = true;
                    timeofworks = JsonConvert.DeserializeObject<List<Timeofwork>>(content);
                }
                else
                {
                    hasTimeTable = false;
                }
            }
            var usersPhotos = Registration.PostCreateFoundation("usersPhoto").GetAwaiter().GetResult();
            if (usersPhotos != null &&
            Regex.Match(usersPhotos, "Connecting error").Success == false &&
            Regex.Match(usersPhotos, "user does not exist").Success == false)
            {
                usersPhotos1 = JsonConvert.DeserializeObject<List<UsersPhotos>>(usersPhotos);
            }
            DateTime today = DateTime.Now;
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int checktime = 0;
            if (hasTimeTable == true) {
                foreach (Timeofwork timeofwork in timeofworks)
                {
                    if ((int)dayOfWeek == timeofwork.DayOfWeek)
                    {
                        TimeOfWork.Text = "Today from " + timeofwork.StartTime.ToString("hh\\:mm") + " to " + timeofwork.EndTime.ToString("hh\\:mm");
                        checktime = 1;
                    }
                }
                if(checktime == 0)
                {
                    TimeOfWork.Text = "Today is a day off";
                    IsOpen.Text = "Closed now";
                    IsOpen.TextColor = Color.Red;
                }
            }
            foreach (Typeofrestaurant typeofrestaurant in PageOfRestaurants.typeofrestaurants)
            {
                if (PageOfRestaurants.restaurantmain.Id == typeofrestaurant.RestaurantId)
                {
                    foreach (Restaurantstypes restaurantstypes1 in PageOfRestaurants.types)
                    {
                        if (typeofrestaurant.TypeId == restaurantstypes1.Id)
                        {
                            Specialization.Text += restaurantstypes1.TypeName + " ";
                        }
                    }
                }
            }

            float result = 0;
            int quantity = 0;
            foreach (Review review in PageOfRestaurants.reviews)
            {
                if (PageOfRestaurants.restaurantmain.Id == review.RestaurantId)
                {
                    result += review.Rating;
                    quantity++;
                }
            }
            if (quantity > 0)
            {
                Score.Text = Math.Round((result / quantity), 1, MidpointRounding.AwayFromZero).ToString();
                NumberOfReviews.Text = quantity + " reviws";
            }
            else
            {
                Score.Text = "No";
                NumberOfReviews.Text = quantity + " reviws";
            }
            foreach (Restaurant restaurant in PageOfRestaurants.restaurants)
            {
                if (PageOfRestaurants.restaurantmain.Id == restaurant.Id)
                {
                    RestaurantName.Text = restaurant.Name;
                    Address.Text = restaurant.Address;
                    Description.Text = restaurant.Description;
                    AverageBill.Text = "$ " + restaurant.AverageBill.ToString() + " for two people";
                    Email.Text = restaurant.Email;
                    if (restaurant.Telephone != null)
                    {
                        Telephone.Text = restaurant.Telephone;
                    }
                    else
                    {
                        Telephone.IsVisible = false;
                    }
                    if (restaurant.Email != null)
                    {
                        Email.Text = restaurant.Email;
                    }
                    else
                    {
                        Email.IsVisible = false;
                    }
                    if (restaurant.WebSiteLink != null)
                    {
                        Link.Text = restaurant.WebSiteLink;
                    }
                    else
                    {
                        Link.IsVisible = false;
                    }
                    if (restaurant.IsClosed == 1)
                    {
                        IsOpen.Text = "Closed now";
                        IsOpen.TextColor = Color.Red;
                    }
                    else
                    {
                        IsOpen.Text = "Open now";
                        IsOpen.TextColor = Color.Green;
                    }
                    if (MainPage.user.Id != 0 && MainPage.user.Id == restaurant.OwnerId || MainPage.user.Role == "admin")
                    {
                        AddPhoto.IsVisible = true;
                        AddMenu.IsVisible = true;
                    }
                }
            }
            Image image = new Image();
            foreach (Photoofrestaurant photoofrestaurant in PageOfRestaurants.photoofrestaurants)
            {
                if (PageOfRestaurants.restaurantmain.Id == photoofrestaurant.RestaurantId)
                {
                    ToRestImgImport toRestImgImport = new ToRestImgImport();
                    toRestImgImport.Id = photoofrestaurant.Id;
                    toRestImgImport.RestaurantId = photoofrestaurant.RestaurantId;
                    PageOfRestaurants.ToImgFromArr(photoofrestaurant.Photo, image);
                    toRestImgImport.Photo = image.Source;
                    toRestImgImports.Add(toRestImgImport);                  
                }
            }
            foreach (Menuofrestaurant menuofrestaurant in PageOfRestaurants.menuofrestaurants)
            {
                if (PageOfRestaurants.restaurantmain.Id == menuofrestaurant.RestaurantId)
                {
                    ToRestMenuImport toRestMenuImport = new ToRestMenuImport();
                    toRestMenuImport.Id = menuofrestaurant.Id;
                    toRestMenuImport.RestaurantId = menuofrestaurant.RestaurantId;
                    PageOfRestaurants.ToImgFromArr(menuofrestaurant.Photo, image);
                    toRestMenuImport.Menu = image.Source;
                    toRestMenuImports.Add(toRestMenuImport);
                }
            }
            foreach (Review review1 in PageOfRestaurants.reviews)
            {
                if (PageOfRestaurants.restaurantmain.Id == review1.RestaurantId)
                {
                    FullReview full = new FullReview();
                    full.Id = review1.Id;
                    foreach (UsersPhotos usersPhoto in usersPhotos1)
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
                    toReview.Add(full);
                }
            }
            ReviewsCollection.ItemsSource = toReview;
            RestaurantsPhotoCollection.ItemsSource = toRestImgImports;
            MenuPhotoCollection.ItemsSource = toRestMenuImports;
        }


        public async void Back_ToRestaurants(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                string r = PageOfRestaurants.LoadRestaurants().GetAwaiter().GetResult();
                if (r == "succsess")
                {
                    //PageOfRestaurants.DrawRestaurants(PageOfRestaurants.restaurantsShort, PageOfRestaurants.collection);
                    //App.Current.MainPage = new NavigationPage(new PageOfRestaurants());
                    Navigation.RemovePage(this);                    
                }
                else
                {
                    await DisplayAlert("Сообщение об ошибке", r, "OK");
                }

                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public async void GoToLink(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                Navigation.RemovePage(this);
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public async void Add_photo(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                try
                {
                    var photoDencode = await PageOfRestaurants.GetImageTo64();
                    if (photoDencode != null && photoDencode != "Object not selected" && photoDencode != "Error")
                    {
                        PageOfRestaurants.ImageExport friend = new PageOfRestaurants.ImageExport { userId = PageOfRestaurants.restaurantmain.Id, Photo = photoDencode };
                        string json = JsonConvert.SerializeObject(friend);
                        var content = await PageOfRestaurants.PhotoAddAsync("sendphoto/restaurantphoto/", json);
                        if (content != null &&
                        Regex.Match(content, "Connecting error").Success == false &&
                        Regex.Match(content, "No internet connection").Success == false)
                        {
                            string r = PageOfRestaurants.LoadRestaurants().GetAwaiter().GetResult();
                            if (r == "succsess")
                            {
                                LoadRestaurant();
                            }
                            else
                            {
                                await DisplayAlert("Сообщение об ошибке", r, "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Data", ex.Message, "OK");
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        public async void Add_photoMenu(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                try
                {
                    var photoDencode = await PageOfRestaurants.GetImageTo64();
                    if (photoDencode != null && photoDencode != "Object not selected" && photoDencode != "Error")
                    {
                        PageOfRestaurants.ImageExport friend = new PageOfRestaurants.ImageExport { userId = PageOfRestaurants.restaurantmain.Id, Photo = photoDencode };
                        string json = JsonConvert.SerializeObject(friend);
                        var content = await PageOfRestaurants.PhotoAddAsync("sendphoto/restaurantmenu/", json);
                        if (content != null &&
                        Regex.Match(content, "Connecting error").Success == false &&
                        Regex.Match(content, "No internet connection").Success == false)
                        {
                            string r = PageOfRestaurants.LoadRestaurants().GetAwaiter().GetResult();
                            if (r == "succsess")
                            {
                                LoadRestaurant();
                            }
                            else
                            {
                                await DisplayAlert("Сообщение об ошибке", r, "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Data", ex.Message, "OK");
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public async void See_reviews(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;

                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public async void Add_review(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;

                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
    }
}  

