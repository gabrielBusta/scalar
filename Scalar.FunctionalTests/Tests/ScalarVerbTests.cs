﻿using Scalar.Tests.Should;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace Scalar.FunctionalTests.Tests
{
    [TestFixture]
    public class ScalarVerbTests
    {
        public ScalarVerbTests()
        {
        }

        private enum ExpectedReturnCode
        {
            Success = 0,
            ParsingError = 1,
        }

        [TestCase]
        public void UnknownVerb()
        {
            this.CallScalar("help", ExpectedReturnCode.Success);
            this.CallScalar("unknownverb", ExpectedReturnCode.ParsingError);
        }

        [TestCase]
        public void UnknownArgs()
        {
            this.CallScalar("log --help", ExpectedReturnCode.Success);
            this.CallScalar("log --unknown-arg", ExpectedReturnCode.ParsingError);
        }

        private void CallScalar(string args, ExpectedReturnCode expectedErrorCode)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(ScalarTestConfig.PathToScalar);
            processInfo.Arguments = args;
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            using (Process process = Process.Start(processInfo))
            {
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                process.ExitCode.ShouldEqual((int)expectedErrorCode, result);
            }
        }
    }
}
