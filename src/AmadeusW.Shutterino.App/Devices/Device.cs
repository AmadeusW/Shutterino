﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace AmadeusW.Shutterino.App.Devices
{
    public abstract class Device
    {
        public bool IsAvailable { get; protected set; } = false;

        public abstract Task<bool> InitializeAsync();
        public abstract Task CleanUpAsync();
    }
}
