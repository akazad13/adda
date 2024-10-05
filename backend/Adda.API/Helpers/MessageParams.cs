namespace Adda.API.Helpers;

public class MessageParams
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
    public string MessageContainer { get; set; } = "unread";
}
