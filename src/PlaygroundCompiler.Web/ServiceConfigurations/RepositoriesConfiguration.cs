using PlaygroundCompiler.Infrastrucutre.Services;
using PlaygroundCompiler.Infrastrucutre.Services.Interfaces;
using PlaygroundService.Infrastrucutre.Business;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Repositories;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;

namespace PlaygroundService.ServiceConfigurations
{
    public static class RepositoriesConfiguration
    {
        public static IServiceCollection AddLogicRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPlaygroundLogic, PlaygroundLogic>();
            services.AddScoped<IJudgeLogic, JudgeLogic>();
            services.AddScoped<IAnalyzeLogic, AnalyzeLogic>();
            services.AddScoped<IBlobService, BlobService>();

            return services;
        }
        public static IServiceCollection AddCodeRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICodeBuildRepository, CodeBuildRepository>();
            services.AddScoped<ICodeFileRepository, CodeFileRepository>();
            services.AddScoped<IProcessRepository, UnixProcessRepository>();

            return services;
        }
    }
}
