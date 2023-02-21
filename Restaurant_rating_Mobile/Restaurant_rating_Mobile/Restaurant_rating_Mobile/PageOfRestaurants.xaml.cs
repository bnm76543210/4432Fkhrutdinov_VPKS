using IntelliAbb.Xamarin.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Restaurant_rating_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageOfRestaurants : ContentPage
    {
        public static string photoDencode;

        public class ShortRestaurantData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Cathegories { get; set; }
            public string AverageBill { get; set; }
            public string Address { get; set; }
            public string Score { get; set; }
            public ImageSource Image { get; set; }
            public ImageSource Like { get; set; }
            public int IsClosed { get; set; }
        }
        public class ImageExport
        {
            public int userId { get; set; }
            public string Photo { get; set; }
        }

        public static List<Restaurant> restaurants = new List<Restaurant>();
        public static List<Restaurantstypes> types = new List<Restaurantstypes>();
        public static List<Typeofrestaurant> typeofrestaurants = new List<Typeofrestaurant>();
        public static List<Likeforrestaurant> restaurants_like = new List<Likeforrestaurant>();
        public static List<Photoofrestaurant> photoofrestaurants = new List<Photoofrestaurant>();
        public static List<Menuofrestaurant> menuofrestaurants = new List<Menuofrestaurant>();
        public static List<Review> reviews = new List<Review>();
        public static List<ShortRestaurantData> restaurantsShort = new List<ShortRestaurantData>();
        public static List<ShortRestaurantData> restaurantsShort1 = new List<ShortRestaurantData>();
        public static string FilterSearch;
        public static ShortRestaurantData restaurantmain;
        public static CollectionView collection;
        public PageOfRestaurants()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            collection = RestaurantsCollection;
            string r = LoadRestaurants().GetAwaiter().GetResult();
            if (r == "succsess")
            {
                DrawRestaurants(restaurantsShort, RestaurantsCollection);
            }
            else
            {
                DisplayAlert("Сообщение об ошибке", r, "OK");
            }
            RestaurantsCollection.ItemsSource = restaurantsShort;
            RestaurantsTypes.ItemsSource = types;
            if (MainPage.user.Id != 0)
            {
                ToImgButtoFromArr(MainPage.user.Photo, ProfileIMG);
            }

        }

        /// <summary>
        /// Конфертация фотографии аватара из строки в image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="toimgsource"></param>
        public async void ToImgButtoFromArr(string image, ImageButton toimgsource)
        {
            try
            {
                byte[] imageAsBytes = Convert.FromBase64String(image);
                Image image1 = new Image();
                image1.Source = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(imageAsBytes);
                });
                toimgsource.Source = image1.Source;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Конфертация фотографии из строки в image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="toimgsource"></param>
        public static async void ToImgFromArr(string image, Image toimgsource)
        {
            byte[] imageAsBytes = Convert.FromBase64String(image);
            Image image1 = new Image();
            image1.Source = ImageSource.FromStream(() =>
            {
                return new MemoryStream(imageAsBytes);
            });
            toimgsource.Source = image1.Source; 
        }

        /// <summary>
        /// Прорисовка всех ресторанов
        /// </summary>
        public static void DrawRestaurants(List<ShortRestaurantData> list,CollectionView collectionView)
        {
            list.Clear();
            collectionView.ItemsSource = null;
            foreach (Restaurant restaurant in restaurants)
            {
                ShortRestaurantData shortRestaurantData = new ShortRestaurantData();
                shortRestaurantData.Id = restaurant.Id;
                shortRestaurantData.Name = restaurant.Name;
                shortRestaurantData.AverageBill = "$ " + restaurant.AverageBill.ToString() + " for two people";
                shortRestaurantData.Address = restaurant.Address;
                shortRestaurantData.IsClosed = restaurant.IsClosed;
                foreach (Typeofrestaurant typeofrestaurant in typeofrestaurants)
                {
                    if (restaurant.Id == typeofrestaurant.RestaurantId)
                    {
                        foreach (Restaurantstypes restaurantstypes1 in types)
                        {
                            if (typeofrestaurant.TypeId == restaurantstypes1.Id)
                            {
                                shortRestaurantData.Cathegories += (restaurantstypes1.TypeName + "  ");
                            }
                        }
                    }
                }
                foreach (Photoofrestaurant photoofrestaurant in photoofrestaurants)
                {
                    if (restaurant.Id == photoofrestaurant.RestaurantId)
                    {
                        Image image = new Image();
                        ToImgFromArr(photoofrestaurant.Photo, image);
                        shortRestaurantData.Image = image.Source;
                    }
                }
                float result = 0;
                int quantity = 0;
                foreach (Review review in reviews)
                {
                    if (restaurant.Id == review.RestaurantId)
                    {
                        result += review.Rating;
                        quantity++;
                    }
                }
                if (quantity > 0)
                {
                    shortRestaurantData.Score = Math.Round((result / quantity), 1, MidpointRounding.AwayFromZero).ToString();
                }
                else
                {
                    shortRestaurantData.Score = "No";
                }
                if (MainPage.user.Id != 0)
                {
                    shortRestaurantData.Like = ImageSource.FromFile("like_heart.png");
                    foreach (Likeforrestaurant restaurantlike in restaurants_like)
                    {
                        if (restaurant.Id == restaurantlike.RestaurantID)
                        {
                            shortRestaurantData.Like = ImageSource.FromFile("redlike_heart.png");
                        }
                    }
                }
                else
                {
                    shortRestaurantData.Like = ImageSource.FromFile("like_heart.png");
                }
                list.Add(shortRestaurantData);
            }
            collection.ItemsSource = restaurantsShort;
        }


        /// <summary>
        /// Загрузка всех ресторанов
        /// </summary>
        public static async Task<string> LoadRestaurants()
        {
            try
            {
                int a = 0;
                var content = Registration.PostCreateFoundation("restaurants").GetAwaiter().GetResult();
                if (content != null &&
                Regex.Match(content, "Connecting error").Success == false)
                {
                    if (Regex.Match(content, "restaurants does not exist").Success == true)
                    {
                        return "No restaurants here";
                    }
                    else
                    {
                        restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(content);
                        a++;
                    }
                }
                else
                {
                     return "Connection error, please try again later";
                }
                var content_types = Registration.PostCreateFoundation("showtypes").GetAwaiter().GetResult();
                if (content_types != null &&
                Regex.Match(content_types, "Connecting error").Success == false)
                {
                    if (Regex.Match(content_types, "restaurants types does not exist").Success == true)
                    {
                        return "No restaurants here";
                    }
                    else
                    {
                        types = JsonConvert.DeserializeObject<List<Restaurantstypes>>(content_types);
                        a++;
                    }
                }
                else
                {
                    return "Connection error, please try again later";
                }
                var content_reviews = Registration.PostCreateFoundation("reviews").GetAwaiter().GetResult();
                if (content_reviews != null &&
                Regex.Match(content_reviews, "Connecting error").Success == false)
                {
                    if (Regex.Match(content_types, "Review does not exist ").Success == true)
                    {
                        return "No reviews here";
                    }
                    else
                    {
                        reviews = JsonConvert.DeserializeObject<List<Review>>(content_reviews);
                        a++;
                    }
                }
                else
                {
                    return "Connection error, please try again later";
                }
                var restaurant_types = Registration.PostCreateFoundation("typesofrestaurant").GetAwaiter().GetResult();
                if (restaurant_types != null &&
                Regex.Match(restaurant_types, "Connecting error").Success == false)
                {
                    if (Regex.Match(restaurant_types, "restaurants types does not exist").Success == false)
                    {
                        typeofrestaurants = JsonConvert.DeserializeObject<List<Typeofrestaurant>>(restaurant_types);
                        a++;
                    }
                }
                else
                {
                    return "Connection error, please try again later";
                }
                var restaurants_photo = Registration.PostCreateFoundation("getphoto/restaurants").GetAwaiter().GetResult();
                if (restaurants_photo != null &&
                Regex.Match(restaurants_photo, "Connecting error").Success == false)
                {
                    if (Regex.Match(restaurants_photo, "restaurants does not exist").Success == true)
                    {
                        return "No restaurants here";
                    }
                    else
                    {
                        photoofrestaurants = JsonConvert.DeserializeObject<List<Photoofrestaurant>>(restaurants_photo);
                        a++;
                    }
                }
                else
                {
                    return "Connection error, please try again later";
                }
                var restaurantsmenu_photo = Registration.PostCreateFoundation("getphoto/restmenu").GetAwaiter().GetResult();
                if (restaurantsmenu_photo != null &&
                Regex.Match(restaurantsmenu_photo, "Connecting error").Success == false)
                {
                    if (Regex.Match(restaurantsmenu_photo, "restaurants menu photo does not exist ").Success == true)
                    {
                        return "restaurants menu photo does not exist ";
                    }
                    else
                    {
                        menuofrestaurants = JsonConvert.DeserializeObject<List<Menuofrestaurant>>(restaurantsmenu_photo);
                        a++;
                    }
                }
                else
                {
                    return "Connection error, please try again later";
                }
                if (MainPage.user.Id != 0)
                {
                    var restaurant_likes = Registration.PostCreateFoundation("showlikes/?userid=" + MainPage.user.Id).GetAwaiter().GetResult();
                    if (restaurant_likes != null &&
                    Regex.Match(restaurant_likes, "Connecting error").Success == false)
                    {
                        if (Regex.Match(restaurant_likes, "there is no likes").Success == false)
                        {
                            restaurants_like = JsonConvert.DeserializeObject<List<Likeforrestaurant>>(restaurant_likes);
                            a++;
                        }
                    }
                    else
                    {
                        return "Connection error, please try again later";
                    }
                }
                if(a >= 5)
                {
                    return "succsess";
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Добавление типа ресторана
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Handle_IsCheckedChanged(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                Button button = (Button)sender;
                var type = (Restaurantstypes)button.BindingContext;
                await DisplayAlert("Сообщение об ошибке", button.Id.ToString(), "OK");
                if (button.BackgroundColor == Color.LightGray)
                {
                    FilterSearch += (type.TypeName + " ");
                    button.BackgroundColor = Color.LightPink;
                }
                else
                {
                    if (FilterSearch != null)
                    {
                        FilterSearch = FilterSearch.Replace((type.TypeName + " "), "");
                        button.BackgroundColor = Color.LightGray;
                    }
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        /// <summary>
        /// Местоположение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_location(object sender, System.EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                //Button button = (Button)sender;
                //await Navigation.PushAsync(new Registration());
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        /// <summary>
        /// Выбор фото профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_profile(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                try
                {
                    if (MainPage.user.Id != 0)
                    {                
                        var photoDencode = await GetImageTo64();
                        if (photoDencode != null && photoDencode != "Object not selected" && photoDencode != "Error")
                        {
                            ImageExport friend = new ImageExport { userId = MainPage.user.Id, Photo = photoDencode };
                            string json = JsonConvert.SerializeObject(friend);
                            var content = await PhotoAddAsync("sendphoto/userphoto/", json);
                            if (content != null &&
                            Regex.Match(content, "Connecting error").Success == false &&
                            Regex.Match(content, "No internet connection").Success == false)
                            {
                                await DisplayAlert("Data", content.ToString(), "OK");
                                ToImgButtoFromArr(photoDencode, ProfileIMG);
                            }
                            else
                            {
                                await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                            }
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

        /// <summary>
        /// поисковая строка фильтров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_restaurant(object sender, EventArgs e)
        {
            try
            {
                restaurantsShort1.Clear();
                RestaurantsCollection.ItemsSource = null;
                Isopen.BackgroundColor = Color.White;
                Rating_3.BackgroundColor = Color.White;
                Rating_4.BackgroundColor = Color.White;
                if (Search_textInput.Text != null)
                {                   
                    foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                    {
                        if (shortRestaurantData.Name.Contains(Search_textInput.Text))
                        {
                            restaurantsShort1.Add(shortRestaurantData);               
                        }
                    }
                    RestaurantsCollection.ItemsSource = restaurantsShort1;
                }
                else
                {
                    foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                    {
                        restaurantsShort1.Add(shortRestaurantData);
                    }
                    RestaurantsCollection.ItemsSource = restaurantsShort1;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Открывает панель выбора фильтров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Filters_Button(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                //SlidingPanel.HeightRequest = 0;
                //ShadowGrid.IsVisible = true;
                //for (; SlidingPanel.HeightRequest <= 400;)
                //{
                //    SlidingPanel.HeightRequest += 20;
                //    await Task.Delay(1);
                //}

                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        /// <summary>
        /// Закрывает панель выбора фильтров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BackFromFilters(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                SlidingPanel.HeightRequest = 400;
                for (; SlidingPanel.HeightRequest > 0;)
                {
                    SlidingPanel.HeightRequest -= 20;
                    await Task.Delay(1);
                }
                ShadowGrid.IsVisible = false;
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        /// <summary>
        /// Очищает строку выбора и сам выбор филтров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClearAll(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                //Button button = (Button)sender;
                //await Navigation.PushAsync(new Registration());

                FilterSearch = "";
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        /// <summary>
        /// Выполняет поиск по фильтрам
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetTypes(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                RestaurantsCollection.ItemsSource = null;
                await DisplayAlert("Check connection", FilterSearch, "OK");
                foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                {
                    //await DisplayAlert("Check connection", "succsess", "OK");
                    if (Regex.Match(shortRestaurantData.Cathegories, FilterSearch).Success == true)
                    {
                        await DisplayAlert("Check connection", "succsess", "OK");
                    }
                }
                //var checkbox = RestaurantsTypes.FindByName<Checkbox>("TypeCheck");
                //Grid grid = checkbox.Parent as Grid;
                //Restaurantstypes type = (Restaurantstypes)grid.BindingContext;
                //string selectedValues = string.Empty;
                //foreach (Restaurantstypes item in RestaurantsTypes.ItemsSource)
                //{
                //    if (checkbox.IsChecked)
                //    {
                //        selectedValues += type.TypeName + ",";
                //    }
                //}

                //await DisplayAlert("Check connection", selectedValues, "OK");

                //SlidingPanel.HeightRequest = 400;
                //for (; SlidingPanel.HeightRequest > 0;)
                //{
                //    SlidingPanel.HeightRequest -= 20;
                //    await Task.Delay(1);
                //}

                //ShadowGrid.IsVisible = false;
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        private async void Filter_Button(object sender, EventArgs e)
        {
            try
            {
                if (MainPage._canTap)
                {
                    MainPage._canTap = false;
                    Search_textInput.Text = "";
                    Button button = (Button)sender;
                    restaurantsShort1.Clear();
                    RestaurantsCollection.ItemsSource = null;
                    if (button.Text == "Rating 4.0+")
                    {
                        Rating_3.BackgroundColor = Color.White;
                        Isopen.BackgroundColor = Color.White;
                        if (button.BackgroundColor == Color.White)
                        {
                            button.BackgroundColor = Color.LightPink;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {                                
                                if (shortRestaurantData.Score != "No")
                                {
                                    if (Convert.ToDouble(shortRestaurantData.Score) >= 4.0)
                                    {
                                        restaurantsShort1.Add(shortRestaurantData);
                                    }
                                }
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                        else
                        {
                            button.BackgroundColor = Color.White;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {
                                restaurantsShort1.Add(shortRestaurantData);
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                    }
                    if (button.Text == "Rating 3.0+")
                    {
                        Rating_4.BackgroundColor = Color.White;
                        Isopen.BackgroundColor = Color.White;
                        if (button.BackgroundColor == Color.White)
                        {
                            button.BackgroundColor = Color.LightPink;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {
                                if (shortRestaurantData.Score != "No")
                                {
                                    if (Convert.ToDouble(shortRestaurantData.Score) >= 3.0)
                                    {
                                        restaurantsShort1.Add(shortRestaurantData);
                                    }
                                }
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                        else
                        {
                            button.BackgroundColor = Color.White;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {
                                restaurantsShort1.Add(shortRestaurantData);
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                    }
                    if (button.Text == "Is open")
                    {
                        Rating_3.BackgroundColor = Color.White;
                        Rating_4.BackgroundColor = Color.White;
                        if (button.BackgroundColor == Color.White)
                        {
                            button.BackgroundColor = Color.LightPink;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {                              
                                if (shortRestaurantData.IsClosed == 0)
                                {
                                    restaurantsShort1.Add(shortRestaurantData);
                                }
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                        else
                        {
                            button.BackgroundColor = Color.White;
                            foreach (ShortRestaurantData shortRestaurantData in restaurantsShort)
                            {
                                restaurantsShort1.Add(shortRestaurantData);
                            }
                            RestaurantsCollection.ItemsSource = restaurantsShort1;
                        }
                    }
                    await Task.Delay(200);
                    MainPage._canTap = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }      

        private async void Show_restaurant(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                //Button button = (Button)sender;
                //await Navigation.PushAsync(new Registration());

                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        private async void Like_Button(object sender, EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                if (MainPage.user.Id != 0)
                {
                    Search_textInput.Text = null;
                    Isopen.BackgroundColor = Color.White;
                    Rating_3.BackgroundColor = Color.White;
                    Rating_4.BackgroundColor = Color.White;
                    ImageButton button = sender as ImageButton;
                    Grid grid = button.Parent.Parent.Parent.Parent as Grid;
                    ShortRestaurantData restaurant = (ShortRestaurantData)grid.BindingContext;
                    try
                    {
                        if (button.Source == ImageSource.FromFile("like_heart.png"))
                        {
                            await DisplayAlert("Error", "try", "OK");
                            var content = Registration.PostCreateFoundation("sendrestlike/?userID=" + MainPage.user.Id
                                + "& restID=" + restaurant.Id).GetAwaiter().GetResult();
                            if (content != null &&
                            Regex.Match(content, "Connecting error").Success == false &&
                            Regex.Match(content, "No internet connection").Success == false &&
                            Regex.Match(content, "Like set").Success == true)
                            {
                                string r = LoadRestaurants().GetAwaiter().GetResult();
                                if (r == "succsess")
                                {
                                    DrawRestaurants(restaurantsShort, RestaurantsCollection);
                                }
                                else
                                {
                                    await DisplayAlert("Сообщение об ошибке", r, "OK");
                                }
                                RestaurantsCollection.ItemsSource = restaurantsShort;
                            }
                            else
                            {
                                await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                            }
                        }
                        else
                        {
                            var content = Registration.PostCreateFoundation("reseresttlike/?userID=" + MainPage.user.Id
                                + "& restID=" + restaurant.Id).GetAwaiter().GetResult();
                            if (content != null &&
                            Regex.Match(content, "Connecting error").Success == false &&
                            Regex.Match(content, "No internet connection").Success == false &&
                            Regex.Match(content, "Like reset").Success == true)
                            {
                                string r = LoadRestaurants().GetAwaiter().GetResult();
                                if (r == "succsess")
                                {
                                    DrawRestaurants(restaurantsShort, RestaurantsCollection);
                                }
                                else
                                {
                                    await DisplayAlert("Сообщение об ошибке", r, "OK");
                                }
                                RestaurantsCollection.ItemsSource = restaurantsShort;
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
                }
                else
                {
                    bool result = await DisplayAlert("You are not registred", "Do you want to register?", "Yes", "No");
                    if(result == true)
                    {
                        await Navigation.PushModalAsync(new MainPage());
                        App.Current.MainPage = new NavigationPage(new MainPage());
                    }
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        private async void RestaurantsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                Search_textInput.Text = "";
                Isopen.BackgroundColor = Color.White;
                Rating_3.BackgroundColor = Color.White;
                Rating_4.BackgroundColor = Color.White;
                restaurantmain = e.CurrentSelection[0] as ShortRestaurantData;
                RestaurantsCollection.SelectedItem = null;
                //App.Current.MainPage = new NavigationPage(new PageOfRestaurants());
                await Navigation.PushAsync(new RestaurantPage());
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }

        public static async Task<string> PhotoAddAsync(string finalOfLink, string json)
        {
            if (await App.CheckConnection() != "NetworkAccess error")
            {
                HttpContent content = new StringContent(json);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.PostAsync(MainPage.HostLink + finalOfLink, content).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent responseContent = response.Content;
                    var json1 = await responseContent.ReadAsStringAsync();
                    if (content != null &&
                        Regex.Match(json1, "Connecting error").Success == false &&
                        Regex.Match(json1, "Error: ").Success == false && Regex.Match(json1, "Photo set").Success == true)
                    {
                        return json1.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "No internet connection";
            }
        }

        public static async Task<string> GetImageTo64()
        {
            try
            {
                FileResult photo = await MediaPicker.PickPhotoAsync();
                Image img = new Image();
                img.Source = ImageSource.FromFile(photo.FullPath);
                var photoDencode = Convert.ToBase64String(ReadAllBytes(photo.FullPath));
                return photoDencode;
            }
            catch (Exception ex)
            {
                if (ex.Message != "Object reference not set to an instance of an object.")
                {
                    return "Object not selected";
                }
                else
                {
                    return "Error";
                }
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                using (var memoryStream = new MemoryStream())
                {
                    streamReader.BaseStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}