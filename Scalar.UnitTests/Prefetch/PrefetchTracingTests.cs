﻿using Scalar.Common.Prefetch.Pipeline;
using Scalar.Common.Prefetch.Pipeline.Data;
using Scalar.Common.Tracing;
using Scalar.Tests.Should;
using Scalar.UnitTests.Mock.Common;
using Scalar.UnitTests.Mock.Git;
using NUnit.Framework;
using System.Collections.Concurrent;

namespace Scalar.UnitTests.Prefetch
{
    [TestFixture]
    public class PrefetchTracingTests
    {
        private const string FakeSha = "fakesha";
        private const string FakeShaContents = "fakeshacontents";

        [TestCase]
        public void ErrorsForBatchObjectDownloadJob()
        {
            using (ITracer tracer = CreateTracer())
            {
                MockScalarEnlistment enlistment = new MockScalarEnlistment();
                MockHttpGitObjects httpGitObjects = new MockHttpGitObjects(tracer, enlistment);
                MockPhysicalGitObjects gitObjects = new MockPhysicalGitObjects(tracer, null, enlistment, httpGitObjects);

                BlockingCollection<string> input = new BlockingCollection<string>();
                input.Add(FakeSha);
                input.CompleteAdding();

                BatchObjectDownloadStage dut = new BatchObjectDownloadStage(1, 1, input, new BlockingCollection<string>(), tracer, enlistment, httpGitObjects, gitObjects);
                dut.Start();
                dut.WaitForCompletion();

                string sha;
                input.TryTake(out sha).ShouldEqual(false);

                IndexPackRequest request;
                dut.AvailablePacks.TryTake(out request).ShouldEqual(false);
            }
        }

        [TestCase]
        public void SuccessForBatchObjectDownloadJob()
        {
            using (ITracer tracer = CreateTracer())
            {
                MockScalarEnlistment enlistment = new MockScalarEnlistment();
                MockHttpGitObjects httpGitObjects = new MockHttpGitObjects(tracer, enlistment);
                httpGitObjects.AddBlobContent(FakeSha, FakeShaContents);
                MockPhysicalGitObjects gitObjects = new MockPhysicalGitObjects(tracer, null, enlistment, httpGitObjects);

                BlockingCollection<string> input = new BlockingCollection<string>();
                input.Add(FakeSha);
                input.CompleteAdding();

                BatchObjectDownloadStage dut = new BatchObjectDownloadStage(1, 1, input, new BlockingCollection<string>(), tracer, enlistment, httpGitObjects, gitObjects);
                dut.Start();
                dut.WaitForCompletion();

                string sha;
                input.TryTake(out sha).ShouldEqual(false);
                dut.AvailablePacks.Count.ShouldEqual(0);

                dut.AvailableObjects.Count.ShouldEqual(1);
                string output = dut.AvailableObjects.Take();
                output.ShouldEqual(FakeSha);
            }
        }

        [TestCase]
        public void ErrorsForIndexPackFile()
        {
            using (ITracer tracer = CreateTracer())
            {
                MockScalarEnlistment enlistment = new MockScalarEnlistment();
                MockPhysicalGitObjects gitObjects = new MockPhysicalGitObjects(tracer, null, enlistment, null);

                BlockingCollection<IndexPackRequest> input = new BlockingCollection<IndexPackRequest>();
                BlobDownloadRequest downloadRequest = new BlobDownloadRequest(new string[] { FakeSha });
                input.Add(new IndexPackRequest("mock:\\path\\packFileName", downloadRequest));
                input.CompleteAdding();

                IndexPackStage dut = new IndexPackStage(1, input, new BlockingCollection<string>(), tracer, gitObjects);
                dut.Start();
                dut.WaitForCompletion();
            }
        }

        private static ITracer CreateTracer()
        {
            return new MockTracer();
        }
    }
}
