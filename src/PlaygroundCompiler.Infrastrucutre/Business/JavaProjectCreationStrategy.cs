using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business
{
    public class JavaProjectCreationStrategy : ProjectCreationStrategy
    {
        private string BuilderMachineName { get; set; }
        private string CompilerName { get; set; }
        private string OutputCommand { get; set; }
        private string RunTimeout { get; set; }

        public JavaProjectCreationStrategy(IProcessRepository processRepository, string compilerName, string builderMachine, string outputCommand, string runTimeout) : base(processRepository)
        {
            CompilerName = compilerName;
            BuilderMachineName = builderMachine;
            OutputCommand = outputCommand;
            RunTimeout = runTimeout;
        }

        public override async Task<bool> Prepare(Code code)
        {
            code.MetaInfo.Compiler = CompilerName;
            code.MetaInfo.BuilderMachineName = BuilderMachineName;
            code.MetaInfo.RunTimeout = RunTimeout;

            if (!string.IsNullOrEmpty(OutputCommand))
            {
                code.OutputCommand = OutputCommand;
            }

            foreach (var item in code.InitialSourceCode.Select((value, index) => new { value, index }))
            {
                string sourceCode = RemoveComments(item.value);
                string temporarySourceCode = sourceCode.Replace("'{'", "' '").Replace("'}'", "' '");
                temporarySourceCode = Regex.Replace(temporarySourceCode, @"(\"".*?[^\\]\"")", m => new string(' ', m.Length));
                bool isBalanced = AreBracketsBalanced(temporarySourceCode);
                if (isBalanced)
                {
                    foreach (var i in SeparateJavaFiles(temporarySourceCode, sourceCode))
                    {
                        code.SourceCodeFiles.Add(i.Key, i.Value);
                    }
                }
                else
                {
                    Regex rx = new(@"^.*(?:public\s*)(class|interface|enum)( *)([a-zA-Z_][a-zA-Z_0-9]*)", RegexOptions.Multiline);
                    var matches = rx.Matches(sourceCode);
                    code.SourceCodeFiles.Add(matches[0].ToString(), sourceCode);
                }
            }

            bool prepareBase = await base.Prepare(code);

            if (code.Id.Contains("analyze"))
            {
                ProcessRepository.SystemCopy("/app/scripts/analyze.sh", $"/usercode/{code.Id}");
            }

            if (prepareBase)
            {
                ProcessRepository.SystemCopy("/app/scripts/javaRunner.sh", $"/usercode/{code.Id}");

                return true;
            }
            return false;
        }

        private static string RemoveComments(string sourceCode)
        {
            List<CodeState> codeStates = new();

            char currentCharacter, nextCharacter;

            StringStates currentState = StringStates.Self;
            for (int i = 0; i < sourceCode.Length - 1; i++)
            {
                int currentIndex = i;
                currentCharacter = sourceCode[i];
                nextCharacter = sourceCode[i + 1];

                switch (currentCharacter.ToString() + nextCharacter.ToString())
                {
                    case "//":
                        if (currentState == StringStates.Self)
                        {
                            currentState = StringStates.InComment;
                            codeStates.Add(new CodeState(i, currentState));
                            i++;
                        }
                        break;
                    case "/*":
                        if (currentState == StringStates.Self)
                        {
                            currentState = StringStates.InMultilineComment;
                            codeStates.Add(new CodeState(i, currentState));
                            i++;
                        }
                        break;
                    case "*/":
                        if (currentState == StringStates.InMultilineComment)
                        {
                            codeStates.Last().EndIndex = ++i;
                            currentState = StringStates.Self;
                        }
                        break;
                    case "\\\"":
                        i++;
                        break;
                }

                switch (currentCharacter)
                {
                    case '"':
                        if (currentState == StringStates.Self)
                        {
                            currentState = StringStates.InString;
                            codeStates.Add(new CodeState(i, currentState));
                        }
                        else if (currentState == StringStates.InString)
                        {
                            codeStates.Last().EndIndex = i;
                            currentState = StringStates.Self;
                        }
                        break;
                    case '\n':
                        if (currentState == StringStates.InComment)
                        {
                            codeStates.Last().EndIndex = sourceCode[i - 1] == '\r' ? i - 1 : i;
                            currentState = StringStates.Self;
                        }
                        break;
                }

                if (currentIndex + 1 == sourceCode.Length - 1 && currentState == StringStates.InComment)
                {
                    codeStates.Last().EndIndex = sourceCode.Length - 1;
                    currentState = StringStates.Self;
                }
            }

            for (int i = codeStates.Count - 1; i >= 0; i--)
            {
                var item = codeStates[i];

                if (item.State == StringStates.InComment || item.State == StringStates.InMultilineComment)
                {
                    var strBuilder = new StringBuilder(sourceCode);
                    strBuilder.Remove(item.StartIndex, item.EndIndex + 1 - item.StartIndex);
                    sourceCode = strBuilder.ToString();
                }
            }

            return sourceCode;
        }

        private static bool AreBracketsBalanced(string sourceCode)
        {
            var bracketsCount = 0;

            for (int i = 0; i < sourceCode.Length; i++)
            {
                switch (sourceCode[i])
                {
                    case '{':
                        bracketsCount++;
                        break;
                    case '}':
                        bracketsCount--;
                        break;
                }

                if (bracketsCount < 0) return false;
            }

            return bracketsCount == 0;
        }

        private static Dictionary<string, string> SeparateJavaFiles(string temporarySourceCode, string sourceCode)
        {
            var returnList = new Dictionary<string, string>();

            string header = string.Empty;

            bool headerFound = false;

            int processedIndex = 0;

            int lastIndex = 0;

            Regex rx = new(@"^.*( *)(class|interface)( *)([a-zA-Z_][a-zA-Z_0-9]*)", RegexOptions.Multiline);
            var matches = rx.Matches(temporarySourceCode);
            int counter = matches.Count; //count of iterations over non-inner classes (changes in the for loop)
            for (int i = 0; i < counter; i++)
            {
                Match match = matches[matches.Count - counter + i];

                if (match.Index < processedIndex) continue;
                int bracketsCount = 0;

                string intermediateCode = string.Empty;

                bool isSurrounded = false;

                if (!headerFound && match.Index > 0)
                {
                    header = sourceCode.Substring(0, match.Index - 1);
                }
                headerFound = true;

                if (lastIndex > 0)
                {
                    intermediateCode = sourceCode.Substring(lastIndex + 1, match.Index - lastIndex - 1);
                }

                string className = match.Groups[4].Value;

                for (int j = match.Index; j < temporarySourceCode.Length; j++)
                {
                    switch (temporarySourceCode[j])
                    {
                        case '{':
                            bracketsCount++;
                            break;
                        case '}':
                            bracketsCount--;
                            isSurrounded = (bracketsCount == 0);
                            lastIndex = j;
                            break;
                    }

                    if (isSurrounded)
                    {
                        processedIndex = lastIndex;
                        var classCode = intermediateCode + sourceCode[match.Index..(lastIndex + 1)];
                        var nestedMatches = rx.Matches(classCode);
                        counter -= (nestedMatches.Count - 1); // minus nested classes from the current class

                        if (i + 1 >= counter)
                        {
                            classCode += sourceCode[(lastIndex + 1)..];
                        }

                        returnList.Add(className, header + "\n" + classCode);

                        break;
                    }
                }
            }
            return returnList;
        }

        public enum StringStates
        {
            Self = 0,
            InComment = 1,
            InMultilineComment = 2,
            InString = 3
        }

        class CodeState
        {
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public StringStates State { get; set; }

            public CodeState() { }
            public CodeState(int startIndex, StringStates state) : this()
            {
                StartIndex = startIndex;
                State = state;
            }
        }
    }
}
