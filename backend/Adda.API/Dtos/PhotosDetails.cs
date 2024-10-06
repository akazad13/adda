namespace Adda.API.Dtos;

public class PhotosDetails
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
}
