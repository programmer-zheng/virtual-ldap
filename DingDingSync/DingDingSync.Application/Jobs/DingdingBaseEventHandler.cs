using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Core.Entities;
using DingTalk.Api.Response;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using TinyPinyin;

namespace DingDingSync.Application.Jobs
{
    public abstract class DingdingBaseEventHandler
    {
        protected readonly IRepository<DepartmentEntity, long> _departmentRepository;
        protected readonly IRepository<UserEntity, string> _userRepository;
        protected readonly IRepository<UserDepartmentsRelationEntity, string> _deptUserRelaRepository;
        protected readonly IUserAppService _userAppService;
        protected readonly IDepartmentAppService _departmentAppService;
        protected readonly IDingdingAppService _dingdingAppService;
        protected readonly IObjectMapper _objectMapper;
        protected readonly IConfiguration _configuration;
        protected readonly IIkuaiAppService _iKuaiAppService;
        protected readonly ILogger _logger;

        public DingdingBaseEventHandler(
            IRepository<DepartmentEntity, long> departmentRepository,
            IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            IUserAppService userAppService,
            IDepartmentAppService departmentAppService,
            IDingdingAppService dingdingAppService,
            IObjectMapper objectMapper,
            IConfiguration configuration,
            IIkuaiAppService iKuaiAppService,
            ILogger logger)
        {
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _deptUserRelaRepository = deptUserRelaRepository;
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
            _configuration = configuration;
            _iKuaiAppService = iKuaiAppService;
            _logger = logger;
        }

        public abstract void Do(string msg);

        /// <summary>
        /// 判断是否为管理人员
        /// </summary>
        /// <param name="dingdingUser"></param>
        /// <returns></returns>
        public virtual bool IsAdmin(OapiV2UserGetResponse.UserGetResponseDomain dingdingUser)
        {
            return dingdingUser.Admin ||
                   (dingdingUser.LeaderInDept != null && dingdingUser.LeaderInDept.Count(t => t.Leader) > 0);
        }
    }
}