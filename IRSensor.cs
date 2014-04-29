// the code I've put together is released under The MIT License (MIT)
//based on http://forums.netduino.com/index.php?/topic/1246-parallax-pir-sensor-class/?hl=%2Binfrared+%2Bproximity+%2Bsensor 

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace com.christoc.netduino.timelapsetank
{

    public delegate void PIRTriggeredEventHandler(bool triggered, DateTime time);

    class IRSensor
    {
        private InterruptPort _sensor;

        public event PIRTriggeredEventHandler SensorTriggered;

        /// <summary>
        /// Initializes a new instance of the <see cref="PIRSensor"/> class.
        /// </summary>
        /// <param name="portId">The port id.</param>
        public IRSensor(Cpu.Pin portId)
        {
            _sensor = 
                new InterruptPort(
                    portId, 
                    false, 
                    Port.ResistorMode.Disabled, 
                    Port.InterruptMode.InterruptEdgeBoth);

            _sensor.OnInterrupt +=
                new NativeEventHandler(
                    (data1, data2, time) =>
                    {
                        OnSensorTriggered(data1, data2, time);
                    }
            );

        }

        /// <summary>
        /// Called when [PIR triggered].
        /// </summary>
        /// <param name="data1">The data1.</param>
        /// <param name="data2">The data2.</param>
        /// <param name="time">The time.</param>
        protected void OnSensorTriggered(uint data1, uint data2, DateTime time)
        {
            var evt = SensorTriggered;
            if (evt != null)
                evt.Invoke(data2 == 1, time);
        }
    }
}
