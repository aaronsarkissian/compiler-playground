using System.IO;
using System.Threading.Tasks;

namespace PlaygroundCompiler.Infrastrucutre.Services.Interfaces
{
    public interface IBlobService
    {
        Task<string> GetBlobImageUrl(FileStream fileStream);
    }
}
