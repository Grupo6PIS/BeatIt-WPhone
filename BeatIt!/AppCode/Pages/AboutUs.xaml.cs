using Microsoft.Phone.Controls;

namespace BeatIt_.AppCode.Pages
{
    public partial class AboutUs : PhoneApplicationPage
    {
        public AboutUs()
        {
            InitializeComponent();
            InitDevelopListBox();
        }

        private void InitDevelopListBox()
        {
          /*  DevelopListBox.Items.Clear();
            var listItem = new DeveloperListItem();
            listItem.NameTxtBlock.Text = "Martin";
            listItem.RoleTxtBlock.Text = "UI";
            var uri = new Uri("http://graph.facebook.com/100002316914037/picture?type=square", UriKind.Absolute);
            listItem.UserImage.Source = new BitmapImage(uri);
            DevelopListBox.Items.Add(listItem);
            /*var r2 = new DTRanking("2", 2, 127, "Martín Berguer",
                "");
            var r3 = new DTRanking("3", 3, 106, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            var r4 = new DTRanking("4", 4, 94, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            var r5 = new DTRanking("5", 5, 73, "Alejandro Brusco",
                "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            var r6 = new DTRanking("6", 6, 22, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            var r7 = new DTRanking("7", 7, 15, "Martín Steglich",
                "http://graph.facebook.com/tinchoste/picture?type=square");*/
        }
    }
}