using System;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Text;

namespace PubSubJobs.Common
{
    public class JobService : Service
    {
        private readonly string workerId;

        public JobService(IAppSettings appSettings)
        {
            workerId = appSettings.GetString(AppSettingsKeys.WorkerId);
        }
        
        public JobResponse Any(JobRequest request)
        {
            var response = new JobResponse
            {
                JobId = request.JobId,
                Result = $"Job: '{request.Description}' processed by: {workerId}"
            };
            
            Console.WriteLine(response.Dump());

            return response;
        }
        
    }
}