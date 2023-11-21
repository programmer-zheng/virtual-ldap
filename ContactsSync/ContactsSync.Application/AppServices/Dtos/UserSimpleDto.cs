namespace ContactsSync.Application.AppServices.Dtos;

public class UserSimpleDto
{
    public Guid Uid { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public bool IsPasswordInited { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsVpnEnabled { get; set; }
}