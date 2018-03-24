using GHIElectronics.TinyCLR.Devices.Gpio;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace SeeedGroveStarterKit
{
    public class UltrasonicRanger
    {
        private readonly GpioPulseReaderWriter _pulse;

        /// <summary>
        /// Use to correct measurement in linear way:  result = value * A + B (default: 0.81f)
        /// </summary>
        public float A = 0.81f;

        /// <summary>
        /// Use to correct measurement in linear way:  result = value * A + B (default: 2.11f)
        /// </summary>
        public float B = 2.11f;

        /// <summary>
        /// Constructor of HC-SR04
        /// </summary>
        /// <param name="signalPin">pin connected to trigger and echo pin</param>
        public UltrasonicRanger(int signalPin)
        {
            _pulse = new GpioPulseReaderWriter(GpioPulseReaderWriter.Mode.EchoDuration, true, 10, signalPin);
        }

        /// <summary>
        /// Get distance in centimeters
        /// </summary>
        /// <returns>Value return is in centimeters</returns>
        public float ReadCentimeters()
        {
            return (float)(_pulse.Read() * 17 / 1000.0) * A + B;
        }

        /// <summary>
        /// Get distance in inches
        /// </summary>
        /// <returns>Value return is in inches</returns>
        public double ReadInches()
        {
            return ReadCentimeters() / 2.54;
        }

    }

    public class DistanceSensor
    {
        public GpioPin _pin { get; set; }
        static long MicrosDiff(long begin, long end)
        {
            return end - begin;
        }
        static long micros()
        {
            var ticks = DateTime.Now.Ticks;
            long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);
            return microseconds;
        }
        long pulseIn(bool state, long timeout = 1000000)
        {
            var begin = micros();

            // wait for any previous pulse to end
            while (_pin.Read() == GpioPinValue.High) if (MicrosDiff(begin, micros()) >= timeout) return 0;

            // wait for the pulse to start
            while (!(_pin.Read() == GpioPinValue.High)) if (MicrosDiff(begin, micros()) >= timeout) return 0;
            var pulseBegin = micros();

            // wait for the pulse to stop
            while (_pin.Read() == GpioPinValue.High) if (MicrosDiff(begin, micros()) >= timeout) return 0;
            var pulseEnd = micros();

            return MicrosDiff(pulseBegin, pulseEnd);
        }

        public DistanceSensor(int SignalPin)
        {
            _pin = GpioController.GetDefault().OpenPin(SignalPin);


        }


        /*The measured distance from the range 0 to 400 Centimeters*/
        public long MeasureInCentimeters()
        {
            //pinMode(_pin, OUTPUT);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
            _pin.Write(GpioPinValue.Low);
            Thread.Sleep(2);
            _pin.Write(GpioPinValue.High);
            //digitalWrite(_pin, HIGH);
            Thread.Sleep(5);
            _pin.Write(GpioPinValue.Low);

            //digitalWrite(_pin, LOW);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            //pinMode(_pin, INPUT);
            long duration;
            duration = pulseIn(true);
            long RangeInCentimeters;
            RangeInCentimeters = duration / 29 / 2;
            return RangeInCentimeters;
        }
        /*The measured distance from the range 0 to 157 Inches*/
        public long MeasureInInches()
        {
            _pin.SetDriveMode(GpioPinDriveMode.Output);
            //pinMode(_pin, OUTPUT);
            _pin.Write(GpioPinValue.Low);
            //digitalWrite(_pin, LOW);
            Thread.Sleep(2);

            //delayMicroseconds(2);
            _pin.Write(GpioPinValue.High);
            //digitalWrite(_pin, HIGH);
            Thread.Sleep(5);
            //delayMicroseconds(5);
            _pin.Write(GpioPinValue.Low);
            //digitalWrite(_pin, LOW);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            //pinMode(_pin, INPUT);
            long duration;
            duration = pulseIn(true);
            long RangeInInches;
            RangeInInches = duration / 74 / 2;
            return RangeInInches;
        }
    }
}
