using System;
using Sonoff.Core;

namespace SonoffWizardV2
{
    internal static class CliEntry
    {
        public static void Run(string[] args)
        {
            // Expected syntax: <exe> <deviceId> <host:port> <0|1>
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: SonoffWizardV2.exe <deviceId> <host:port> <0|1>");
                Environment.Exit(2);
            }

            string deviceId = args[0];
            string hostPort = args[1];

            if (!int.TryParse(args[2], out int action) || (action != 0 && action != 1))
            {
                Console.WriteLine("Action must be 0 (off) or 1 (on).");
                Environment.Exit(2);
            }

            var client = new SonoffClient(hostPort);

            try
            {
                bool ok = client.Switch(deviceId, action == 1);
                Console.WriteLine(ok
                    ? $"Device {deviceId} {(action == 1 ? "ON" : "OFF")}"
                    : "Command failed.");
                Environment.Exit(ok ? 0 : 1);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Environment.Exit(1);
            }
        }
    }
}
