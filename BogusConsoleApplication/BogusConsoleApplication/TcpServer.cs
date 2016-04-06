using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BogusConsoleApplication
{
    internal class TcpServer
    {
        private readonly int _port;
        private readonly CancellationToken _ct;
        private readonly Func<TcpClient, CancellationToken, bool> _callback;
        private readonly ManualResetEvent _clientConnected = new ManualResetEvent(false);
        private readonly Logger _logger;
        private TcpListener _listener;
        private Task _listenerTask;

        public TcpServer(int port, CancellationToken ct, Logger logger, Func<TcpClient, CancellationToken, bool> callback)
        {
            _logger = logger;
            _callback = callback;
            _port = port;
            _ct = ct;
        }

        public void Initialize()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
        }

        public void StartListener()
        {
            _listener.Start();
            _listenerTask = Task.Run(() => ListenerThread());
        }

        public void StopListener()
        {
            _listener.Stop();

            // this needs some help
            _listenerTask.Wait();
        }

        private void ListenerThread()
        {
            while(!_ct.IsCancellationRequested)
            {
                _clientConnected.Reset();
                _logger.Log("accepting clients");

                // non-blocking
                var client = _listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);

                _logger.Log("waiting for client connection or a cancellation");
                var handleSet = WaitHandle.WaitAny(new WaitHandle[] { _ct.WaitHandle, _clientConnected });
                _logger.Log($"WaitAny returned...{handleSet}");
            }
        }

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            if (_ct.IsCancellationRequested)
            {
                _logger.Log("early termination");
                return;
            }

            // End the operation and display the received data on the console.
            TcpClient client = _listener.EndAcceptTcpClient(ar);

            // Process the connection here. (Add the client to a server table, read data, etc.)
            _logger.Log("Client connected completed");

            Task.Run(() => _callback(client, _ct));

            _logger.Log("signalling complete");
            // Signal the calling thread to continue.
            _clientConnected.Set();
        }
    }
}
