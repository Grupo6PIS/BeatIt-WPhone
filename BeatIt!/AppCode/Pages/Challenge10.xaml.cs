using System;
using System.Windows;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using RestSharp;


namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge10
    {
        private const string AppKey = "mCRYcnQWo0mshpidnNRwHJqylB3Ap1crSMljsnExuNjkbguxty";

        private readonly CameraCaptureTask _photoCameraCapture = new CameraCaptureTask();
        private ChallengeDetail10 _currentChallenge;
        private IFacadeController _ifc;


        public Challenge10()
        {
            InitializeComponent();


            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail10)_ifc.getChallenge(10);
            _ifc.setCurrentChallenge(_currentChallenge);

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
            ProgressBar.IsIndeterminate = true;
            _photoCameraCapture.Completed += photoCameraCapture_Completed;
        }




        private readonly BitmapImage _image = new BitmapImage();
        private void photoCameraCapture_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK) return;
            _image.SetSource(e.ChosenPhoto);
            CountFacesServices();
            
        } 




        private void hyperlinkButtonStart_Click(object sender, RoutedEventArgs e)
        {
            _photoCameraCapture.Show();
            ProgressBar.Visibility = Visibility.Visible;
            
        }

        private void CountFacesServices()
        {
            // HARDCODE >>>>>>
/*
            var bitImage = new BitmapImage
            {
                CreateOptions = BitmapCreateOptions.None,
                UriSource = new Uri("/BeatIt!;component/Images/Messi_Cara.png", UriKind.Relative)
            };

            */
            var writableBitmap = new WriteableBitmap(_image);
            var ms = new MemoryStream();
            writableBitmap.SaveJpeg(ms, _image.PixelWidth, _image.PixelHeight, 0, 100);
            var imageBytes = ms.ToArray();

            //HARDCODE  <<<<<<

            var client = new RestClient("https://apicloud-facerect.p.mashape.com");
            var request = new RestRequest("/process-file.json", Method.POST);

            request.AddHeader("X-Mashape-Key", AppKey);
            request.AddFile("image", imageBytes, "image.png");

            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorMessage != null) return;
                ProgressBar.Visibility = Visibility.Visible;
                var json = JObject.Parse(response.Content);
                var cantidad = ((JArray)(json["faces"])).Count;
               _currentChallenge.CompleteChallenge(cantidad);
                
                MessageBox.Show("Hay un total de " + cantidad + " personas");
                ProgressBar.Visibility = Visibility.Collapsed;
                
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            });
        }

    }
}