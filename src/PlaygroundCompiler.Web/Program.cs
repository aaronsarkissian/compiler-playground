using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace PlaygroundService
{
    public class Program
    {
        private static readonly string _environmentName;
        private static readonly IConfigurationRoot _configuration;

        static Program()
        {
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            if (string.IsNullOrWhiteSpace(_environmentName))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _configuration["Environment"]);
            }
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(_configuration.GetSection("ApplicationUrl").Value);
                });
    }
}
