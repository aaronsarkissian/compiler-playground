using Microsoft.Extensions.Options;
using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Configurations;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business
{
    public class PlaygroundLogic : IPlaygroundLogic
    {
        private readonly ICodeFileRepository _codeFileRepository;
        private readonly ICodeBuildRepository _codeBuildRepository;
        private readonly IProcessRepository _processRepository;

        private readonly IOptions<VmConfiguration> _vmConfiguration;
        private readonly IOptions<CompilerConfiguration> _compilerConfiguration;
        private readonly IOptions<RunTimeouts> _runTimeouts;
        private readonly string baseUrl;

        public PlaygroundLogic(ICodeBuildRepository codeBuildRepository, ICodeFileRepository codeFileRepository, IProcessRepository processRepository,
            IOptions<VmConfiguration> vmConfiguration, IOptions<CompilerConfiguration> compilerConfiguration, IOptions<RunTimeouts> runTimeouts)
        {
            _codeFileRepository = codeFileRepository;
            _codeBuildRepository = codeBuildRepository;
            _processRepository = processRepository;
            _vmConfiguration = vmConfiguration;
            _compilerConfiguration = compilerConfiguration;
            _runTimeouts = runTimeouts;
            baseUrl = _vmConfiguration.Value.BaseUrl;
        }

        public async Task<CompilerOutput> BuildAndRun(Code code)
        {
            CompilerLanguages compilerNameBase = _compilerConfiguration.Value.Build;
            CompilerLanguages builderMachineBase = _vmConfiguration.Value.ImageNames;
            CompilerLanguages outputCommandBase = _compilerConfiguration.Value.Output;
            CompilerLanguages runTimeoutBase = _runTimeouts.Value.Times;

            if (string.IsNullOrEmpty(code.Id))
            {
                code.Id = Guid.NewGuid().ToString();
            }

            ProjectCreationStrategy projectCreationStrategy = code.Language switch
            {
                Languages.Cpp => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Cpp, baseUrl + builderMachineBase.Cpp, outputCommandBase.Cpp, runTimeoutBase.Cpp),
                Languages.C => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.C, baseUrl + builderMachineBase.C, outputCommandBase.C, runTimeoutBase.C),
                Languages.Cs => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Cs, baseUrl + builderMachineBase.Cs, outputCommandBase.Cs, runTimeoutBase.Cs),
                Languages.Java => new JavaProjectCreationStrategy(_processRepository, compilerNameBase.Java, baseUrl + builderMachineBase.Java, outputCommandBase.Java, runTimeoutBase.Java),
                Languages.Py => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Py, baseUrl + builderMachineBase.Py, outputCommandBase.Py, runTimeoutBase.Py),
                Languages.Php => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Php, baseUrl + builderMachineBase.Php, outputCommandBase.Php, runTimeoutBase.Php),
                Languages.Rb => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Rb, baseUrl + builderMachineBase.Rb, outputCommandBase.Rb, runTimeoutBase.Rb),
                Languages.Swift => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Swift, baseUrl + builderMachineBase.Swift, outputCommandBase.Swift, runTimeoutBase.Swift),
                Languages.Node or Languages.Js => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Node, baseUrl + builderMachineBase.Node, outputCommandBase.Node, runTimeoutBase.Node),
                Languages.Sql => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Sql, baseUrl + builderMachineBase.Sql, outputCommandBase.Sql, runTimeoutBase.Sql),
                Languages.Go => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Go, baseUrl + builderMachineBase.Go, outputCommandBase.Go, runTimeoutBase.Go),
                Languages.R => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.R, baseUrl + builderMachineBase.R, outputCommandBase.R, runTimeoutBase.R),
                Languages.Ts => new CommonProjectCreationStrategy(_processRepository, compilerNameBase.Ts, baseUrl + builderMachineBase.Ts, outputCommandBase.Ts, runTimeoutBase.Ts),
                _ => throw new NotImplementedException(),
            };
            await _codeFileRepository.CreateProjectFiles(code, projectCreationStrategy);
            _codeBuildRepository.BuildAndRun(code);
            CompilerOutput output = await _codeBuildRepository.GetResult(code.Id);
            return output;
        }
    }
}
