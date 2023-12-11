using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Web.Models;

public class CreateUserApprovalViewModel
{
    [Required]
    public Guid Uid { get; set; }

    [Required]
    public string ApprovalData { get; set; }
}