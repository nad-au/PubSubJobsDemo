namespace PubSubJobs.Common
{
    public static class CacheKeys
    {
        public static string JobCompleted<T>(string jobGroupId) => $"urn:{typeof(T).Name}:completed:{jobGroupId}";
    }
}