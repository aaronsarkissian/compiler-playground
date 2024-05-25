using System.Collections.Generic;

namespace PlaygroundService.Infrastrucutre.Entities
{
    public class JudgeOutput
    {
        public int StatusCode { get; set; }
        public string CompileOutput { get; set; }
        public string Xml { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Outputs { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Errors { get; set; }
    }
}
