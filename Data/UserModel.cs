using System.Data;

namespace Inventory_Management.Data;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public bool HasInitialPassword { get; set; } = true;
    public UserType UType { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
    public Guid RegisterdBy { get; set; }
}
