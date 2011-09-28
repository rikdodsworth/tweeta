using System;
using System.Diagnostics;
using System.Windows.Threading;
using WP7HelperFX.Common.Logging;

namespace WP7HelperFX.Common
{
    public static class MemoryProfiler
    {
        private const bool MemoryProfilingOutputEnabled = true;
        private const bool MemoryOutputInfoEnabled = true;

        private const int MaxSamples = 256;
        private const int SampleInterval = 5;

        private static double[] memorySamples = new double[MaxSamples];
        private static double[] memorySamplesMan = new double[MaxSamples];

        private static int sampleIndex;
        private static DispatcherTimer timer;

        [Conditional("DEBUG")]
        public static void Initialize()
        {
            if (MemoryProfilingOutputEnabled)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(SampleInterval);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        [Conditional("DEBUG")]
        public static void ShutDown()
        {
            if (MemoryProfilingOutputEnabled)
            {
                timer.Stop();
                OutputMemoryStats();
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            double native = (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage") / 1048576.0; ;
            memorySamples[sampleIndex] = native;

            double managed = GC.GetTotalMemory(false) / 1048576.0;
            memorySamplesMan[sampleIndex] = managed;

            sampleIndex++;

            if (sampleIndex >= MaxSamples)
                sampleIndex = 0;

            if (MemoryOutputInfoEnabled)
                Debug.WriteLine(string.Format("Mem usage  : Nat {0:0.00}MB, Mgd {1:0.00}MB ", native, managed));
        }

        private static void OutputMemoryStats()
        {
            if (Log.Verbosity < Verbosity.INFO)
                return;

            Log.Info("Memory usage stats (MB)");
            Log.Info("Native\t(current, \t\tpeak)");

            PrintMemStats(memorySamples);

            Log.Info("Managed\t(current, \t\tpeak)");
            Log.Info("End memory usage stats");

            PrintMemStats(memorySamplesMan);
        }

        private static void PrintMemStats(double[] values)
        {
            double peak = 0;

            for (int i = 0; i < MaxSamples && i < sampleIndex; i++)
            {
                double val = values[i];

                if (val > peak)
                    peak = val;

                Log.Info(string.Format("    {0:0.00}, \t{1:0.00}", val, peak));
            }
        }
    }

}
