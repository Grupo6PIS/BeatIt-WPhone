using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BeatIt_.AppCode.Controllers
{
    public class WebServicesController
    {
        public delegate void CallbackWebService(JObject json);

        private const string Url = "http://beatit-udelar.rhcloud.com";

        private CallbackWebService _callback;

        public void Login(string userId, CallbackWebService callbackLogin)
        {
            if (callbackLogin != null)
                _callback = callbackLogin;
            string parameter = "userID=" + userId;

            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;


            wc.UploadStringAsync(new Uri(Url + "/user/login/"), "POST", parameter);
        }

        public void SendAllStates(string json)
        {
            string parameter = "data=" + json;

            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;

            wc.UploadStringAsync(new Uri(Url + "/user/sendAllStates/"), "POST", parameter);
        }

        public void UpdateUser(string userId, string name, string imageUrl, CallbackWebService callbackUpdateuser)
        {
            if (callbackUpdateuser != null)
                _callback = callbackUpdateuser;
            string parameter = "userID=" + userId + "&name=" + name + "&imageURL=" + imageUrl;

            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;

            wc.UploadStringAsync(new Uri(Url + "/user/updateUser/"), "POST", parameter);
        }


        public void GetRound(CallbackWebService callbackGetRound)
        {
            if (callbackGetRound != null)
                _callback = callbackGetRound;
            var wc = new WebClient();

            wc.DownloadStringCompleted += WcOnDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(Url + "/round/getRound/"), "GET");
        }

        public void GetRanking(CallbackWebService callbackGetRanking)
        {
            if (callbackGetRanking != null)
                _callback = callbackGetRanking;
            var wc = new WebClient();

            wc.DownloadStringCompleted += WcOnDownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(Url + "/round/getRanking/"), "GET");
        }

        public void SendScore(string userId, int score, CallbackWebService callbackSendScore)
        {
            if (callbackSendScore != null)
                _callback = callbackSendScore;
            string parameter = "userID=" + userId + "&score=" + score;

            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringCompleted += WcOnUploadStringCompleted;

            wc.UploadStringAsync(new Uri(Url + "/round/sendScore/"), "POST", parameter);
        }

        private void WcOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && _callback != null)
            {
                _callback(JObject.Parse(e.Result));
                _callback = null;
            }
            else
            {
                const string errorStr = "{'error':true}";
                _callback(JObject.Parse(errorStr));
                _callback = null;
            }
        }

        public void WcOnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (_callback!=null){
                if (e.Error == null)
                {
                    _callback(JObject.Parse(e.Result));
                    _callback = null;
                }
                else
                {
                    const string errorStr = "{'error':true}";
                    _callback(JObject.Parse(errorStr));
                    _callback = null;
                }
            }
        }
    }
}