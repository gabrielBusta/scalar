﻿using Scalar.FunctionalTests.Tools;
using NUnit.Framework;

namespace Scalar.FunctionalTests.Tests.EnlistmentPerTestCase
{
    [TestFixture]
    public abstract class TestsWithEnlistmentPerTestCase
    {
        private readonly bool forcePerRepoObjectCache;

        public TestsWithEnlistmentPerTestCase(bool forcePerRepoObjectCache = false)
        {
            this.forcePerRepoObjectCache = forcePerRepoObjectCache;
        }

        public ScalarFunctionalTestEnlistment Enlistment
        {
            get; private set;
        }

        [SetUp]
        public virtual void CreateEnlistment()
        {
            if (this.forcePerRepoObjectCache)
            {
                this.Enlistment = ScalarFunctionalTestEnlistment.CloneAndMountWithPerRepoCache(ScalarTestConfig.PathToScalar, skipPrefetch: false);
            }
            else
            {
                this.Enlistment = ScalarFunctionalTestEnlistment.CloneAndMount(ScalarTestConfig.PathToScalar);
            }
        }

        [TearDown]
        public virtual void DeleteEnlistment()
        {
            if (this.Enlistment != null)
            {
                this.Enlistment.UnmountAndDeleteAll();
            }
        }
    }
}
