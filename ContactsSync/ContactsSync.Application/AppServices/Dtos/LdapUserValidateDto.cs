namespace ContactsSync.Application.AppServices.Dtos;

public class LdapUserValidateDto
{
    public long Uid { get; set; }

    public string Password { get; set; }

    public bool IsPasswordInited { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsVpnEnabled { get; set; }
}