using System;

namespace SvnTools
{
    public static class Svn
    {
        const string kExecutable = "svn.exe";

        public static string[] List(string target)
        {
            ProcessResult result = ProcessCapture.Get(kExecutable, "ls " + target);
            if (result.ExitCode == 0)
            {
                return result.Output.Lines();
            }
            else
            {
                throw new Exception(result.Error);
            }
        }
    }
}
