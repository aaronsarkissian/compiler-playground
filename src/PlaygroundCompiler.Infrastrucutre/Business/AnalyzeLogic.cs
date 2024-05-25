using Microsoft.Extensions.Options;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Configurations;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business
{
    public class AnalyzeLogic : IAnalyzeLogic
    {
        private readonly ICodeFileRepository _codeFileRepository;
        private readonly ICodeBuildRepository _codeBuildRepository;
        private readonly IProcessRepository _processRepository;

        private readonly IOptions<VmConfiguration> _vmConfiguration;
        private readonly IOptions<AnalyzeConfiguration> _analyzeConfiguration;
        private readonly IOptions<RunTimeouts> _runTimeouts;
        private readonly string baseUrl;

        public AnalyzeLogic(ICodeBuildRepository codeBuildRepository, ICodeFileRepository codeFileRepository, IProcessRepository processRepository,
            IOptions<VmConfiguration> vmConfiguration, IOptions<AnalyzeConfiguration> analyzeConfiguration, IOptions<RunTimeouts> runTimeouts)
        {
            _codeFileRepository = codeFileRepository;
            _codeBuildRepository = codeBuildRepository;
            _processRepository = processRepository;
            _vmConfiguration = vmConfiguration;
            _analyzeConfiguration = analyzeConfiguration;
            _runTimeouts = runTimeouts;
            baseUrl = _vmConfiguration.Value.BaseUrl;
        }

        public async Task<AnalyzeResult[]> Analyze(Code code)
        {
            CompilerLanguages analyzerNameBase = _analyzeConfiguration.Value.Build;
            CompilerLanguages builderMachineBase = _vmConfiguration.Value.ImageNames;
            CompilerLanguages analyzerCommandBase = _analyzeConfiguration.Value.Output;
            CompilerLanguages runTimeoutBase = _runTimeouts.Value.Times;

            if (string.IsNullOrEmpty(code.Id))
            {
                code.Id = $"{Guid.NewGuid()}-analyze";
            }

            ProjectCreationStrategy projectCreationStrategy = code.Language switch
            {
                Languages.Py => new CommonProjectCreationStrategy(_processRepository, analyzerNameBase.Py, $"{baseUrl}{builderMachineBase.Py}", analyzerCommandBase.Py, runTimeoutBase.Py),
                Languages.Cpp => new CommonProjectCreationStrategy(_processRepository, analyzerNameBase.Cpp, $"{baseUrl}{builderMachineBase.Cpp}", analyzerCommandBase.Cpp, runTimeoutBase.Cpp),
                Languages.Js or Languages.Node => new CommonProjectCreationStrategy(_processRepository, analyzerNameBase.Node, $"{baseUrl}{builderMachineBase.Node}", analyzerCommandBase.Node, runTimeoutBase.Node),
                Languages.Cs => new CommonProjectCreationStrategy(_processRepository, analyzerNameBase.Cs, $"{baseUrl}{builderMachineBase.Cs}", analyzerCommandBase.Cs, runTimeoutBase.Cs),
                Languages.Java => new JavaProjectCreationStrategy(_processRepository, analyzerNameBase.Java, $"{baseUrl}{builderMachineBase.Java}", analyzerCommandBase.Java, runTimeoutBase.Java),
                Languages.Go => new CommonProjectCreationStrategy(_processRepository, analyzerNameBase.Go, $"{baseUrl}{builderMachineBase.Go}", analyzerCommandBase.Go, runTimeoutBase.Go),
                _ => throw new NotImplementedException(),
            };
            await _codeFileRepository.CreateProjectFiles(code, projectCreationStrategy);
            _codeBuildRepository.Analyze(code);
            AnalyzeResult[] output = await _codeBuildRepository.GetAnalyzeResult(code);
            return output;
        }
    }
}
