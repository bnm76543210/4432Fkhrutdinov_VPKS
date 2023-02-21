using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Restaurant_rating_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        public static bool IsValidPassword(string password)
        {
            return
               password.Any(c => IsLetter(c)) &&
               password.Any(c => IsDigit(c)) &&
               password.Any(c => IsSymbol(c));
        }
        static bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
        static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        static bool IsSymbol(char c)
        {
            return c > 32 && c < 127 && !IsDigit(c) && !IsLetter(c);
        }
        public static int code;
        public static string mainMail = "fakhrutdinov.marat@bk.ru"; //email: restaurants.rating@mail.ru password: nRBUoytTt14;
        public static string mailpass = "jnd0BsHZ4qKUXHrWenwS"; // BwgmEJNAyfhkiJapd8kY
        public Registration()
        {
            InitializeComponent();
        }
        private async void Create_Account_Button(object sender, System.EventArgs e)
        {
            if (MainPage._canTap)
            {
                MainPage._canTap = false;
                int a = 0;
                UserName_label.TextColor = Color.Transparent;
                Email_label.TextColor = Color.Transparent;
                Password_label.TextColor = Color.Transparent;
                if (Username_textInput.Text != null && Email_textInput != null && Password_textInput != null)
                {
                    Regex UserName_regex = new Regex(@"^[a-zA-Z0-9]+$");
                    if (UserName_regex.Match(Username_textInput.Text).Success == false)
                    {
                        UserName_label.TextColor = Color.Red;
                        UserName_label.Text = "Username can contain only letters and numbers";
                        a = 1;
                    }
                    if (Username_textInput.Text.Length <= 7)
                    {
                        UserName_label.TextColor = Color.Red;
                        UserName_label.Text = "Username must be longer than 7 characters";
                        a = 1;
                    }
                    const string pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                    var email = Email_textInput.Text.Trim().ToLowerInvariant();
                    if (Regex.Match(email, pattern).Success == false)
                    {
                        Email_label.TextColor = Color.Red;
                        Email_label.Text = "Email specified incorrectly";
                        a = 1;
                    }
                    if (Email_textInput.Text.Length <= 5)
                    {
                        Email_label.TextColor = Color.Red;
                        Email_label.Text = "Email must be longer than 5 characters";
                        a = 1;
                    }
                    if (IsValidPassword(Password_textInput.Text) == false)
                    {
                        Password_label.TextColor = Color.Red;
                        Password_label.Text = "Password must contains at least 1 alphabet character, 1 number, 1 symbol";
                        a = 1;
                    }
                    if (Password_textInput.Text.Length <= 7)
                    {
                        Password_label.TextColor = Color.Red;
                        Password_label.Text = "Password must be longer than 7 characters";
                        a = 1;
                    }
                    var content = PostCreateFoundation("checkreg/?name=" + Username_textInput.Text + "& email=" + Email_textInput.Text).GetAwaiter().GetResult();
                    if (content != null &&
                    Regex.Match(content, "Connecting error").Success == false &&
                    Regex.Match(content, "No internet connection").Success == false)
                    {
                        if (Regex.Match(content, "email exist").Success == true)
                        {
                            Email_label.TextColor = Color.Red;
                            Email_label.Text = "Email already exist";
                            a = 1;
                        }
                        if (Regex.Match(content, "login exist").Success == true)
                        {
                            UserName_label.TextColor = Color.Red;
                            UserName_label.Text = "Username already exist";
                            a = 1;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Check connection", "Connection error, please try again later", "OK");
                        a = 1;
                    }

                    if (a == 0)
                    {
                        if (await App.CheckConnection() != "NetworkAccess error")
                        {
                            try
                            {
                                Send_to_email(Email_textInput.Text);
                                MainPage.user = new Users();
                                MainPage.user.Name = Username_textInput.Text.Trim().ToLowerInvariant();
                                MainPage.user.Password = Password_textInput.Text.Trim().ToLowerInvariant();
                                MainPage.user.Email = Email_textInput.Text.Trim().ToLowerInvariant();
                                EmailConfirmation.IsPassRecovery = false;
                                await Navigation.PushAsync(new EmailConfirmation());
                                MainPage._canTap = true;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message == "failed recipients")
                                {
                                    Email_label.TextColor = Color.Red;
                                    Email_label.Text = "Email specified incorrectly";
                                    await DisplayAlert("Mail sending", ex.Message, "OK");
                                }
                                else
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
                }
                await Task.Delay(200);
                MainPage._canTap = true;
            }
        }
        public static async Task<string> PostCreateFoundation(string FinalPart)
        {
            if (await App.CheckConnection() != "NetworkAccess error")
            {
                var uri = new UriBuilder(MainPage.HostLink + FinalPart).Uri;
                var client = new WebClient();
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback
                    (
                       delegate { return true; }
                    );
                var content = client.DownloadString(uri);
                return content;
            }
            else
            {
                return "No internet connection";
            }
        }
        public static void Send_to_email(string email_address)
        {
            Random rnd = new Random();
            code = rnd.Next(10000, 99999);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(mainMail);
            mail.To.Add(new MailAddress(email_address));
            mail.Subject = "Verification code: ";
            mail.Body = "Code: " + code;
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.mail.ru";
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(mainMail, mailpass);
            client.Send(mail);
        }
    }
}