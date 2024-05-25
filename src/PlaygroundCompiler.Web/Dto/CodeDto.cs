using PlaygroundService.Infrastrucutre;
using PlaygroundService.Infrastrucutre.Entities;

namespace PlaygroundService.Web.Dto
{
    public class CodeDto
    {
        public string CodeId { get; set; }
        public string Language { get; set; }
        public string[] SourceCode { get; set; }
        public string Input { get; set; }
        public int? UserId { get; set; }

        public Code ToCode()
        {
            if (!Enum.TryParse(Language, true, out Languages language))
            {
                return null;
            }
            return new Code()
            {
                Id = CodeId,
                Input = string.IsNullOrEmpty(Input) ? Environment.NewLine : Input,
                Language = language,
                Date = DateTime.Now,
                InitialSourceCode = SourceCode,
                UserId = UserId,
            };
        }
    }
}
