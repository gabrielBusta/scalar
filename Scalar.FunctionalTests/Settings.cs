using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Scalar.FunctionalTests.Properties
{
    public static class Settings
    {
        public enum ValidateWorkingTreeMode
        {
            None = 0,
            Full = 1,
            SparseMode = 2,
        }

        public static class Default
        {
            public static string CurrentDirectory { get; private set; }

            public static string RepoToClone { get; set; }
            public static string PathToBash { get; set; }
            public static string PathToScalar { get; set; }
            public static string Commitish { get; set; }
            public static string ControlGitRepoRoot { get; set; }
            public static string EnlistmentRoot { get; set; }
            public static string FastFetchBaseRoot { get; set; }
            public static string FastFetchRoot { get; set; }
            public static string FastFetchControl { get; set; }
            public static string PathToGit { get; set; }
            public static string PathToScalarService { get; set; }
            public static string BinaryFileNameExtension { get; set; }

            public static void Initialize()
            {
                CurrentDirectory = Path.GetFullPath(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));

                RepoToClone = @"https://gvfs.visualstudio.com/ci/_git/ForTests";
                Commitish = @"FunctionalTests/20180214";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    EnlistmentRoot = @"C:\Repos\ScalarFunctionalTests\enlistment";
                    PathToScalar = @"Scalar.exe";
                    PathToGit = @"C:\Program Files\Git\cmd\git.exe";
                    PathToBash = @"C:\Program Files\Git\bin\bash.exe";

                    ControlGitRepoRoot = @"C:\Repos\ScalarFunctionalTests\ControlRepo";
                    FastFetchBaseRoot = @"C:\Repos\ScalarFunctionalTests\FastFetch";
                    FastFetchRoot = Path.Combine(FastFetchBaseRoot, "test");
                    FastFetchControl = Path.Combine(FastFetchBaseRoot, "control");
                    PathToScalarService = @"Scalar.Service.exe";
                    BinaryFileNameExtension = ".exe";
                }
                else
                {
                    string root = Path.Combine(
                        Environment.GetEnvironmentVariable("HOME"),
                        "Scalar.FT");
                    EnlistmentRoot = Path.Combine(root, "test");
                    ControlGitRepoRoot = Path.Combine(root, "control");
                    FastFetchBaseRoot = Path.Combine(root, "FastFetch");
                    FastFetchRoot = Path.Combine(FastFetchBaseRoot, "test");
                    FastFetchControl = Path.Combine(FastFetchBaseRoot, "control");
                    PathToScalar = "scalar";
                    PathToGit = "/usr/local/bin/git";
                    PathToBash = "/bin/bash";
                    BinaryFileNameExtension = string.Empty;
                }
            }
        }
    }
}