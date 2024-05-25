using PlaygroundService.Infrastrucutre;
using PlaygroundService.Infrastrucutre.Entities;

namespace PlaygroundService.Web.Dto
{
    public class JudgeUnitDto
    {
        public string JudgeId { get; set; }
        public string Language { get; set; }
        public string[] SourceCode { get; set; }
        public string TestCode { get; set; }
        public string Input { get; set; }
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
                Input = string.IsNullOrEmpty(Input) ? Environment.NewLine : Input,
                Language = language,
                Date = DateTime.Now,
                InitialSourceCode = SourceCode,
                InitialTestCode = TestCode,
                UserId = UserId,
                JudgeTypes = JudgeTypes.UnitTest
            };
        }
    }
}
