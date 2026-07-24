namespace Adda.API.Tests.Fixtures;

/// <summary>
/// Builder for creating test users with fluent API
/// </summary>
public class UserBuilder
{
    private int _id = 1;
    private string _userName = "testuser";
    private string _email = "test@example.com";
    private string _knownAs = "Test User";
    private string _gender = "male";
    private string _introduction = "Hello";
    private string _lookingFor = "Looking for friends";
    private string _interests = "Testing";
    private string _city = "Test City";
    private string _country = "Test Country";
    private DateTime _dateOfBirth = new(1990, 1, 1);

    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithUserName(string userName)
    {
        _userName = userName;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithKnownAs(string knownAs)
    {
        _knownAs = knownAs;
        return this;
    }

    public UserBuilder WithGender(string gender)
    {
        _gender = gender;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Id = _id,
            UserName = _userName,
            Email = _email,
            KnownAs = _knownAs,
            Gender = _gender,
            Introduction = _introduction,
            LookingFor = _lookingFor,
            Interests = _interests,
            city = _city,
            Country = _country,
            DateOfBirth = _dateOfBirth,
            Created = DateTime.Now,
            LastActive = DateTime.Now,
        };
    }
}

/// <summary>
/// Builder for creating test messages
/// </summary>
public class MessageBuilder
{
    private int _id = 1;
    private int _senderId = 1;
    private int _recipientId = 2;
    private string _content = "Test message";
    private DateTime _dateSent = DateTime.Now;
    private bool _isRead = false;
    private bool _senderDeleted = false;
    private bool _recipientDeleted = false;

    public MessageBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public MessageBuilder WithSenderId(int senderId)
    {
        _senderId = senderId;
        return this;
    }

    public MessageBuilder WithRecipientId(int recipientId)
    {
        _recipientId = recipientId;
        return this;
    }

    public MessageBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public Message Build()
    {
        return new Message
        {
            Id = _id,
            SenderId = _senderId,
            RecipientId = _recipientId,
            Content = _content,
            MessageSent = _dateSent,
            IsRead = _isRead,
            SenderDeleted = _senderDeleted,
            RecipientDeleted = _recipientDeleted,
        };
    }
}

/// <summary>
/// Builder for creating test photos
/// </summary>
public class PhotoBuilder
{
    private int _id = 1;
    private int _userId = 1;
    private string _url = "https://test.com/photo.jpg";
    private string _publicId = "test_public_id";
    private string _description = "Test Photo";
    private DateTime _dateAdded = DateTime.Now;
    private bool _isMain = false;
    private bool _isApproved = true;

    public PhotoBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PhotoBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public PhotoBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }

    public PhotoBuilder WithPublicId(string publicId)
    {
        _publicId = publicId;
        return this;
    }

    public PhotoBuilder AsMain(bool isMain = true)
    {
        _isMain = isMain;
        return this;
    }

    public Photo Build()
    {
        return new Photo
        {
            Id = _id,
            UserId = _userId,
            Url = _url,
            Description = _description,
            PublicId = _publicId,
            DateAdded = _dateAdded,
            IsMain = _isMain,
            IsApproved = _isApproved,
        };
    }
}

/// <summary>
/// Factory for creating common test data
/// </summary>
public class TestDataFactory
{
    public static User CreateUser(int id = 1, string userName = null!)
    {
        return new UserBuilder()
            .WithId(id)
            .WithUserName(userName ?? $"user{id}")
            .Build();
    }

    public static Message CreateMessage(int id = 1, int senderId = 1, int recipientId = 2)
    {
        return new MessageBuilder()
            .WithId(id)
            .WithSenderId(senderId)
            .WithRecipientId(recipientId)
            .Build();
    }

    public static Photo CreatePhoto(int id = 1, int userId = 1)
    {
        return new PhotoBuilder()
            .WithId(id)
            .WithUserId(userId)
            .Build();
    }

    public static List<User> CreateUserList(int count = 5)
    {
        var users = new List<User>();
        for (int i = 1; i <= count; i++)
        {
            users.Add(CreateUser(i, $"user{i}"));
        }
        return users;
    }

    public static List<Message> CreateMessageList(int count = 5, int senderId = 1, int recipientId = 2)
    {
        var messages = new List<Message>();
        for (int i = 1; i <= count; i++)
        {
            messages.Add(new MessageBuilder()
                .WithId(i)
                .WithSenderId(senderId)
                .WithRecipientId(recipientId)
                .WithContent($"Test message {i}")
                .Build());
        }
        return messages;
    }
}
