﻿using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using FluentAssertions;
using static GoogleTestAdapter.Tests.Common.TestMetadata.TestCategories;

namespace GoogleTestAdapter.TestAdapter
{
    [TestClass]
    public class TestExecutorSequentialTests : TestExecutorTestsBase
    {

        public TestExecutorSequentialTests() : base(false, 1) { }

        protected override void CheckMockInvocations(int nrOfPassedTests, int nrOfFailedTests, int nrOfUnexecutedTests, int nrOfNotFoundTests)
        {
            base.CheckMockInvocations(nrOfPassedTests, nrOfFailedTests, nrOfUnexecutedTests, nrOfNotFoundTests);

            MockFrameworkHandle.Verify(h => h.RecordResult(It.Is<TestResult>(tr => tr.Outcome == TestOutcome.Passed)),
                Times.Exactly(nrOfPassedTests));
            MockFrameworkHandle.Verify(h => h.RecordEnd(It.IsAny<TestCase>(), It.Is<TestOutcome>(to => to == TestOutcome.Passed)),
                Times.Exactly(nrOfPassedTests));

            MockFrameworkHandle.Verify(h => h.RecordResult(It.Is<TestResult>(tr => tr.Outcome == TestOutcome.Failed)),
                Times.Exactly(nrOfFailedTests));
            MockFrameworkHandle.Verify(h => h.RecordEnd(It.IsAny<TestCase>(), It.Is<TestOutcome>(to => to == TestOutcome.Failed)),
                Times.Exactly(nrOfFailedTests));

            MockFrameworkHandle.Verify(h => h.RecordResult(It.Is<TestResult>(tr => tr.Outcome == TestOutcome.Skipped)),
                Times.Exactly(nrOfNotFoundTests));
            MockFrameworkHandle.Verify(h => h.RecordEnd(It.IsAny<TestCase>(), It.Is<TestOutcome>(to => to == TestOutcome.Skipped)),
                Times.Exactly(nrOfNotFoundTests));
        }


        [TestMethod]
        [TestCategory(Integration)]
        public void RunTests_CancelingExecutor_StopsTestExecution()
        {
            DoRunCancelingTests(
                false, 
                3000,  // 1st test should be executed
                4000); // 2nd test should not be executed 
        }

        [TestMethod]
        [TestCategory(Integration)]
        public void RunTests_CancelingExecutorAndKillProcesses_StopsTestExecutionFaster()
        {
            DoRunCancelingTests(
                true,
                1000,  // 1st test should be canceled
                2500); // 2nd test should not be executed 
        }

        private void DoRunCancelingTests(bool killProcesses, int lower, int upper)
        {
            MockOptions.Setup(o => o.KillProcessesOnCancel).Returns(killProcesses);
            List<Model.TestCase> testCasesToRun = TestDataCreator.GetTestCases("Crashing.LongRunning", "LongRunningTests.Test2");
            testCasesToRun.Count.Should().Be(2);

            Stopwatch stopwatch = new Stopwatch();
            TestExecutor executor = new TestExecutor(TestEnvironment.Logger, TestEnvironment.Options);
            Thread thread = new Thread(() => executor.RunTests(testCasesToRun.Select(DataConversionExtensions.ToVsTestCase), MockRunContext.Object, MockFrameworkHandle.Object));

            stopwatch.Start();
            thread.Start();
            Thread.Sleep(1000);
            executor.Cancel();
            thread.Join();
            stopwatch.Stop();

            stopwatch.ElapsedMilliseconds.Should().BeInRange(lower, upper);
        }


        #region Method stubs for code coverage

        [TestMethod]
        public override void RunTests_TestDirectoryViaUserParams_IsPassedViaCommandLineArg()
        {
            base.RunTests_TestDirectoryViaUserParams_IsPassedViaCommandLineArg();
        }

        [TestMethod]
        public override void RunTests_WorkingDir_IsSetCorrectly()
        {
            base.RunTests_WorkingDir_IsSetCorrectly();
        }

        [TestMethod]
        public override void RunTests_ExternallyLinkedX86Tests_CorrectTestResults()
        {
            base.RunTests_ExternallyLinkedX86Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_StaticallyLinkedX86Tests_CorrectTestResults()
        {
            base.RunTests_StaticallyLinkedX86Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_ExternallyLinkedX64_CorrectTestResults()
        {
            base.RunTests_ExternallyLinkedX64_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_StaticallyLinkedX64Tests_CorrectTestResults()
        {
            base.RunTests_StaticallyLinkedX64Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_CrashingX64Tests_CorrectTestResults()
        {
            base.RunTests_CrashingX64Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_CrashingX86Tests_CorrectTestResults()
        {
            base.RunTests_CrashingX86Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_HardCrashingX86Tests_CorrectTestResults()
        {
            base.RunTests_HardCrashingX86Tests_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_ExternallyLinkedX86TestsInDebugMode_CorrectTestResults()
        {
            base.RunTests_ExternallyLinkedX86TestsInDebugMode_CorrectTestResults();
        }

        [TestMethod]
        public override void RunTests_WithoutBatches_NoLogging()
        {
            base.RunTests_WithoutBatches_NoLogging();
        }

        [TestMethod]
        public override void RunTests_WithSetupAndTeardownBatchesWhereSetupFails_LogsWarning()
        {
            base.RunTests_WithSetupAndTeardownBatchesWhereSetupFails_LogsWarning();
        }

        [TestMethod]
        public override void RunTests_WithSetupAndTeardownBatchesWhereTeardownFails_LogsWarning()
        {
            base.RunTests_WithSetupAndTeardownBatchesWhereTeardownFails_LogsWarning();
        }

        [TestMethod]
        public override void RunTests_WithNonexistingSetupBatch_LogsError()
        {
            base.RunTests_WithNonexistingSetupBatch_LogsError();
        }

        [TestMethod]
        public override void RunTests_WithPathExtension_ExecutionOk()
        {
            base.RunTests_WithPathExtension_ExecutionOk();
        }

        [TestMethod]
        public override void RunTests_WithoutPathExtension_ExecutionFails()
        {
            base.RunTests_WithoutPathExtension_ExecutionFails();
        }

        #endregion

    }

}