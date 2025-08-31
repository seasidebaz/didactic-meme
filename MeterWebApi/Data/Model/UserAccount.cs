using System.ComponentModel.DataAnnotations;

namespace Data.Model;

public class UserAccount
{
    [Required]
    public int AccountId { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}
