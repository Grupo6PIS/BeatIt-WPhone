using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Controllers
{
    public class WebServicesController
    {

        private const string URL = "http://beatit-udelar.rhcloud.com";



        public delegate void CallbackWebService(JObject json);

        private CallbackWebService callback;


        public void Login(string userId, CallbackWebService callbackLogin)
        {
            if (callbackLogin!=null)
                callback = callbackLogin;
            string parameter = "userID=" + userId;

            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;


            wc.UploadStringAsync(new Uri(URL + "/user/login/"), "POST", parameter);
        }

        public void UpdateUser(string userId, string name, string imageURL, CallbackWebService callbackUpdateuser )
        {
            if (callbackUpdateuser!=null)
                callback = callbackUpdateuser;
            string parameter = "userID=" + userId + "&name=" + name + "&imageURL=" + imageURL;

            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;

            wc.UploadStringAsync(new Uri(URL + "/user/updateUser/"), "POST", parameter);
        }


        public void GetRound(CallbackWebService callbackGetRound)
        {
            if (callbackGetRound!=null)
                callback = callbackGetRound;
            WebClient wc = new WebClient();

            wc.DownloadStringCompleted+= WcOnDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(URL + "/round/getRound/"), "GET");
        }

        public void GetRanking(CallbackWebService callbackGetRanking)
        {
            if (callbackGetRanking!=null)
                callback = callbackGetRanking;
            WebClient wc = new WebClient();

            wc.DownloadStringCompleted += WcOnDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(URL + "/round/getRanking/"), "GET");
        }

        public void SendScore(string userId, int score, CallbackWebService callbackSendScore)
        {
            if (callbackSendScore!=null)
                callback = callbackSendScore;
            string parameter = "userID=" + userId + "&score=" + score;
            
            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;

            wc.UploadStringAsync(new Uri(URL + "/user/updateUser/"), "POST", parameter);

        }

        private void WcOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result != null && callback != null)
            {
                callback(JObject.Parse(e.Result));
                callback = null;
            }
        }

        public void WcOnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Result != null && callback != null)
            {
                callback(JObject.Parse(e.Result));
                callback = null;
            }
        }
    }
}
