// =============================================================================
// RULE ID   : cr-dotnet-0051
// RULE NAME : Named Pipes Local
// CATEGORY  : Platform
// DESCRIPTION: Application uses local named pipes through
//              System.IO.Pipes.NamedPipeServerStream for inter-process
//              communication. Local named pipes don't work across distributed
//              cloud environments, breaking IPC mechanisms.
// =============================================================================

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace SyntheticLegacyApp.Platform
{
    public class NamedPipeIPCServer
    {
        // VIOLATION cr-dotnet-0051: NamedPipeServerStream — local machine IPC only
        private const string PipeName = "LegacyApp_CommandPipe";

        public void StartListening()
        {
            // VIOLATION cr-dotnet-0051: Creates a named pipe server on the local machine
            using (var server = new NamedPipeServerStream(
                PipeName, PipeDirection.InOut,
                maxNumberOfServerInstances: 5,
                transmissionMode: PipeTransmissionMode.Message))
            {
                Console.WriteLine($"Waiting for pipe connection on: {PipeName}");

                // VIOLATION cr-dotnet-0051: WaitForConnection blocks thread for local pipe client
                server.WaitForConnection();

                using (var reader = new StreamReader(server, Encoding.UTF8, leaveOpen: true))
                using (var writer = new StreamWriter(server, Encoding.UTF8, leaveOpen: true))
                {
                    string command = reader.ReadLine();
                    Console.WriteLine($"Received command: {command}");
                    writer.WriteLine($"ACK:{command}");
                    writer.Flush();
                }
            }
        }
    }

    public class NamedPipeIPCClient
    {
        // VIOLATION cr-dotnet-0051: NamedPipeClientStream connecting to local pipe server
        public string SendCommand(string command)
        {
            using (var client = new NamedPipeClientStream(
                ".", "LegacyApp_CommandPipe",
                PipeDirection.InOut))
            {
                // VIOLATION cr-dotnet-0051: Connect assumes both processes on same host
                client.Connect(timeout: 5000);

                using (var writer = new StreamWriter(client, leaveOpen: true))
                using (var reader = new StreamReader(client, leaveOpen: true))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine(command);
                    return reader.ReadLine();
                }
            }
        }
    }

    public class LegacyProcessBridge
    {
        // VIOLATION cr-dotnet-0051: Named pipe used as sidecar IPC — fails in multi-pod cloud deployments
        public void BroadcastConfigUpdate(string configJson)
        {
            string[] workerPipes = { "LegacyApp_Worker1", "LegacyApp_Worker2", "LegacyApp_Worker3" };

            foreach (string pipeName in workerPipes)
            {
                using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    try
                    {
                        client.Connect(2000);
                        byte[] data = Encoding.UTF8.GetBytes(configJson);
                        client.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Pipe {pipeName} unreachable: {ex.Message}");
                    }
                }
            }
        }
    }
}
