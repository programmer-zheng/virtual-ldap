using ContactsSync.Domain.Shared;

namespace ContactsSync.Application.Contracts.Dtos;

public class LdapDeptUserDto
{
    private string _password;

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Position { get; set; }

    public string Mobile { get; set; }

    public string Password
    {
        get
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_password)) return _password.DesDecrypt().ToMd5();
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }
        set => _password = value;
    }


    public string Name { get; set; }

    public string Email { get; set; }

    public string BizEmail { get; set; }

    public string Avatar { get; set; }

    public IEnumerable<Guid> Departments { get; set; }
}