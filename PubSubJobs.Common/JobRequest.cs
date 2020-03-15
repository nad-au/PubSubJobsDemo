using ServiceStack;

namespace PubSubJobs.Common
{
    [Route("/job")]
    public class JobRequest : IReturn<JobResponse>
    {
        public string JobId { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
    }
}