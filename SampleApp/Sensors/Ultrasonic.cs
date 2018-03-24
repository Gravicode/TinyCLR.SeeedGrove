using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace SampleApp.Sensors
{
    public class HC_SR04
    {
        private GpioPin portOut;
        private GpioChangeReader interIn;
        private long beginTick;
        private long endTick;
        private long minTicks = 0;  // System latency, 
                                    /// <summary>
                                    /// Constructor
                                    /// </summary>
                                    /// <param name="pinTrig">Netduino pin connected to the HC-SR04 Trig pin</param>
                                    /// <param name="pinEcho">Netduino pin connected to the HC-SR04 Echo pin</param>
        public HC_SR04(int pinTrig, int pinEcho)
        {
            portOut = GpioController.GetDefault().OpenPin(pinTrig);
            portOut.Write(GpioPinValue.Low);
            portOut.SetDriveMode(GpioPinDriveMode.Output);

            interIn = new GpioChangeReader(pinEcho, GpioPinDriveMode.InputPullUp);//GpioController.GetDefault().OpenPin(pinEcho);//new InterruptPort(pinEcho, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            //interIn.OnInterrupt += new NativeEventHandler(interIn_OnInterrupt);
            interIn.SetDriveMode(GpioPinDriveMode.InputPullUp);
            interIn.ValueChanged += interIn_OnInterrupt;
            minTicks = 4000L;

        }

        /// <summary>
        /// Trigger a sensor reading
        /// 
        /// </summary>
        /// <returns>Number of mm to the object</returns>
        public long Ping()
        {
            // Reset Sensor
            portOut.Write(true);
            Thread.Sleep(1);

            // Start Clock
            endTick = 0L;
            beginTick = System.DateTime.Now.Ticks;
            // Trigger Sonic Pulse
            portOut.Write(false);

            // Wait 1/20 second (this could be set as a variable instead of constant)
            Thread.Sleep(50);

            if (endTick > 0L)
            {
                // Calculate Difference
                long elapsed = endTick - beginTick;

                // Subtract out fixed overhead (interrupt lag, etc.)
                elapsed -= minTicks;
                if (elapsed < 0L)
                {
                    elapsed = 0L;
                }

                // Return elapsed ticks
                return elapsed * 10 / 636;
                ;
            }

            // Sonic pulse wasn't detected within 1/20 second
            return -1L;
        }

        /// <summary>
        /// This interrupt will trigger when detector receives back reflected sonic pulse       
        /// </summary>
        /// <param name="data1">Not used</param>
        /// <param name="data2">Not used</param>
        /// <param name="time">Transfer to endTick to calculated sound pulse travel time</param>
        void interIn_OnInterrupt(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // Save the ticks when pulse was received back
            endTick = time.Ticks;
        }
    }
}
