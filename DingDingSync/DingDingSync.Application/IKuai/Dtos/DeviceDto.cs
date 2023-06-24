using System.Collections.Generic;

namespace DingDingSync.Application.IKuai.Dtos
{
    public class DeviceDto
    {

        /// <summary>
        /// 设备ID
        /// </summary>
        public string dev_id { get; set; }

        /// <summary>
        /// 设备友好名称
        /// </summary>
        public string dev_remark { get; set; }

        /// <summary>
        /// 平台合作商，在本设备上可以调用的 api_id 列表
        /// </summary>
        public List<string> open_apis { get; set; }

        /// <summary>
        /// 设备所有者手机号
        /// </summary>
        public string owner_mobile { get; set; }

        /// <summary>
        /// 设备所有者QQ号
        /// </summary>
        public string owner_qq { get; set; }
    }
}
