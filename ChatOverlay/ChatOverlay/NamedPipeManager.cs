using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MaJiCSoft.ChatOverlay
{
    public class NamedPipeManager
    {
        public class NamedPipeStream
        {
            private Stream stream;

            internal NamedPipeStream(Stream stream)
            {
                this.stream = stream;
            }

            public int WriteStream(byte[] data)
            {
                int len = data.Length;
                if (len > UInt16.MaxValue)
                {
                    len = (int)UInt16.MaxValue;
                }
                byte[] lenBytes = BitConverter.GetBytes((Int16)len);
                stream.WriteByte(lenBytes[1]);
                stream.WriteByte(lenBytes[0]);
                stream.Write(data, 0, len);
                stream.Flush();

                return len + 2;
            }

            public byte[] ReadStream()
            {
                int lenBytes;
                lenBytes = (stream.ReadByte()) * 256;
                lenBytes += (stream.ReadByte());
                byte[] data = new byte[lenBytes];
                stream.Read(data, 0, lenBytes);
                return data;
            }
        }

        public string NamedPipeName { get; private set; }
        public Guid Guid { get; private set; }
        public event EventHandler<string> ReceiveString;

        private const string EXIT_STRING = "__EXIT__";
        private Task pipeServerTask;
        private CancellationTokenSource cts;

        private readonly ILogger logger;

        public NamedPipeManager(ILogger<NamedPipeManager> logger)
        {
            this.logger = logger;
            
            var assembly = Assembly.GetExecutingAssembly();
            Guid = assembly.GetType().GUID;
            NamedPipeName = assembly.GetName().Name;
        }

        /// <summary>
        /// Starts a new Pipe server on a new thread
        /// </summary>
        public void StartServer(CancellationToken ctx)
        {
            if (cts != null) return;

            cts = CancellationTokenSource.CreateLinkedTokenSource(ctx);
            pipeServerTask = Task.Run(() => Server(NamedPipeName, logger, cts.Token), cts.Token);
        }

        public NamedPipeStream GetClientStream(int timeout = 1000)
        {
            var client = new NamedPipeClientStream(".", NamedPipeName, PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Identification);
            client.Connect(timeout);

            var pipeClient = new NamedPipeStream(client);
            var server = pipeClient.ReadStream();
            if (server.SequenceEqual(Guid.ToByteArray()))  // Sanity check - should be talking to the other already running instance
            {
                logger.LogInformation($"Connected to named pipe: {NamedPipeName}");
                return pipeClient;
            }
            client.Close();
            throw new IOException($"Server validation error connecting: {NamedPipeName}");
            return null;
        }

        private void Server(string pipeName, ILogger logger, CancellationToken token)
        {
            logger.LogInformation($"Pipe Server Started: {pipeName}");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        using (var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1))
                        {
                            server.WaitForConnectionAsync(token).Wait(token);
                            if (!token.IsCancellationRequested)
                            {
                                var serverStream = new NamedPipeStream(server);

                                serverStream.WriteStream(Guid.ToByteArray());

                                var text = Encoding.UTF8.GetString(serverStream.ReadStream());

                                if (text == EXIT_STRING)
                                    break;

                                OnReceiveString(text);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        logger.LogError(e, "Exception in NamedPipeManager");
                    }
                }
            }
            catch(OperationCanceledException ex)
            {
                // Token got cancelled...
            }
            logger.LogInformation($"Pipe Server Exiting ({cts.IsCancellationRequested})...");
        }

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnReceiveString(string text) => ReceiveString?.Invoke(this, text);

        /// <summary>
        /// Shuts down the pipe server
        /// </summary>
        public void StopServer()
        {
            cts.Cancel();
        }
    }
}
