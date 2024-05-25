using PlaygroundService.Infrastrucutre.Configurations;
using PlaygroundService.ServiceConfigurations;

namespace PlaygroundService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();

            services.AddLogicRepositories(Configuration);
            services.AddCodeRepositories(Configuration);

            services.AddSwagger(Configuration);
            services.AddOptions();

            services.Configure<RunTimeouts>(Configuration.GetSection("RunTimeouts"));
            services.Configure<VmConfiguration>(Configuration.GetSection("Vm"));
            services.Configure<JudgeVmConfiguration>(Configuration.GetSection("JudgeVm"));
            services.Configure<CompilerConfiguration>(Configuration.GetSection("Compiler"));
            services.Configure<JudgeCompilerConfiguration>(Configuration.GetSection("JudgeCompiler"));
            services.Configure<AnalyzeConfiguration>(Configuration.GetSection("Analyze"));
            services.Configure<RootConfiguration>(Configuration);

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSLSwagger(Configuration);
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });
        }
    }
}
