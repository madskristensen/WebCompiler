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

        /// <summary>Determines if telemetry should be reported.</summary>
        public static bool Enabled { get; set; } = true;

        private static TelemetryClient GetAppInsightsClient()
        {
            TelemetryClient client = new TelemetryClient();
            client.InstrumentationKey = TELEMETRY_KEY;
            client.Context.Component.Version = CompilerService.Version;
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();

            return client;
        }

        /// <summary>
        /// The device name is what identifies what kind of device is calling
        /// </summary>
        public static void SetDeviceName(string name)
        {
            _telemetry.Context.Device.Model = name;
        }

        /// <summary>Tracks an event to ApplicationInsights.</summary>
        public static void TrackCompile(Config config)
        {
#if !DEBUG
            if (Enabled)
            {
                string fileName = config.GetAbsoluteInputFile();
                string extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

                _telemetry.TrackEvent(extension);
            }
#endif
        }

        /// <summary>Tracks an event to ApplicationInsights.</summary>
        public static void TrackEvent(string key)
        {
#if !DEBUG
            if (Enabled)
            {
                _telemetry.TrackEvent(key);
            }
#endif
        }

        /// <summary>Tracks any exception.</summary>
        public static void TrackException(Exception ex)
        {
#if !DEBUG
            if (Enabled)
            {
                var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                telex.HandledAt = Microsoft.ApplicationInsights.DataContracts.ExceptionHandledAt.UserCode;
                _telemetry.TrackException(telex);
            }
#endif
        }
    }
}
