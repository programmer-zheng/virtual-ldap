﻿using System;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 加入企业后用户激活
    /// </summary>
    public class UserActiveOrgEventHandler : DingdingBaseEventHandler
    {

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<UserActiveOrgEvent>(msg);
            Console.WriteLine($"用户激活：{string.Join(",", eventinfo.ID)}");
        }
    }
}