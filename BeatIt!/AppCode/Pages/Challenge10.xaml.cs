using Microsoft.Phone.Controls;
using System.Net;
using System;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using RestSharp;



namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge10 : PhoneApplicationPage
    {

        private string appKey = "mCRYcnQWo0mshpidnNRwHJqylB3Ap1crSMljsnExuNjkbguxty";

        public Challenge10()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn }
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
        }

        private void hyperlinkButtonStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            countFacesServices(null);
        }

        private void countFacesServices(byte[] image)
        {
            // HARDCODE >>>>>>

            var bitImage = new BitmapImage();
            bitImage.CreateOptions = BitmapCreateOptions.None;
            bitImage.UriSource = new Uri("/BeatIt!;component/Images/Messi_Cara.png", UriKind.Relative);



            WriteableBitmap writableBitmap = new WriteableBitmap(bitImage);
            MemoryStream ms = new MemoryStream();
            writableBitmap.SaveJpeg(ms, bitImage.PixelWidth, bitImage.PixelHeight, 0, 100);
            byte[] imageBytes = ms.ToArray();

            //HARDCODE  <<<<<<

            var client = new RestClient("https://apicloud-facerect.p.mashape.com");
            var request = new RestRequest("/process-file.json", Method.POST);

            request.AddHeader("X-Mashape-Key", appKey);
            request.AddFile("image", imageBytes, "image.png");

            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorMessage == null)
                {
                    var json = JObject.Parse(response.Content);
                    int cantidad = ((JArray)(json["faces"])).Count;
                }
            });
        }

    }
}