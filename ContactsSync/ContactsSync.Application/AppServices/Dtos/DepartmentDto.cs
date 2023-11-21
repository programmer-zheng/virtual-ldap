namespace ContactsSync.Application.AppServices.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public long OriginId { get; set; }

    public string DeptName { get; set; }

    public long ParentId { get; set; }
}