﻿using System;
using System.IO.Ports;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Communication.Serial
{
    /// <summary>
    /// Implementation of the <see cref="IBinaryConnection"/> interface for
    /// standard serial ports.
    /// </summary>
    [ExpectedConfig(typeof(SerialBinaryConfig))]
    [Plugin(LifeCycle.Transient, typeof(IBinaryConnection), Name = ConnectionName)]
    public class SerialBinaryConnection : IBinaryConnection, ILoggingComponent
    {
        /// <summary>
        /// Unique plugin name for this type
        /// </summary>
        internal const string ConnectionName = "SerialBinaryConnection";

        /// <summary>
        /// Named injected logger instance
        /// </summary>
        [UseChild("SerialConnection")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Connection configuration
        /// </summary>
        private SerialBinaryConfig _config;

        /// <summary>
        /// Validator and interpreter for incoming messages
        /// </summary>
        private readonly IMessageValidator _validator;

        /// <summary>
        /// Serial port used for the communication
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Read context for incoming data
        /// </summary>
        private IReadContext _readContext;

        /// <summary>
        /// Create connection instance
        /// </summary>
        public SerialBinaryConnection(IMessageValidator validator)
        {
            _validator = validator;
        }

        /// 
        public void Initialize(BinaryConnectionConfig config)
        {
            _config = (SerialBinaryConfig) config;
            _serialPort = SerialPortFactory.FromConfig(_config, Logger);
            _serialPort.DataReceived += OnDataReceived;
        }

        ///
        public void Start()
        {
            _readContext = _validator.Interpreter.CreateContext();
            _serialPort.Open();
            CurrentState = BinaryConnectionState.Connected;
        }

        /// 
        public void Dispose()
        {
            _serialPort.DataReceived -= OnDataReceived;
            _serialPort.Dispose();
            _serialPort = null;

            CurrentState = BinaryConnectionState.Disconnected;
        }

        /// 
        public void Reconnect()
        {
        }

        /// <inheritdoc />
        public void Reconnect(int delayMs)
        {
            throw new NotImplementedException();
        }

        ///
        public BinaryConnectionState CurrentState { get; private set; }

        ///
        public event EventHandler<BinaryConnectionState> NotifyConnectionState;

        /// 
        public void Send(BinaryMessage data)
        {
            // Create bytes from message
            var bytes = _validator.Interpreter.SerializeMessage(data);
            _serialPort.Write(bytes, 0, bytes.Length);
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            int available;
            while ((available = _serialPort.BytesToRead) > 0)
            {
                _serialPort.Read(_readContext.ReadBuffer, _readContext.CurrentIndex, available);
                _validator.Interpreter.ProcessReadBytes(_readContext, available, PublishCompleteMessage);
            }
        }

        private void PublishCompleteMessage(BinaryMessage binaryMessage)
        {
            Received(this, binaryMessage);
        }

        ///
        public event EventHandler<BinaryMessage> Received;
    }
}