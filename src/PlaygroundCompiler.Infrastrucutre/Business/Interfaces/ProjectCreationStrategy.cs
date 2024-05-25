using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Business.Interfaces
{
    public abstract class ProjectCreationStrategy
    {
        protected IProcessRepository ProcessRepository { get; set; }
        public ProjectCreationStrategy(IProcessRepository processRepository)
        {
            ProcessRepository = processRepository;
        }

        public virtual async Task<bool> Prepare(Code code)
        {
            if (string.IsNullOrWhiteSpace(code.MetaInfo.MainFileName))
            {
                code.MetaInfo.MainFileName = "file0";
            }

            foreach (var item in code.SourceCodeFiles)
            {
                string codeFile = $"/usercode/{code.Id}/{item.Key}.{code.Language.ToString().ToLower()}";

                if (File.Exists(codeFile))
                {
                    Console.WriteLine("That file exists already.");
                    return false;
                }

                await File.WriteAllTextAsync(codeFile, item.Value);

                if (string.IsNullOrEmpty(code.MetaInfo.DirecotryPath))
                {
                    code.MetaInfo.DirecotryPath = Path.GetDirectoryName(codeFile);
                }
            }

            if (code is JudgeCode)
            {
                var judgeCode = code as JudgeCode;
                switch (judgeCode.JudgeTypes)
                {
                    case JudgeTypes.UnitTest:
                        string testFile = $"/usercode/{code.Id}/Test.{code.Language.ToString().ToLower()}";

                        if (File.Exists(testFile))
                        {
                            Console.WriteLine("Test file exists already.");
                            return false;
                        }

                        await File.WriteAllTextAsync(testFile, judgeCode.InitialTestCode);

                        ProcessRepository.SystemCopy("/app/scripts/judgeUnitRunner.sh", $"/usercode/{code.Id}");
                        break;
                    case JudgeTypes.InputOutput:
                        Directory.CreateDirectory($"/usercode/{code.Id}/inputs");
                        Directory.CreateDirectory($"/usercode/{code.Id}/outputs");
                        foreach (var input in judgeCode.JudgeInputs)
                        {
                            string inputFile = $"/usercode/{code.Id}/inputs/inputFile{DigitFormat(input.Key)}"; //inputFile1080, inputFile17891, ..., inputFile25490235
                            await File.WriteAllTextAsync(inputFile, input.Value);
                        }

                        if (code.Language == Languages.Py)
                        {
                            ProcessRepository.SystemCopy("/app/scripts/judgeIoRunnerPython.sh", $"/usercode/{code.Id}");
                        }
                        else if (code.Language == Languages.Node || code.Language == Languages.Js || code.Language == Languages.Ts)
                        {
                            string ext = code.Language == Languages.Ts ? "ts" : "js";
                            string extPascalCase = string.Concat(ext[0].ToString().ToUpper(), ext.AsSpan(1).ToString());

                            string userCode = await File.ReadAllTextAsync($"/usercode/{code.Id}/file0.{ext}");

                            if (!userCode.Contains("process.stdin.setEncoding") || !userCode.Contains("process.stdin.on"))
                            {
                                string minifiedUserCode = userCode.Replace(" ", "");

                                if (userCode.Contains("function main()") || minifiedUserCode.Contains("main=()=>"))
                                {
                                    string inputBase = await File.ReadAllTextAsync($"/app/scripts/judge{extPascalCase}UserInputBase.txt");
                                    await File.WriteAllTextAsync($"/usercode/{code.Id}/file0.{ext}", inputBase + Environment.NewLine + userCode, Encoding.UTF8);
                                }
                                else
                                {
                                    if (userCode.Contains("readLine()") || minifiedUserCode.Contains("readLine()"))
                                    {
                                        string inputBase = await File.ReadAllTextAsync($"/app/scripts/judge{extPascalCase}UserInputBase.txt");
                                        const string funcBegin = "function main() {";
                                        const string funcClose = "}";
                                        await File.WriteAllTextAsync($"/usercode/{code.Id}/file0.{ext}", inputBase + Environment.NewLine + funcBegin + Environment.NewLine + userCode + Environment.NewLine + funcClose, Encoding.UTF8);
                                    }
                                }
                            }
                        }
                        else if (code.Language == Languages.Sql)
                        {
                            ProcessRepository.SystemCopy("/app/scripts/judgeIoRunnerSQL.sh", $"/usercode/{code.Id}");
                        }
                        ProcessRepository.SystemCopy("/app/scripts/judgeIoRunner.sh", $"/usercode/{code.Id}");
                        break;
                    default:
                        break;
                }
            }
            else if (code.Id.Contains("analyze"))
            {
                ProcessRepository.SystemCopy("/app/scripts/analyze.sh", $"/usercode/{code.Id}");
            }
            else
            {
                ProcessRepository.SystemCopy("/app/scripts/runner.sh", $"/usercode/{code.Id}");
                string inputFile = $"/usercode/{code.Id}/inputFile";
                await File.WriteAllTextAsync(inputFile, code.Input);
            }

            if (code.Language == Languages.Cs)
            {
                ProcessRepository.SystemCopy("/app/scripts/csRunner.sh", $"/usercode/{code.Id}");
            }

            if (code.Language == Languages.Sql)
            {
                ProcessRepository.SystemCopy("/app/scripts/sqlRunner.sh", $"/usercode/{code.Id}");
                ProcessRepository.SystemCopy("/app/scripts/sqlHtmlTemplate.html", $"/usercode/{code.Id}");
            }

            return true;
        }

        private static string DigitFormat(int index)
        {
            return string.Format("{0:D2}", index);
        }
    }
}
