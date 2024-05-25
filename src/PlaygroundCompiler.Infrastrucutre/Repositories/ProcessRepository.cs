using PlaygroundService.Infrastrucutre.Repositories.Interfaces;
using System;
using System.Diagnostics;

namespace PlaygroundService.Infrastrucutre.Repositories
{
    public class UnixProcessRepository : IProcessRepository
    {
        public bool ProcessStart(string fileName, string args, bool waitForExit)
        {
            try
            {
                using var myProcess = new Process();
                myProcess.StartInfo.FileName = fileName;
                myProcess.StartInfo.Arguments = args;

                myProcess.Start();

                if (waitForExit)
                {
                    myProcess.WaitForExit();
                }

                if (myProcess.HasExited)
                {
                    //Console.WriteLine(myProcess.ExitCode);
                }

                return myProcess.ExitCode == 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("The ProcessStart process failed: {0}", e.ToString());
                return false;
            }
        }

        public bool SystemCopy(string source, string target)
        {
            try
            {
                using var myProcess = new Process();
                myProcess.StartInfo.FileName = "cp";
                myProcess.StartInfo.Arguments = $"{source} {target}";
                myProcess.Start();
                myProcess.WaitForExit();
                return myProcess.ExitCode == 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("The SystemCopy process failed: {0}", e.ToString());
                return false;
            }

        }
    }
}
