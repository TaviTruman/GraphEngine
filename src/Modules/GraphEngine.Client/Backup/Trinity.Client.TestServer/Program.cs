﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Client.ServerSide;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.Impl;
using Trinity.Client.TestProtocols.TrinityClientTestModule;
using Trinity.Diagnostics;
using Trinity.Network;

namespace Trinity.Client.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.LogsWritten += Log_LogsWritten;
            TrinityServer server = new TrinityServer();
            server.RegisterCommunicationModule<TrinityClientModule.TrinityClientModule>();
            server.RegisterCommunicationModule<TrinityClientTestModule>();
            server.Start();

            var cmod = server.GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            var tmod = server.GetCommunicationModule<TrinityClientTestModule>();
            int i = 0;
            while (true)
            {
                var client = cmod.Clients.FirstOrDefault();
                Console.WriteLine($"{cmod.Clients.Count()} clients");
                if (client != null)
                {
                    try
                    {
                        using (var msg = new S1Writer("foo", i++))
                            client.P1(msg).ContinueWith(t =>
                            {
                                using (var rsp = t.Result)
                                {
                                    Console.WriteLine($"Client responded: {rsp.foo}, {rsp.bar}");
                                }
                            }, TaskContinuationOptions.RunContinuationsAsynchronously);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private static void Log_LogsWritten(IList<LOG_ENTRY> serverLogEntrys)
        {
            if (serverLogEntrys == null) throw new ArgumentNullException(nameof(serverLogEntrys));

            foreach (var logEntry in serverLogEntrys)
            {

                Console.WriteLine(logEntry.logLevel);

            }
        }
    }
}
