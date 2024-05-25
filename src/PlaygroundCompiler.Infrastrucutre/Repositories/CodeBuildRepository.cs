using Microsoft.Extensions.Options;
using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Configurations;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Repositories
{
    public class CodeBuildRepository : ICodeBuildRepository
    {
        private readonly ICodeFileRepository _codeFileRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IOptions<RootConfiguration> _options;
        const string sqlPass = "POSTGRES_PASSWORD=mysecretpassword";

        public CodeBuildRepository(ICodeFileRepository codeFileRepository, IProcessRepository processRepository,
            IOptions<RootConfiguration> options)
        {
            _codeFileRepository = codeFileRepository;
            _processRepository = processRepository;
            _options = options;
        }

        public void BuildAndRun(Code code)
        {
            try
            {
                string lang = code.Language.ToString().ToLower();
                string args;

                if (code.Language == Languages.Sql)
                {
                    args = $" {_options.Value.Timeout}s -it -v \"{code.MetaInfo.DirecotryPath}\":/usercode -e {sqlPass} -d {code.MetaInfo.BuilderMachineName}";
                }
                else
                {
                    args = $" {_options.Value.Timeout}s -it -v \"{code.MetaInfo.DirecotryPath}\":/usercode {code.MetaInfo.BuilderMachineName} " +
                    $"/usercode/runner.sh {code.MetaInfo.RunTimeout}s \"{code.MetaInfo.Compiler}\" *.{lang} \"{code.OutputCommand}\"";
                }

                _processRepository.ProcessStart("/app/scripts/DockerTimeout.sh", waitForExit: true, args: args);
            }
            catch (Exception e)
            {
                Console.WriteLine("The BuildAndRunRepo process failed: {0}, withLang: {1}", e.ToString(), code.Language);
            }

        }

        public void BuildAndTest(JudgeCode judgeCode)
        {
            string lang = judgeCode.Language.ToString().ToLower();
            string args = string.Empty;

            switch (judgeCode.JudgeTypes)
            {
                case JudgeTypes.UnitTest:
                    if (judgeCode.Language == Languages.Py || judgeCode.Language == Languages.Php || judgeCode.Language == Languages.Rb || judgeCode.Language == Languages.Node)
                    {
                        args = $" {_options.Value.Timeout}s -it -v \"{judgeCode.MetaInfo.DirecotryPath}\":/usercode --network none {judgeCode.MetaInfo.BuilderMachineName} " +
                        $"/usercode/judgeUnitRunner.sh {judgeCode.MetaInfo.RunTimeout}s \"{judgeCode.MetaInfo.Compiler}\" Test.{lang} \"{judgeCode.OutputCommand}\"";
                    }
                    else
                    {
                        args = $" {_options.Value.Timeout}s -it -v \"{judgeCode.MetaInfo.DirecotryPath}\":/usercode --network none {judgeCode.MetaInfo.BuilderMachineName} " +
                        $"/usercode/judgeUnitRunner.sh {judgeCode.MetaInfo.RunTimeout}s \"{judgeCode.MetaInfo.Compiler}\" *.{lang} \"{judgeCode.OutputCommand}\"";
                    }
                    break;
                case JudgeTypes.InputOutput: // --network none
                    if (judgeCode.Language == Languages.Py)
                    {
                        args = $" {_options.Value.Timeout}s -it -v \"{judgeCode.MetaInfo.DirecotryPath}\":/usercode --network none {judgeCode.MetaInfo.BuilderMachineName} " +
                        $"/usercode/judgeIoRunnerPython.sh {judgeCode.MetaInfo.RunTimeout}s \"{judgeCode.MetaInfo.Compiler}\" *.{lang} \"{judgeCode.OutputCommand}\"";
                    }
                    else if (judgeCode.Language == Languages.Sql)
                    {
                        args = $" {_options.Value.Timeout}s -it -v \"{judgeCode.MetaInfo.DirecotryPath}\":/usercode -e {sqlPass} -d {judgeCode.MetaInfo.BuilderMachineName} ";
                    }
                    else
                    {
                        args = $" {_options.Value.Timeout}s -it -v \"{judgeCode.MetaInfo.DirecotryPath}\":/usercode --network none {judgeCode.MetaInfo.BuilderMachineName} " +
                        $"/usercode/judgeIoRunner.sh {judgeCode.MetaInfo.RunTimeout}s \"{judgeCode.MetaInfo.Compiler}\" *.{lang} \"{judgeCode.OutputCommand}\"";
                    }
                    break;
                default:
                    break;
            }

            _processRepository.ProcessStart("/app/scripts/DockerTimeout.sh", waitForExit: true, args: args);
        }

        public void Analyze(Code code)
        {
            string lang = code.Language.ToString().ToLower();
            string args = code.Language switch
            {
                Languages.Cpp or Languages.Py or Languages.Js or Languages.Node or Languages.Cs or Languages.Java or Languages.Go => $" {_options.Value.Timeout}s -it -v \"{code.MetaInfo.DirecotryPath}\":/usercode --network none {code.MetaInfo.BuilderMachineName} " +
                                   $"/usercode/analyze.sh {code.MetaInfo.RunTimeout}s \"{code.MetaInfo.Compiler}\" *.{lang} \"{code.OutputCommand}\"",
                _ => throw new NotImplementedException(),
            };
            _processRepository.ProcessStart("/app/scripts/DockerTimeout.sh", waitForExit: true, args: args);
        }

        public async Task<CompilerOutput> GetResult(string codeId)
        {
            return await _codeFileRepository.GetOutputItem($"/usercode/{codeId}");
        }

        public async Task<JudgeOutput> GetJudgeUnitResult(string judgeCodeId)
        {
            return await _codeFileRepository.GetJudgeUnitOutput($"/usercode/{judgeCodeId}");
        }

        public async Task<JudgeOutput> GetJudgeIoResult(string judgeCodeId)
        {
            return await _codeFileRepository.GetJudgeIoOutput($"/usercode/{judgeCodeId}");
        }

        public async Task<AnalyzeResult[]> GetAnalyzeResult(Code code)
        {
            return await _codeFileRepository.GetAnalyzeOutput(code);
        }
    }
}
