namespace PlaygroundService.Infrastrucutre.Repositories.Interfaces
{
    public interface IProcessRepository
    {
        bool SystemCopy(string source, string target);
        bool ProcessStart(string fileName, string args = null, bool waitForExit = false);

    }
}
