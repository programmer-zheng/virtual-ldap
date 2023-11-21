using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Web.Models;

public class LdapRequestViewModel
{
    [Required]
    public string UserName { get; set; }

    public string Password { get; set; }
}