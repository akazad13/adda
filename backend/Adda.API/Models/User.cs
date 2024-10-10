using Microsoft.AspNetCore.Identity;

namespace Adda.API.Models;

public class User : IdentityUser<int>
{
    public string? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string? Introduction { get; set; }
    public string? LookingFor { get; set; }
    public string? Interests { get; set; }
    public string? city { get; set; }
    public string? Country { get; set; }
    public ICollection<Photo>? Photos { get; set; }
    public ICollection<Bookmark>? Bookmarkers { get; }
    public ICollection<Bookmark>? Bookmarkeds { get; }
    public ICollection<Message>? MessagesSent { get; }
    public ICollection<Message>? MessagesReceived { get; }
    public ICollection<UserRole>? UserRoles { get; }
}
