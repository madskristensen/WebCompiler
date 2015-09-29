using System;
using Microsoft.ApplicationInsights;

namespace WebCompiler
{
    /// <summary>
    /// Reports anonymous usage through ApplicationInsights
    /// </summary>
    public static class Telemetry
    {
        private static TelemetryClient _telemetry = GetAppInsightsClient();
        private const string TELEMETRY_KEY = "6e6f3a28-9a6b-4338-a03d-560756b25a40";

        private static TelemetryClient GetAppInsightsClient()
        {
            TelemetryClient client = new TelemetryClient();
            client.InstrumentationKey = TELEMETRY_KEY;
            client.Context.Component.Version = CompilerService.Version;
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();

            return client;
        }

        /// <summary>Flushes the stream to the ApplicationInsights service.</summary>
        public static void Flush()
        {
            _telemetry.Flush();
        }

        /// <summary>Tracks an event to ApplicationInsights.</summary>
        public static void TrackCompile(Config config)
        {
#if !DEBUG
            string fileName = config.GetAbsoluteInputFile();
            string extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            _telemetry.TrackEvent(extension);
#endif
        }

        /// <summary>Tracks an event to ApplicationInsights.</summary>
        public static void TrackEvent(string key)
        {
#if !DEBUG
            _telemetry.TrackEvent(key);
#endif
        }
    }
}
