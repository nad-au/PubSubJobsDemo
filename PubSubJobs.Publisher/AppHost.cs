using System;
using Funq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PubSubJobs.Common;
using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;

namespace PubSubJobs.Publisher
{
    public class AppHost : AppHostBase
    {
        private readonly IApplicationLifetime appLifetime;

        public AppHost(IHostingEnvironment hostingEnvironment, IApplicationLifetime appLifetime) : base("PubSubJobs", typeof(AppHost).Assembly)
        {
            this.appLifetime = appLifetime;
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

            var cacheClient = redisFactory.GetCacheClient();
            var jobGroupId = AppSettings.GetString(AppSettingsKeys.JobGroupId); 
            var jobCompletedCacheKey = CacheKeys.JobCompleted<JobResponse>(jobGroupId);
            var numberOfJobs = AppSettings.Get<int>(AppSettingsKeys.NumberOfJobs);

            cacheClient.Remove(jobCompletedCacheKey);

            mqServer.RegisterHandler<JobResponse>(m => {  
                Console.WriteLine("Received: " + m.GetBody().Result);
                
                cacheClient.Increment(jobCompletedCacheKey, 1);
                if (cacheClient.Get<int>(jobCompletedCacheKey) >= numberOfJobs)
                {
                    appLifetime.StopApplication();
                }
                
                return null;
            });
            
            AfterInitCallbacks.Add(host => {
                mqServer.Start();
        
                var mqClient = mqServer.CreateMessageQueueClient();
                for (var i = 1; i <= numberOfJobs; i++)
                {
                    mqClient.Publish(new JobRequest
                    {
                        JobId = Guid.NewGuid().ToString(),
                        GroupId = jobGroupId,
                        Description = $"Job {i}"
                    });
                }
            });
        }
    }
}