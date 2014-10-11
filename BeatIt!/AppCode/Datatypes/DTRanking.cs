
namespace BeatIt_.AppCode.Datatypes
{
    public class DTRanking
    {
        public string UserId { get; set; }
        public int Position { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public DTRanking(string userId,
                         int position,
                         int score,
                         string name,
                         string imageUrl) 
        {
            UserId = userId;
            Position = position;
            Score = score;
            Name = name;
            ImageUrl = imageUrl;
        }
    }
}
