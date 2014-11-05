using System;
using System.Globalization;
using System.Windows;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
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
            _currentChallenge = (ChallengeDetail10)_ifc.GetChallenge(10);

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


            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();
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
            MessageBox.Show(AppResources.Challenge10_Warning);
            _photoCameraCapture.Show();
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;
            ProgressBar.Visibility = Visibility.Visible;
            
        }

        private void CountFacesServices()
        {

            var writableBitmap = new WriteableBitmap(_image);
            var ms = new MemoryStream();
            writableBitmap.SaveJpeg(ms, _image.PixelWidth, _image.PixelHeight, 0, 50);
            var imageBytes = ms.ToArray();

            var client = new RestClient("https://apicloud-facerect.p.mashape.com");
            var request = new RestRequest("/process-file.json", Method.POST);

            request.AddHeader("X-Mashape-Key", AppKey);
            request.AddFile("image", imageBytes, "image.png");

            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorMessage != null) return;
                ProgressBar.Visibility = Visibility.Visible;
                if (!string.IsNullOrEmpty(response.Content))
                {
                    var json = JObject.Parse(response.Content);
                    var cantidad = ((JArray) (json["faces"])).Count;
                    _currentChallenge.CompleteChallenge(cantidad);

                    MessageBox.Show(AppResources.Challenge10_Count.Replace("@faces",
                        cantidad.ToString(CultureInfo.InvariantCulture)));
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show(AppResources.Challenge10_Error);
                    _currentChallenge.CompleteChallenge(0);
                }
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
                
            });
        }

    }
}