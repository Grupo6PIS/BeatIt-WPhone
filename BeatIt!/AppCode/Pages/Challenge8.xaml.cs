using System.Windows;
using Microsoft.Phone.Controls;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge8 : PhoneApplicationPage
    {
        public Challenge8()
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
        }

        private void hyperlinkButtonStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void hyperlinkButtonNo_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void hyperlinkButtonYes_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}