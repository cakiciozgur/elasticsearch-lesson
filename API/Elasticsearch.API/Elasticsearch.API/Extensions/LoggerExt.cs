
namespace Elasticsearch.API.Extensions
{
    public static class LoggerExt
    {
        public static void AddLogger(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<ApplicationLogs>>();
            //Add Singleton if you want to use Generic class logger in place of ILogger<T>
            services.AddSingleton(typeof(ILogger), logger);
        }

        public class ApplicationLogs
        {
        }
    }
}
