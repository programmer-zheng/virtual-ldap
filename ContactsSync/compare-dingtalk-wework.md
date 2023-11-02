# 钉钉与企业微信对比

## 获取access_token

|           | 钉钉                                                         | 企业微信                                                     | 说明                                                         |
| --------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| 文档地址  | [获取企业内部应用的accessToken](https://open.dingtalk.com/document/orgapp/obtain-the-access_token-of-an-internal-app) | [获取access_token](https://developer.work.weixin.qq.com/document/path/91039) |                                                              |
| 请求方式  | POST                                                         | GET                                                          |                                                              |
| 请求地址  | https://api.dingtalk.com/v1.0/oauth2/accessToken             |                                                              |                                                              |
| Query参数 | -                                                            | corpid                                                       | 企业ID                                                       |
|           | -                                                            | corpsecret                                                   | 应用的凭证密钥                                               |
| Body参数  | appKey                                                       | -                                                            | 企业内部应用的AppKey                                         |
|           | appSecret                                                    | -                                                            | 企业内部应用的AppSecret                                      |
| 返回字段  | accessToken                                                  | access_token                                                 |                                                              |
|           | expireIn                                                     | expires_in                                                   | 过期时间，单位秒                                             |
|           | code                                                         | errcode                                                      | 正常情况下返回0<br />【钉钉】正常情况下不返回，仅在错误时返回 |



## 获取部门列表

|           | 钉钉                                                         | 企业微信                                                     | 说明                                                         |
| --------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| 文档地址  | [获取部门列表](https://open.dingtalk.com/document/orgapp/obtain-the-department-list-v2) | [获取部门列表](https://developer.work.weixin.qq.com/document/path/90208) |                                                              |
| 请求方式  | POST                                                         | GET                                                          |                                                              |
| 请求地址  | https://oapi.dingtalk.com/topapi/v2/department/listsub       | https://qyapi.weixin.qq.com/cgi-bin/department/list          |                                                              |
| Query参数 | access_token                                                 | access_token                                                 |                                                              |
|           | -                                                            | id                                                           | **非必填 **部门id。获取指定部门及其下的子部门（以及子部门的子部门等等，递归）。 如果不填，默认获取全量组织架构 |
| Body参数  | dept_id                                                      | -                                                            | 父部门ID（非必填）                                           |
|           | language                                                     | -                                                            | 通讯录语言，非必填                                           |
| 返回字段  | dept_id                                                      | id                                                           | 部门ID                                                       |
|           | name                                                         | name                                                         | 部门名称                                                     |
|           | parent_id                                                    | parentid                                                     | 父部门ID                                                     |
|           | create_dept_group                                            | -                                                            | 是否同步创建一个关联此部门的企业群                           |
|           | auto_add_user                                                | -                                                            | 部门群已经创建后，有新人加入部门是否会自动加入该群           |
|           | -                                                            | department_leader                                            | 部门负责人的UserID；第三方仅通讯录应用可获取                 |
|           | -                                                            | order                                                        | 在父部门中的次序值。order值大的排序靠前。值范围是[0, 2^32)   |

## 获取部门用户/成员详情

|           | 钉钉                                                         | 企业微信                                                     | 说明                                                         |
| --------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| 文档地址  | [获取部门用户详情](https://open.dingtalk.com/document/orgapp/queries-the-complete-information-of-a-department-user) | [获取部门成员详情](https://developer.work.weixin.qq.com/document/path/90201) |                                                              |
| 请求方式  | POST                                                         | GET                                                          |                                                              |
| 请求地址  | https://oapi.dingtalk.com/topapi/v2/user/list                | https://qyapi.weixin.qq.com/cgi-bin/user/list                |                                                              |
| Query参数 | access_token                                                 | access_token                                                 |                                                              |
|           | -                                                            | department_id                                                | 获取的部门id                                                 |
| Body参数  | dept_id                                                      | -                                                            | **必填** 部门ID，可调用[获取部门列表](https://open.dingtalk.com/document/orgapp/obtain-the-department-list-v2#)获取，如果是根部门，该参数传1。只获取当前部门下的员工信息，不包含子部门内的员工。 |
|           | cursor                                                       | -                                                            | **必填** 分页查询的游标，最开始传0，后续传返回参数中的next_cursor值。 |
|           | size                                                         | -                                                            | **必填** 分页大小。                                          |
|           | order_field                                                  | -                                                            | 部门成员的排序规则，默认不传是按自定义排序（custom）：<br />**entry_asc**：代表按照进入部门的时间升序 <br />**entry_desc**：代表按照进入部门的时间降序 <br />**modify_asc**：代表按照部门信息修改时间升序 <br />**modify_desc**：代表按照部门信息修改时间降序 <br />**custom**：代表用户定义(未定义时按照拼音)排序 |
|           | contain_access_limit                                         | -                                                            | 是否返回访问受限的员工                                       |
|           | language                                                     | -                                                            | 通讯录语言                                                   |
| 返回字段  | userid                                                       | userid                                                       | 用户的userId                                                 |
|           | unionid                                                      | open_userid                                                  | 用户在当前开发者企业账号范围内的唯一标识【企业微信仅第三方应用可获取】 |
|           | name                                                         | name                                                         | 用户姓名                                                     |
|           | -                                                            | alias                                                        | 别名                                                         |
|           | avatar                                                       | avatar                                                       | 头像地址                                                     |
|           | -                                                            | thumb_avatar                                                 | 头像缩略图URL                                                |
|           | state_code                                                   | -                                                            | 国际电话区号                                                 |
|           | mobile                                                       | mobile                                                       | 手机号码                                                     |
|           | hide_mobile                                                  | -                                                            | 是否号码隐藏                                                 |
|           | telephone                                                    | telephone                                                    | 分机号                                                       |
|           | job_number                                                   | -                                                            | 员工工号                                                     |
|           | title                                                        | position                                                     | 职位                                                         |
|           | email                                                        | -                                                            | 员工邮箱                                                     |
|           | org_email                                                    | biz_mail                                                     | 员工的企业邮箱                                               |
|           | work_place                                                   | -                                                            | 办公地点                                                     |
|           | remark                                                       | -                                                            | 备注                                                         |
|           | dept_id_list                                                 | -                                                            | 所属部门id列表                                               |
|           | dept_order                                                   | order                                                        | 员工在部门中的排序                                           |
|           | extension                                                    | extattr                                                      | 扩展属性                                                     |
|           | hired_date                                                   | -                                                            | 入职时间，Unix时间戳，单位毫秒                               |
|           | active                                                       | status                                                       | 激活状态                                                     |
|           | admin                                                        | -                                                            | 是否为企业的管理员                                           |
|           | boss                                                         | -                                                            | 是否为企业的老板                                             |
|           | leader                                                       | is_leader_in_dept                                            | 是否是部门的主管                                             |
|           | exclusive_account                                            | -                                                            | 是否企业账号                                                 |
|           | -                                                            | direct_leader                                                | 直属上级UserID，返回在应用可见范围内的直属上级列表，最多有1个直属上级 |
|           | -                                                            | qr_code                                                      | 员工个人二维码                                               |
|           | -                                                            | external_profile                                             | 成员对外属性                                                 |
|           | -                                                            | external_position                                            | 对外职务，如果设置了该值，则以此作为对外展示的职务，否则以position来展示 |
|           | -                                                            | address                                                      | 地址                                                         |
|           | -                                                            | main_department                                              | 主部门，仅当应用对主部门有查看权限时返回。                   |

## 发送消息通知

|           | 钉钉                                                         | 企业微信                                                     | 说明                                                         |
| --------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| 文档地址  | [发送工作通知](https://open.dingtalk.com/document/orgapp/asynchronous-sending-of-enterprise-session-messages) | [发送应用消息](https://developer.work.weixin.qq.com/document/path/90236) |                                                              |
| 请求方式  | POST                                                         | POST                                                         |                                                              |
| 请求地址  | https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2 | https://qyapi.weixin.qq.com/cgi-bin/message/send             |                                                              |
| Query参数 | access_token                                                 | access_token                                                 |                                                              |
| Body参数  | agent_id                                                     | agentid                                                      | 企业应用的id                                                 |
|           | userid_list                                                  | touser                                                       | 接收者的userid列表<br />【钉钉：用英文逗号分割，最大用户列表长度100。】<br />【企微：多个接收者用\|分割，最多支持1000个，特殊情况：指定为"@all"，则向该企业应用的全部成员发送】 |
|           | dept_id_list                                                 | toparty                                                      | 接收者的部门id列表<br />【钉钉：最大列表长度20】<br />【企微：最多支持100个】 |
|           | to_all_user                                                  |                                                              | 是否发送给企业全部用户                                       |
|           |                                                              | totag                                                        | 指定接收消息的标签，标签ID列表，多个接收者用‘                |
|           | msg                                                          |                                                              | 消息内容，最长不超过2048个字节                               |
|           |                                                              | safe                                                         |                                                              |
|           |                                                              | enable_duplicate_check                                       | 表示是否开启重复消息检查，0表示否，1表示是，默认0            |
|           |                                                              | duplicate_check_interval                                     | 表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时   |
|           |                                                              | msgtype                                                      | 消息类型                                                     |
|           |                                                              | content                                                      | 消息内容，最长不超过2048个字节，超过将截断**（支持id转译）** |
|           |                                                              |                                                              |                                                              |

