using System;
using System.Collections.Generic;

namespace PlaygroundService.Infrastrucutre.Entities
{
    public class Code
    {
        private Languages localLanguage;
        public string Id { get; set; }
        public string[] InitialSourceCode { get; set; }
        public Dictionary<string, string> SourceCodeFiles { get; set; } = new Dictionary<string, string>();
        public int? ErrorCode { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string OutputCommand { get; internal set; }
        public DateTime Date { get; set; }
        public Languages Language
        {
            get { return localLanguage; }
            set
            {
                if (value == Languages.Node)
                {
                    localLanguage = Languages.Js;
                }
                else
                {
                    localLanguage = value;
                }
            }
        }
        public int? UserId { get; set; }
        public MetaInfo MetaInfo { get; set; } = new MetaInfo();
    }
}
