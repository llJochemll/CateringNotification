using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace PXLCateringNotification.Data
{
    public class Subscription : TableEntity
    {
        public class UserSubscription
        {
            public enum Campus
            {
                DiepenBeek,
                ElfdeLinie,
                VilderStraat
            }

            public class NotificationSetting
            {
                public Campus Campus { get; set; }
                public int Hour { get; set; }
                public int Minute { get; set; }
            }

            public List<NotificationSetting> Monday { get; set; }
            public List<NotificationSetting> Tuesday { get; set; }
            public List<NotificationSetting> Wednesday { get; set; }
            public List<NotificationSetting> Thursday { get; set; }
            public List<NotificationSetting> Friday { get; set; }
        }

        public UserSubscription CurrentSubscription { get; set; }
        public UserSubscription RequestedSubscription { get; set; }
    }
}
