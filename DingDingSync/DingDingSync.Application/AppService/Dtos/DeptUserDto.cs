using Abp.Extensions;

namespace DingDingSync.Application.AppService.Dtos
{
    public class DeptUserDto
    {
        public string Userid { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

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

        public string Mobile { get; set; }

        public string JobNumber { get; set; }

        public DateTime? HiredDate { get; set; }

        public string HireDateStr
        {
            get { return HiredDate.HasValue ? HiredDate.Value.ToString("yyyy-MM-dd") : "-"; }
        }

        public string UnionId { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }

        public string WorkPlace { get; set; }

        public bool Active { get; set; }

        public string Position { get; set; }

        public List<long> Department { get; set; }
        public bool AccountEnabled { get; internal set; }
        public bool VpnAccountEnabled { get; internal set; }
    }
}