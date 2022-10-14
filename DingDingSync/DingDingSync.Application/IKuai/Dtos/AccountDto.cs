namespace DingDingSync.Application.IKuai.Dtos
{

    public class AccountDto : AccountCommon
    {
        public AccountDto(string _username, string _password, string _name) : base(_username, _password, _name)
        {
        }

        ///<summary>
        /// id
        /// </summary>
        public int id { get; set; }
    }

    public class AccountCommon
    {

        public AccountCommon(string _username, string _password, string _name)
        {
            username = _username;
            passwd = _password;
            name = _name;

            enabled = "yes";
            share = 2;
            ppptype = "any";
            bind_ifname = "any";
            bind_vlanid = "0";
            auto_vlanid = 0;
            packages = 0;
            last_conntime = 0;
            auto_mac = 1;
            ip_addr = string.Empty;
            mac = string.Empty;

            comment = string.Empty;
            address = string.Empty;
            cardid = string.Empty;
            phone = string.Empty;

        }

        ///<summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        ///<summary>
        /// 拨号类型(any、pppoe、pptp、l2tp、ovpn、web)
        /// </summary>
        public string ppptype { get; set; }

        ///<summary>
        /// 密码
        /// </summary>    
        public string passwd { get; set; }

        ///<summary>
        /// 套餐类型(对应套餐功能的id, 0表示自定义)
        /// </summary>
        public int packages { get; set; }

        ///<summary>
        /// 自动绑定MAC，0-关闭，1-开启
        /// </summary>
        public int auto_mac { get; set; }

        ///<summary>
        /// 0-关闭，1-开启 (iK-Router 3.2.0+ 专有)
        /// </summary>
        public int auto_vlanid { get; set; }

        ///<summary>
        /// 绑定网卡
        /// </summary>
        public string bind_ifname { get; set; }

        ///<summary>
        /// 绑定vlanid(0表示不绑定；支持格式2000/2000.400；范围1~4090)
        /// </summary>
        public string bind_vlanid { get; set; }

        ///<summary>
        /// 创建时间
        /// </summary>
        public int create_time { get; set; }

        ///<summary>
        /// 下载
        /// </summary>
        public int download { get; set; }

        ///<summary>
        /// 状态 yes,no
        /// </summary>
        public string enabled { get; set; }

        ///<summary>
        /// 过期日期
        /// </summary>
        public int expires { get; set; }

        ///<summary>
        /// 最后一次连接时间戳，0表示显示为空 (iK-Router 3.2.0+ 专有)
        /// </summary>
        public int last_conntime { get; set; }

        ///<summary>
        /// 共享数
        /// </summary>
        public int share { get; set; }

        ///<summary>
        /// 开始时期
        /// </summary>
        public int start_time { get; set; }

        ///<summary>
        /// upload
        /// </summary>
        public int upload { get; set; }

        #region 以下为非必填

        ///<summary>
        /// 姓名
        /// </summary>    
        public string name { get; set; }


        ///<summary>
        /// 手机
        /// </summary>    
        public string phone { get; set; }


        ///<summary>
        /// 住址
        /// </summary>    
        public string address { get; set; }

        ///<summary>
        /// 证件号码(
        /// </summary>    
        public string cardid { get; set; }

        ///<summary>
        /// 备注
        /// </summary>
        public string comment { get; set; }

        ///<summary>
        /// 固定IP
        /// </summary>
        public string ip_addr { get; set; }

        ///<summary>
        /// 绑定MAC
        /// </summary>
        public string mac { get; set; }

        #endregion
    }
}
