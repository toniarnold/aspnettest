using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace iselenium
{
    /// <summary>
    /// IPC for sharing running status and test results in 2nd-order GUI tests between
    /// the test running in a test adapter in Visual Studio and the test running in the
    /// web server process.
    /// </summary>
    public static class TestServerIPC
    {
        // The MMFs themselves (IDisposable)
        // The parent process which calls StartServer does not share memory
        // with the child process running the tests and capturing the results,
        // thus the same fields can be used for creator and consumer.

        private static MemoryMappedFile MmfIsTestRunning;
        private static MemoryMappedFile MmfTestSummary;
        private static MemoryMappedFile MmfTestResultXml;
        private static MemoryMappedFile MmfTestResultFailedXml;

        public static void Dispose()
        {
            if (MmfIsTestRunning != null)
            {
                MmfIsTestRunning.Dispose();
                MmfIsTestRunning = null;
            }
            if (MmfTestSummary != null)
            {
                MmfTestSummary.Dispose();
                MmfTestSummary = null;
            }
            if (MmfTestResultXml != null)
            {
                MmfTestResultXml.Dispose();
                MmfTestResultXml = null;
            }
            if (MmfTestResultFailedXml != null)
            {
                MmfTestResultFailedXml.Dispose();
                MmfTestResultFailedXml = null;
            }
        }

        /// <summary>
        /// Create Memory Mapped Files to write to. New when the tests are run in the browser,
        /// existing when the tests are run from the Visual Studio process.
        /// </summary>
        public static void CreateOrOpenMmmfs()
        {
            MmfIsTestRunning = MemoryMappedFile.CreateOrOpen("is-test-running", 1);
            MmfTestSummary = MemoryMappedFile.CreateOrOpen("test-result", 100);
            MmfTestResultXml = MemoryMappedFile.CreateOrOpen("test-result-xml", 500000);
            MmfTestResultFailedXml = MemoryMappedFile.CreateOrOpen("test-result-failed-xml", 500000);

            // Initialize the tests as running such that AssertTestOK is guaranteed to wait
            // when it is reached before the web server reacted to clicking the test button.
            IsTestRunning = true;
        }

        /// <summary>
        /// Shared between processes.
        /// To be polled until the tests are finished.
        /// </summary>
        public static bool IsTestRunning
        {
            get
            {
                if (MmfIsTestRunning == null)
                    return false;
                bool running = false;
                var mutex = new Mutex(true);
                using (var stream = MmfIsTestRunning.CreateViewStream())
                {
                    var reader = new BinaryReader(stream);
                    running = reader.ReadBoolean();
                }
                mutex.ReleaseMutex();
                return running;
            }
            set
            {
                // Silently skip  finally { TestServerIPC.IsTestRunning = false; } in TestRunnerBase.Run
                if (MmfIsTestRunning != null || value == true)
                {
                    var mutex = new Mutex(true);
                    using (var stream = MmfIsTestRunning.CreateViewStream())
                    {
                        BinaryWriter writer = new BinaryWriter(stream);
                        writer.Write(value);
                    }
                    mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Shared between processes, capacity 100 chars
        /// </summary>
        public static string TestSummary
        {
            get
            {
                string result;
                var mutex = new Mutex(true);
                using (var stream = MmfTestSummary.CreateViewStream())
                {
                    var reader = new BinaryReader(stream);
                    result = reader.ReadString();
                }
                mutex.ReleaseMutex();
                return result;
            }
            set
            {
                var mutex = new Mutex(true);
                using (var stream = MmfTestSummary.CreateViewStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(value);
                }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Shared between processes, capacity 500000 chars.
        /// </summary>
        public static string TestResultXml
        {
            get
            {
                string result;
                var mutex = new Mutex(true);
                using (var stream = MmfTestResultXml.CreateViewStream())
                {
                    var reader = new BinaryReader(stream);
                    result = reader.ReadString();
                }
                mutex.ReleaseMutex();
                return result;
            }
            set
            {
                var mutex = new Mutex(true);
                using (var stream = MmfTestResultXml.CreateViewStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(value);
                }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Shared between processes, capacity 500000 chars.
        /// </summary>
        public static string TestResultFailedXml
        {
            get
            {
                string result;
                var mutex = new Mutex(true);
                using (var stream = MmfTestResultFailedXml.CreateViewStream())
                {
                    var reader = new BinaryReader(stream);
                    result = reader.ReadString();
                }
                mutex.ReleaseMutex();
                return result;
            }
            set
            {
                var mutex = new Mutex(true);
                using (var stream = MmfTestResultFailedXml.CreateViewStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(value);
                }
                mutex.ReleaseMutex();
            }
        }
    }
}