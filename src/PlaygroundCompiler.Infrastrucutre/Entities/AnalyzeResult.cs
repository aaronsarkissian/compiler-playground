namespace PlaygroundService.Infrastrucutre.Entities
{
    public class AnalyzeResult
    {
        public string Type { get; set; }
        public string Obj { get; set; }
        public int? Line { get; set; }
        public int? Column { get; set; }
        public string Symbol { get; set; }
        public string Message { get; set; }
    }
}
