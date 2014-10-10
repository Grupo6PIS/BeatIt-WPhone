
namespace BeatIt_.AppCode.Datatypes
{
    public class DTRanking
    {
        public int UserId { get; set; }
        public int Position { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public DTRanking(int userId,
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
