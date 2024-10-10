namespace Adda.API.Dtos;

public class PhotoResponse
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public string? PublicId { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
}
