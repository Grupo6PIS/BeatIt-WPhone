using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Facebook;
using Microsoft.Phone.Controls;


namespace BeatIt_.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            navigateInTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            navigateOutTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }

        private readonly FacebookClient _fb = new FacebookClient();

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            IFacadeController ifc = FacadeController.getInstance();

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
            var loginUrl = "http://www.facebook.com/dialog/oauth?" + urlBuilder.ToString();

            AuthenticationBrowser.Navigate(new Uri(loginUrl));
            AuthenticationBrowser.Visibility = Visibility.Visible;

        }

        public string AccessToken { get; set; }

        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {

            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                MessageBox.Show(oauthResult.ErrorDescription);
            }
        }

        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();

                User user = new User();
                user.UserId = 1;
                user.FbId = (string)result["id"];
                user.FbAccessToken = accessToken;
                user.FirstName = (string)result["first_name"];
                user.LastName = (string)result["last_name"];
                user.Country = "Uruguay";
                user.BirthDate = new DateTime(1989, 08, 07);
                user.ImageUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", user.FbId, "square", accessToken);
                user.Email = (string)result["email"];
                var ht = (IDictionary<string, object>)result["hometown"];
                user.Country = (string)ht["name"];
                IFacadeController ifc = FacadeController.getInstance();
                ifc.loginUser(user);

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative)));
            };

            fb.GetAsync("me");
        }
    }
}