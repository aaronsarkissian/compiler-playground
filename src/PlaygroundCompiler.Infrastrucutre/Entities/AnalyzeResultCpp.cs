namespace PlaygroundService.Infrastrucutre.Entities
{
    public class AnalyzeResultCpp
    {
        public string Kind { get; set; }
        public ErrorLocation[] Locations { get; set; }
        public string Message { get; set; }
    }

    public class ErrorLocation
    {
        public Caret Caret { get; set; }
    }

    public class Caret
    {
        public int? Line { get; set; }
        public int? Column { get; set; }
    }
}
