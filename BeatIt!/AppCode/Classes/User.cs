using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BeatIt_.AppCode.Classes
{
    public class User
    {
        private int userId;
        private string fbId;
        private string fbAccessToken;
        private string firstName;
        private string lastName;
        private string country;
        private DateTime birthDate;
        private string imageUrl;
        private string email;

        public User()
        {
 
        }

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string FbId
        {
            get { return fbId; }
            set { fbId = value; }
        }

        public string FbAccessToken
        {
            get { return fbAccessToken; }
            set { fbAccessToken = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        public DateTime BirthDate
        {
            get { return birthDate; }
            set { birthDate = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
