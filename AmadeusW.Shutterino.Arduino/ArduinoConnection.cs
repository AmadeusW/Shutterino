﻿using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.Arduino
{
    public class ArduinoConnection
    {
        RemoteDevice _arduino;
        NetworkSerial _connection;
        bool _connected;
        byte _servoPin;
        byte _servoIdle;
        byte _servoOff;
        byte _servoPressed;

        public ArduinoConnection(byte servoPin, byte servoIdle, byte servoOff, byte servoPressed)
        {
            _servoPin = servoPin;
            _servoIdle = servoIdle;
            _servoOff = servoOff;
            _servoPressed = servoPressed;
        }

        public async Task<bool> Connect(string host, ushort port)
        {
            if (String.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Please provide hostname", nameof(host));

            _connection = new NetworkSerial(new Windows.Networking.HostName(host), port);
            _arduino = new RemoteDevice(_connection);

            _connection.ConnectionEstablished += _connection_ConnectionEstablished;
            _connection.ConnectionFailed += _connection_ConnectionFailed;
            _arduino.DeviceReady += _arduino_DeviceReady;
            _connection.begin(115200, SerialConfig.SERIAL_8N1);
            return true;
        }

        private void _arduino_DeviceReady()
        {
            _connected = true;
        }

        private void _connection_ConnectionFailed(string message)
        {
            throw new Exception(message);
        }

        private void _connection_ConnectionEstablished()
        {
            _connected = true;
            _arduino.pinMode(_servoPin, PinMode.SERVO);
            _arduino.analogWrite(_servoPin, _servoIdle);
        }

        public async Task<bool> Disconnect()
        {
            _arduino.analogWrite(_servoPin, _servoOff);
            await Task.Delay(100);
            _connection.end();
            _arduino.Dispose();
            _connection.Dispose();
            return true;
        }

        public async Task<bool> MoveServo()
        {
            if (_connected)
            {
                _arduino.analogWrite(_servoPin, _servoPressed);
                await Task.Delay(30);
                _arduino.analogWrite(_servoPin, _servoIdle);
                await Task.Delay(30);
                return true;
            }
            return false;
        }
    }
}
