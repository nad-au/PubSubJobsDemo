using System;
using Funq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PubSubJobs.Common;
using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;

namespace PubSubJobs.Subscriber
{
    public class AppHost : AppHostBase
    {
        public AppHost(IHostingEnvironment hostingEnvironment) : base("PubSubJobs", typeof(JobService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            AppSettings = new NetCoreAppSettings(configurationBuilder.Build());

            var redisFactory = new PooledRedisClientManager(
                AppSettings.GetString(AppSettingsKeys.RedisConnection));
            var mqServer = new RedisMqServer(redisFactory, retryCount:AppSettings.Get<int>(AppSettingsKeys.RedisRetries));

            mqServer.RegisterHandler<JobRequest>(base.ExecuteMessage);
            
            AfterInitCallbacks.Add(host => {
                mqServer.Start();
            });
        }
    }
}