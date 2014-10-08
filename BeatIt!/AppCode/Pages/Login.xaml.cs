using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Facebook;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Pages
{
    public partial class Login
    {
        private readonly FacebookClient _fb = new FacebookClient();
        public string AccessToken { get; set; }
        private User _user;

        public Login()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn}
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut}
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            ProgressBar.IsIndeterminate = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bool isLoggedUser = IsolatedStorageSettings.ApplicationSettings.Contains("IsLoggedUser") && (bool)IsolatedStorageSettings.ApplicationSettings["IsLoggedUser"];
            if (isLoggedUser)
            {
                LoginBtn.IsEnabled = false;
                ProgressBar.Visibility = Visibility.Visible;

                _user = new User
                {
                    UserId = (int) IsolatedStorageSettings.ApplicationSettings["Id"],
                    FbId = (string) IsolatedStorageSettings.ApplicationSettings["FbId"],
                    FbAccessToken = (string) IsolatedStorageSettings.ApplicationSettings["FbAccessToken"],
                    FirstName = (string) IsolatedStorageSettings.ApplicationSettings["FirstName"],
                    LastName = (string) IsolatedStorageSettings.ApplicationSettings["LastName"],
                    Country = (string) IsolatedStorageSettings.ApplicationSettings["Country"],
                    BirthDate = (DateTime) IsolatedStorageSettings.ApplicationSettings["BirthDate"],
                    ImageUrl = (string) IsolatedStorageSettings.ApplicationSettings["ImageUrl"],
                    Email = (string) IsolatedStorageSettings.ApplicationSettings["Email"]
                };
                _user.Country = (string)IsolatedStorageSettings.ApplicationSettings["Country"];

                var ws = new WebServicesController();
                ws.GetRound(GetRoundFinished);
            }
            else
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            FacadeController.getInstance();

            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = "829846350380023";
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "page";
            parameters["scope"] = "user_hometown, publish_actions";

            var urlBuilder = new StringBuilder();
            foreach (var current in parameters)
            {
                if (urlBuilder.Length > 0)
                {
                    urlBuilder.Append("&");
                }
                var encoded = HttpUtility.UrlEncode((string)current.Value);
                urlBuilder.AppendFormat("{0}={1}", current.Key, encoded);
            }
            var loginUrl = "http://www.facebook.com/dialog/oauth?" + urlBuilder;

            AuthenticationBrowser.Navigate(new Uri(loginUrl));
            AuthenticationBrowser.Visibility = Visibility.Visible;

        }

        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {

            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    AuthenticationBrowser.Visibility = Visibility.Collapsed;
                    ProgressBar.Visibility = Visibility.Visible;
                    LoginBtn.IsEnabled = false;
                });
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(oauthResult.ErrorDescription));
            }
        }

        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(e.Error.Message);
                        ProgressBar.Visibility = Visibility.Collapsed;
                        LoginBtn.IsEnabled = true;
                    });
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                _user = new User
                {
                    UserId = 1,
                    FbId = (string) result["id"],
                    FbAccessToken = accessToken,
                    FirstName = (string) result["first_name"],
                    LastName = (string) result["last_name"],
                    Country = "Uruguay",
                    BirthDate = new DateTime(1989, 08, 07)
                };
                _user.ImageUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _user.FbId, "square", accessToken);
                if (result.Keys.Contains("email"))
                    _user.Email = (string)result["email"];
                if (result.Keys.Contains("hometown"))
                {
                    var ht = (IDictionary<string, object>)result["hometown"];
                    _user.Country = (string)ht["name"];
                }

                IsolatedStorageSettings.ApplicationSettings["IsLoggedUser"] = true;
                IsolatedStorageSettings.ApplicationSettings["Id"] = _user.UserId;
                IsolatedStorageSettings.ApplicationSettings["FbId"] = _user.FbId;
                IsolatedStorageSettings.ApplicationSettings["FbAccessToken"] = _user.FbAccessToken;
                IsolatedStorageSettings.ApplicationSettings["FirstName"] = _user.FirstName;
                IsolatedStorageSettings.ApplicationSettings["LastName"] = _user.LastName;
                IsolatedStorageSettings.ApplicationSettings["Country"] = _user.Country;
                IsolatedStorageSettings.ApplicationSettings["BirthDate"] = _user.BirthDate;
                IsolatedStorageSettings.ApplicationSettings["ImageUrl"] = _user.ImageUrl;
                IsolatedStorageSettings.ApplicationSettings["Email"] = _user.Email;
                IsolatedStorageSettings.ApplicationSettings.Save();

                var ws = new WebServicesController();
                ws.UpdateUser(_user.FbId, _user.FirstName, _user.ImageUrl, UpdateUserFinished);
            };

            fb.GetAsync("me");
        }

        private void UpdateUserFinished(JObject jsonResponse) 
        {
            if (!(bool)jsonResponse["error"])
            {
                var ws = new WebServicesController();
                ws.GetRound(GetRoundFinished);   
            }
            else
            {
                _user = null;
                IFacadeController ifc = FacadeController.getInstance();
                ifc.logoutUser();
                Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    LoginBtn.IsEnabled = true;
                    MessageBox.Show("Ha ocurrido un error al iniciar sesion");
                });
            }
        }

        private void GetRoundFinished(JObject jsonResponse)
        {
            if (!(bool)jsonResponse["error"])
            {
                IFacadeController ifc = FacadeController.getInstance();
                ifc.loginUser(_user, jsonResponse);
                Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    LoginBtn.IsEnabled = true;
                    NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative));
                });
            }
            else 
            {
                _user = null;
                IFacadeController ifc = FacadeController.getInstance();
                ifc.logoutUser();
                Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    LoginBtn.IsEnabled = true;
                    MessageBox.Show("Ha ocurrido un error al iniciar sesion");
                });
            }
        }
    }
}