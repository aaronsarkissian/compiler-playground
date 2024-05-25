using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business
{
    public class CommonProjectCreationStrategy : ProjectCreationStrategy
    {
        private string BuilderMachineName { get; set; }
        private string CompilerName { get; set; }
        private string OutputCommand { get; set; }
        private string RunTimeout { get; set; }

        public CommonProjectCreationStrategy(IProcessRepository processRepository, string compilerName, string builderMachine, string outputCommand, string runTimeout) : base(processRepository)
        {
            CompilerName = compilerName;
            BuilderMachineName = builderMachine;
            OutputCommand = outputCommand;
            RunTimeout = runTimeout;
        }

        public override async Task<bool> Prepare(Code code)
        {
            code.MetaInfo.Compiler = CompilerName;
            code.MetaInfo.BuilderMachineName = BuilderMachineName;
            code.MetaInfo.RunTimeout = RunTimeout;

            foreach (var item in code.InitialSourceCode.Select((value, index) => new { value, index }))
            {
                code.SourceCodeFiles.Add($"file{item.index}", item.value);
            }

            if (!string.IsNullOrEmpty(OutputCommand))
            {
                code.OutputCommand = OutputCommand;
            }

            return await base.Prepare(code);
        }
    }
}
