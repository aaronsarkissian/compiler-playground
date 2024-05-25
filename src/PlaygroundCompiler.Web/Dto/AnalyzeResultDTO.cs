namespace PlaygroundService.Web.Dto
{
    public class AnalyzeResultDTO
    {
        public string Type { get; set; }
        public string Obj { get; set; }
        public int? Line { get; set; }
        public int? Column { get; set; }
        public string Symbol { get; set; }
        public string Message { get; set; }
    }
}
