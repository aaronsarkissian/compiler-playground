using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Entities;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business.Interfaces
{
    public interface IPlaygroundLogic
    {
        Task<CompilerOutput> BuildAndRun(Code code);

    }
}
