using System.Collections.Generic;

namespace PlaygroundService.Infrastrucutre.Entities
{
    public class JudgeCode : Code
    {
        public string InitialTestCode { get; set; }
        public string UnitTestOutput { get; set; } // AKA XML
        public IEnumerable<KeyValuePair<int, string>> JudgeInputs { get; set; }
        public JudgeTypes JudgeTypes { get; set; }
    }
}
