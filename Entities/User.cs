using Microsoft.AspNetCore.Identity;

public  class User : IdentityUser
{
    public string? Name { get; set; }
    public Guid? ItemId { get; set; }

    public User() {}
    public User(string Id, string name, string userName, string phoneNumber, string email, Guid? itemId)
    {
        this.Id = Id;
        Name = name;
        UserName = userName;
        PhoneNumber = phoneNumber;
        Email = email;
        ItemId = itemId;
    }
}