namespace ContactsSync.Application.AppServices.Dtos;

public class DeptUserDto
{
    public string Uid { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string BizEmail { get; set; }

    public string Mobile { get; set; }

    private string _password;

    public string Password
    {
        get
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_password))
                {
                    return _password.DesDecrypt().ToMd5();
                }
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }
        set { _password = value; }
    }

    public string Avatar { get; set; }

    public bool IsPasswordInited { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsVpnEnabled { get; set; }
}