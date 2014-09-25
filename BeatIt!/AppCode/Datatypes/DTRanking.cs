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

namespace BeatIt_.AppCode.Datatypes
{
    public class DTRanking
    {
        private int userId;
        private int position;
        private int score;
        private string name;
        private string imageUrl;

        public DTRanking(int userId,
                         int position,
                         int score,
                         string name,
                         string imageUrl) 
        {
            this.userId = userId;
            this.position = position;
            this.score = score;
            this.name = name;
            this.imageUrl = imageUrl;
        }

        public int getUserId() { return userId; }

        public int getPosition() { return position; }

        public int getScore() { return score; }

        public string getName() { return name; }

        public string getImageUrl() { return imageUrl; }
    }
}
