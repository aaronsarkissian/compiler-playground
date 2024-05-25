namespace PlaygroundService.Infrastrucutre.Entities
{
    public class MetaInfo
    {
        public string DirecotryPath { get; set; }
        public string MainFileName { get; set; }
        public string OutputFileName { get; set; }
        public int ExitCode { get; set; }
        public string BuilderMachineName { get; set; }
        public string Compiler { get; set; }
        public string RunTimeout { get; set; }
    }
}
