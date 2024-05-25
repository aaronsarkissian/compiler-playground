using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Entities;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Repositories.Interfaces
{
    public interface ICodeBuildRepository
    {
        void BuildAndRun(Code code);
        void BuildAndTest(JudgeCode judgeCode);
        void Analyze(Code code);
        Task<CompilerOutput> GetResult(string codeId);
        Task<JudgeOutput> GetJudgeUnitResult(string judgeCodeId);
        Task<JudgeOutput> GetJudgeIoResult(string judgeCodeId);
        Task<AnalyzeResult[]> GetAnalyzeResult(Code code);
    }
}
