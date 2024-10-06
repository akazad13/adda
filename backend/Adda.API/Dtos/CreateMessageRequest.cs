namespace Adda.API.Dtos;

public class CreateMessageRequest
{
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
    public string Content { get; set; }
}
