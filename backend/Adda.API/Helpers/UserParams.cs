namespace Adda.API.Helpers;

public class UserParams
{
    private const int s_maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > s_maxPageSize) ? s_maxPageSize : value; }
    }
    public int UserId { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public string? OrderBy { get; set; }
    public bool Bookmarkers { get; set; } = false;
    public bool Bookmarkeds { get; set; } = false;
}
