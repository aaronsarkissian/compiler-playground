using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Entities;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Repositories.Interfaces
{
    public interface ICodeFileRepository
    {
        Task<bool> CreateProjectFiles(Code code, ProjectCreationStrategy strategy);
        Task<bool> CreateJudgeFiles(JudgeCode judgeCode, ProjectCreationStrategy strategy);
        Task<CompilerOutput> GetOutputItem(string path);
        Task<JudgeOutput> GetJudgeUnitOutput(string path);
        Task<JudgeOutput> GetJudgeIoOutput(string path);
        Task<AnalyzeResult[]> GetAnalyzeOutput(Code code);
        Task DeleteProjectDirectory(string path);
    }
}
