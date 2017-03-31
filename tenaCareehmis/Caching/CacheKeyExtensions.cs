using eHMISWebApi.Models;

namespace eHMISWebApi.Caching
{
    public static class CacheKeyExtensions
    {
        public static string GetKey(this AnalyticsParameters p)
        {
            var key = $"{p.UserId}_{p.reportType}";
            
            return key;
        }
    }
}