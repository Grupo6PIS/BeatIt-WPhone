using System;

namespace BeatIt_.AppCode.Classes
{
    public class User
    {
        public string UserId { get; set; }
        public string FbId { get; set; }
        public string FbAccessToken { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime BirthDate { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
    }
}
