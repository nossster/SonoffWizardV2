using System;
using System.Linq;
using System.Windows.Forms;
using Sonoff.Core;

namespace SonoffWizardV2
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // ───── CLI mode ─────────────────────────────────────────
            // syntax: <exe> <deviceId> <ip:port> <endpoint> <0|1>
            if (args.Length >= 4)
            {
                string devId    = args[0];
                string hostPort = args[1];
                string endpoint = args[2];                     // передаём в лог
                bool   turnOn   = args.Last() == "1";

                try
                {
                    bool ok = new SonoffClient(hostPort).Switch(devId, turnOn);
                    Console.WriteLine(ok
                        ? $"[{endpoint}] {(turnOn ? "ON" : "OFF")}"
                        : "[ERROR] command failed");
                    Environment.Exit(ok ? 0 : 1);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    Environment.Exit(1);
                }
                return;
            }

            // ───── GUI mode ─────────────────────────────────────────
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
