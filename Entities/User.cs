using Microsoft.AspNetCore.Identity;

public  class User : IdentityUser
{
    public Guid? ItemId { get; set; }

    public User() {}
    public User(string Id, string userName, string phoneNumber, string email, Guid? itemId)
    {
        this.Id = Id;
        UserName = userName;
        PhoneNumber = phoneNumber;
        Email = email;
        ItemId = itemId;
    }
}