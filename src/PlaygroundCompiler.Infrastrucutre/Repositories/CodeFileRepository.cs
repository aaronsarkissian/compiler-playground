using Newtonsoft.Json;
using PlaygroundCompiler.Infrastrucutre.Entities;
using PlaygroundCompiler.Infrastrucutre.Services.Interfaces;
using PlaygroundService.Infrastrucutre.Business.Interfaces;
using PlaygroundService.Infrastrucutre.Entities;
using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlaygroundService.Infrastrucutre.Repositories
{
    public class CodeFileRepository : ICodeFileRepository
    {
        private const string noOutput = "No output.";
        private const string noTestOutput = "No test output.";
        private readonly IBlobService _blobService;

        public CodeFileRepository(IBlobService blobService)
        {
            _blobService = blobService;
        }

        // Playground and Judge and Analyze
        private static string CreateProjectDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already.");
                    return null;
                }

                DirectoryInfo di = Directory.CreateDirectory(path);

                return di.FullName;
            }
            catch (Exception e)
            {
                Console.WriteLine("The CreateProjectDirectory process failed: {0}", e.ToString());
                return null;
            }
        }

        // Playground and Analyze
        public async Task<bool> CreateProjectFiles(Code code, ProjectCreationStrategy strategy)
        {
            try
            {
                code.MetaInfo.DirecotryPath = CreateProjectDirectory($"/usercode/{code.Id}");

                return await strategy.Prepare(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("The CreateProjectFiles process failed: {0}", e.ToString());
                return false;
            }
        }

        // Only Judge
        public async Task<bool> CreateJudgeFiles(JudgeCode judgeCode, ProjectCreationStrategy strategy)
        {
            try
            {
                judgeCode.MetaInfo.DirecotryPath = CreateProjectDirectory($"/usercode/{judgeCode.Id}");

                return await strategy.Prepare(judgeCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("The CreateJudgeFiles process failed: {0}", e.ToString());

                return false;
            }
        }

        public static async Task<bool> CreateAnalyzeFiles(Code code, ProjectCreationStrategy strategy)
        {
            try
            {
                code.MetaInfo.DirecotryPath = CreateProjectDirectory($"/usercode/{code.Id}");

                return await strategy.Prepare(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("The CreateAnalyzeFiles process failed: {0}", e.ToString());
                return false;
            }
        }

        // Only Playground
        public async Task<CompilerOutput> GetOutputItem(string path)
        {
            string outputPath = $"{path}/completed";
            string errorPath = $"{path}/errors";
            string logFilePath = $"{path}/logfile.txt";

            CompilerOutput compilerOutput = new();

            try
            {
                if (Directory.Exists(path))
                {
                    bool imageExists = false;
                    bool outputExists = true;
                    if (File.Exists(errorPath) && new FileInfo(errorPath).Length > 0)
                    {
                        // For the Error field only
                        compilerOutput.Error = await File.ReadAllTextAsync(errorPath);

                        // Incomplete output from log file
                        if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > 0)
                        {
                            compilerOutput.Output = $"{await File.ReadAllTextAsync(logFilePath)}\n\n{await File.ReadAllTextAsync(errorPath)}\n";
                        }
                        // Complete output from output file
                        else if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
                        {
                            compilerOutput.Output = $"{await File.ReadAllTextAsync(outputPath)}\n\n{await File.ReadAllTextAsync(errorPath)}\n";
                        }
                        else
                        {
                            compilerOutput.Output = $"{await File.ReadAllTextAsync(errorPath)}\n";
                        }
                    }
                    else if (File.Exists(outputPath))
                    {
                        if (new FileInfo(outputPath).Length > 0)
                        {
                            compilerOutput.Output = await File.ReadAllTextAsync(outputPath);
                        }
                        else if (new FileInfo(outputPath).Length == 0)
                        {
                            outputExists = false;
                            compilerOutput.Output = noOutput;
                        }
                    }
                    else
                    {
                        if (new FileInfo(logFilePath).Length > 0)
                        {
                            compilerOutput.Output = await File.ReadAllTextAsync(logFilePath);
                        }
                        else
                        {
                            outputExists = false;
                            compilerOutput.Output = noOutput;
                        }
                    }

                    if (Directory.GetFiles(path, "*.png", SearchOption.AllDirectories).Length != 0)
                    {
                        var images = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                        if (images.Length > 0)
                        {
                            imageExists = true;
                            compilerOutput.Images = new string[images.Length];
                            for (int i = 0; i < images.Length; i++)
                            {
                                using var filestream = File.OpenRead(images[i]);
                                compilerOutput.Images[i] = await _blobService.GetBlobImageUrl(filestream);
                            }
                        }
                    }
                    if (imageExists && !outputExists)
                    {
                        compilerOutput.Output = string.Empty;
                    }
                }
                else
                {
                    compilerOutput.Output = noOutput;
                }
                return compilerOutput;
            }
            catch (Exception e)
            {
                Console.WriteLine("The reading output process failed: {0}", e.ToString());
                return null;
            }
            finally
            {
                await DeleteProjectDirectory(path);
            }
        }

        // Only Judge
        public async Task<JudgeOutput> GetJudgeUnitOutput(string path)
        {
            string testOutputPath = $"{path}/output.xml";
            JudgeOutput judgeOutput = new();

            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(testOutputPath) && new FileInfo(testOutputPath).Length > 0)
                    {
                        judgeOutput.Xml = await File.ReadAllTextAsync(testOutputPath);
                        return judgeOutput;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("The unitJudge reading output process failed: {0}", e.ToString());
                return null;
            }
            finally
            {
                await DeleteProjectDirectory(path);
            }
        }

        // Only IO Judge
        public async Task<JudgeOutput> GetJudgeIoOutput(string path)
        {
            string errorPath = $"{path}/errors";
            string testOutputPath = $"{path}/outputs";
            string errorCode = $"{path}/errorCode";
            JudgeOutput judgeOutput = new();

            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(errorCode) && new FileInfo(errorCode).Length > 0)
                    {
                        int.TryParse(await File.ReadAllTextAsync(errorCode), out int statusCodeInt);
                        judgeOutput.StatusCode = statusCodeInt;
                    }

                    if (File.Exists(errorPath) && new FileInfo(errorPath).Length > 0)
                    {
                        judgeOutput.CompileOutput = await File.ReadAllTextAsync(errorPath);
                    }

                    if (Directory.Exists(testOutputPath))
                    {
                        var outputs = new List<KeyValuePair<int, string>>();
                        var errors = new List<KeyValuePair<int, string>>();

                        foreach (var file in Directory.GetFiles(testOutputPath, "output*"))
                        {
                            var id = GetFileNameId(file, "output");
                            outputs.Add(new KeyValuePair<int, string>(id, await File.ReadAllTextAsync(file)));
                            judgeOutput.Outputs = outputs;
                        }
                        foreach (var file in Directory.GetFiles(testOutputPath, "error*"))
                        {
                            var id = GetFileNameId(file, "error");
                            errors.Add(new KeyValuePair<int, string>(id, await File.ReadAllTextAsync(file)));
                            judgeOutput.Errors = errors;
                        }
                        return judgeOutput;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("The judgeIo reading output process failed: {0}", e.ToString());
                return null;
            }
            finally
            {
                await DeleteProjectDirectory(path);
            }
        }

        // Only Analyze
        public async Task<AnalyzeResult[]> GetAnalyzeOutput(Code code)
        {
            string path = $"/usercode/{code.Id}";

            string analyzePath = $"{path}/analyze";

            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(analyzePath) && new FileInfo(analyzePath).Length > 0)
                    {
                        switch (code.Language)
                        {
                            case Languages.Cpp:
                                var analyzeResultCpp = JsonConvert.DeserializeObject<AnalyzeResultCpp[]>(await File.ReadAllTextAsync(analyzePath));

                                if (!analyzeResultCpp.Any())
                                {
                                    return Array.Empty<AnalyzeResult>();
                                }

                                return new AnalyzeResult[]
                                {
                                    new AnalyzeResult
                                    {
                                        Type = analyzeResultCpp[0].Kind,
                                        Line = analyzeResultCpp[0].Locations[0].Caret.Line,
                                        Column = analyzeResultCpp[0].Locations[0].Caret.Column,
                                        Message = analyzeResultCpp[0].Message
                                    }
                                };

                            case Languages.Js:
                            case Languages.Node:
                                var analyzeResultNode = JsonConvert.DeserializeObject<AnalyzeResultNode[]>(await File.ReadAllTextAsync(analyzePath));

                                if (!analyzeResultNode[0].Messages.Any())
                                {
                                    return Array.Empty<AnalyzeResult>();
                                }

                                return new AnalyzeResult[]
                                {
                                    new AnalyzeResult
                                    {
                                        Line = analyzeResultNode[0].Messages[0].Line,
                                        Column = analyzeResultNode[0].Messages[0].Column,
                                        Message = analyzeResultNode[0].Messages[0].Message
                                    }
                                };

                            case Languages.Py:
                                return JsonConvert.DeserializeObject<AnalyzeResult[]>(await File.ReadAllTextAsync(analyzePath));

                            case Languages.Cs:
                                string csPattern = @"(.+\/file\d+\.cs\((\d+)\,(\d+)\)\:\s)?error.+:(.+)";
                                string csInput = File.ReadLines(analyzePath).First();
                                Regex csRegex = new(csPattern, RegexOptions.IgnoreCase);
                                Match csMatch = csRegex.Match(csInput);

                                string csMessage = csMatch.Groups[4].ToString().Trim();

                                if (!csMessage.Any())
                                {
                                    return Array.Empty<AnalyzeResult>();
                                }

                                int.TryParse(csMatch.Groups[2].ToString(), out int csLine);
                                int.TryParse(csMatch.Groups[3].ToString(), out int csColumn);

                                return new AnalyzeResult[]
                                {
                                    new AnalyzeResult
                                    {
                                        Line = csLine,
                                        Column = csColumn,
                                        Message = csMessage
                                    }
                                };

                            case Languages.Java:
                                string javaPattern = @".+\.java\:(\d+)\:\serror(.+)?:(.+)";
                                string javaInput = File.ReadAllText(analyzePath);
                                Regex javaRegex = new(javaPattern, RegexOptions.IgnoreCase);
                                Match javaMatch = javaRegex.Match(javaInput);

                                string javaMessage = javaMatch.Groups[3].ToString().Trim();

                                if (!javaMessage.Any())
                                {
                                    return Array.Empty<AnalyzeResult>();
                                }

                                int javaLine = 0; // always ± few lines wrong. Because of the imports custom java logic.
                                int javaColumn = 0; // no info from compiler

                                return new AnalyzeResult[]
                                {
                                    new AnalyzeResult
                                    {
                                        Line = javaLine,
                                        Column = javaColumn,
                                        Message = javaMessage
                                    }
                                };

                            case Languages.Go:
                                string goPattern = @"(.+\/file\d+\.go\:(\d+)\:(\d+)\:\s)(.+)";
                                string goInput = File.ReadAllText(analyzePath);
                                string goInputClean = Regex.Replace(goInput, @"# command-line-arguments", "", RegexOptions.IgnoreCase);
                                Regex goRegex = new(goPattern, RegexOptions.IgnoreCase);
                                Match goMatch = goRegex.Match(goInputClean);

                                string goMessage = goMatch.Groups[4].ToString().Trim();

                                if (!goMessage.Any())
                                {
                                    return Array.Empty<AnalyzeResult>();
                                }

                                int.TryParse(goMatch.Groups[2].ToString(), out int goLine);
                                int.TryParse(goMatch.Groups[3].ToString(), out int goColumn);

                                return new AnalyzeResult[]
                                {
                                    new AnalyzeResult
                                    {
                                        Line = goLine,
                                        Column = goColumn,
                                        Message = goMessage
                                    }
                                };

                            default:
                                break;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("The analyze reading output process failed: {0}", e.ToString());
                return null;
            }
            finally
            {
                await DeleteProjectDirectory(path);
            }
        }

        // Playground and Judge and Analyze
        public Task DeleteProjectDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("That path does not exist.");
                    return Task.CompletedTask;
                }

                Directory.Delete(path, true); // second parameter (true) is for recursive deleting
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine("The deleting process failed: {0}", e.ToString());
                return Task.CompletedTask;
            }
        }

        private static int GetFileNameId(string fileName, string fileType)
        {
            if (int.TryParse(Regex.Match(fileName, $@"(?:{fileType})(\d+)").Groups[1].ToString(), out int id)) // Get the number(id) part of the filename
            {
                return id;
            }
            return 0;
        }
    }
}
