namespace ContactsSync.Application.Contracts.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public long OriginId { get; set; }

    public string Name { get; set; }

    public long ParentId { get; set; }
}