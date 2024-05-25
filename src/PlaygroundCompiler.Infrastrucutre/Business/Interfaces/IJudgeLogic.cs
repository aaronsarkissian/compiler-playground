using PlaygroundService.Infrastrucutre.Entities;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business.Interfaces
{
    public interface IJudgeLogic
    {
        Task<JudgeOutput> BuildAndTest(JudgeCode judgeCode);
    }
}
