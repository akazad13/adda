namespace EasyConnect.API.Models
{
    public class Bookmark
    {
        public int BookmarkerId { get; set; }
        public User Bookmarker { get; set; }
        public int BookmarkedId { get; set; }
        public User Bookmarked { get; set; }
    }
}
