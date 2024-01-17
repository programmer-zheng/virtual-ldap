using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Web.Models;

public class LdapRequestViewModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}