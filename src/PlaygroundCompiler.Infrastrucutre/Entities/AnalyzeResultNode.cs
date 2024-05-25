namespace PlaygroundService.Infrastrucutre.Entities
{
    public class AnalyzeResultNode
    {
        public ErrorMessage[] Messages { get; set; }
    }

    public class ErrorMessage
    {
        public string Message { get; set; }
        public int? Line { get; set; }
        public int? Column { get; set; }
    }
}
