using PlaygroundService.Infrastrucutre;
using PlaygroundService.Infrastrucutre.Entities;

namespace PlaygroundService.Web.Dto
{
    public class JudgeIoDto
    {
        public string JudgeId { get; set; }
        public string Language { get; set; }
        public string[] SourceCode { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Inputs { get; set; }
        public int? UserId { get; set; }

        public JudgeCode ToCode()
        {
            if (!Enum.TryParse(Language, true, out Languages language))
            {
                return null;
            }
            return new JudgeCode()
            {
                Id = JudgeId,
                JudgeInputs = Inputs,
                Language = language,
                Date = DateTime.Now,
                InitialSourceCode = SourceCode,
                UserId = UserId,
                JudgeTypes = JudgeTypes.InputOutput
            };
        }
    }
}
