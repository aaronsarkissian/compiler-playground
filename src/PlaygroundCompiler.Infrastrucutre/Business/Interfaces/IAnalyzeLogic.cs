using PlaygroundService.Infrastrucutre.Entities;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business.Interfaces
{
    public interface IAnalyzeLogic
    {
        Task<AnalyzeResult[]> Analyze(Code code);
    }
}
