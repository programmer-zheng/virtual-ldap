using System.Collections.Generic;

namespace DingDingSync.Application.AppService.Dtos
{
    public class ManageDto
    {
        public UserSimpleDto User { get; set; }

        public List<DeptUserDto> UserList { get; set; }
    }
}
