// - Christopher J Hammond http://www.chrishammond.com 
// this code is adapted from the following thread on netduino.com 
// http://forums.netduino.com/index.php?/topic/780-netduino-with-adafruit-motor-shield/page__view__findpost__p__8045
// the code I've put together is released under The MIT License (MIT)

using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace com.christoc.netduino.timelapsetank
{
    public class Program
    {
        //get an instance of the DcMotor class passing in the enumerated header M4 (netduino can access M3 and M4 on the adafruit motor shield)
        static DcMotor rightDrive = new DcMotor(MotorHeaders.M3);
        static DcMotor leftDrive = new DcMotor(MotorHeaders.M4);

        //front and rear IR sensors for controlling when the tank should turn around
        static IRSensor frontSensor = new IRSensor(Pins.GPIO_PIN_A2);
        static IRSensor rearSensor = new IRSensor(Pins.GPIO_PIN_A3);

        //currently not running any programs, here originally from https://christocnetduino.codeplex.com/SourceControl/latest/#Projects/MotorTest/
        private enum runprograms
        {
            One,
            Two,
            Three,
            Four
        } ;

        //forward and reverse directions
        private enum directions
        {
            Forward,
            Reverse
        } ;

        static int _curProgram = (int)runprograms.One;
        private static int _curDirection = (int)directions.Forward;
        private static OutputPort onboardLed = new OutputPort(Pins.ONBOARD_LED, false);

        private const long DebounceDelay = 10000000;

        private static DateTime lastTriggeredTime = new DateTime();

        private static long _lastButtonPushed;
        public static void Main()
        {
            //removed the button from the original MotorTest project
            //var switchButton = new InterruptPort(Pins.GPIO_PIN_A1, true, Port.ResistorMode.PullUp,
            //                                     Port.InterruptMode.InterruptEdgeLow);
            //switchButton.OnInterrupt += switchButton_OnInterrupt;


            //set the speed to 0 to start
            rightDrive.SetSpeed(0);
            leftDrive.SetSpeed(0);

            //set the direction of the motor
            rightDrive.Run(MotorDirection.Forward);
            leftDrive.Run(MotorDirection.Forward);
            onboardLed.Write(true);

            //wait for the IRSensors to stabilize
            Thread.Sleep(60000);

            frontSensor.SensorTriggered += FrontSensorTriggered;
            rearSensor.SensorTriggered += RearSensorTriggered;

            while (true)
            {
                //turn off the LED to let you know it is ready.
                onboardLed.Write(false);

                //when the front or rear sensor is triggered switch directions
                RunCurrentProgram();
            }
            // ReSharper disable FunctionNeverReturns
        }
        // ReSharper restore FunctionNeverReturns

        static void FrontSensorTriggered(bool triggered, DateTime time)
        {
            if (time > lastTriggeredTime.AddSeconds(15))
            {
                //if we are going forward, let's 
                if (_curDirection == (int)directions.Forward)
                {
                    // if pir is high, that means it triggered
                    onboardLed.Write(triggered);
                    if (_curDirection == 0) _curDirection = 1;
                    else _curDirection = 0;
                    lastTriggeredTime = time;
                }
            }
        }

        static void RearSensorTriggered(bool triggered, DateTime time)
        {

            if (time > lastTriggeredTime.AddSeconds(15))
            {
                if (_curDirection == (int)directions.Reverse)
                {
                    // if pir is high, that means it triggered
                    onboardLed.Write(triggered);
                    if (_curDirection == 0) _curDirection = 1;
                    else _curDirection = 0;
                    lastTriggeredTime = time;
                }
            }
        }

        static private void RunCurrentProgram()
        {
            MovementProgram();
        }

        private static void MovementProgram()
        {
            if (_curDirection == 0)
            {
                GoStraight(75, 100, MotorDirection.Forward);
            }
            else
            {
                GoStraight(75, 100, MotorDirection.Reverse);
            }
            //how long to wait between movement, initially 4 seconds
            Stop(4000);
        }

        private static void Stop()
        {
            rightDrive.SetSpeed(0);
            leftDrive.SetSpeed(0);
        }

        //stop with a wait attached
        private static void Stop(int wait)
        {
            rightDrive.SetSpeed(0);
            leftDrive.SetSpeed(0);
            Thread.Sleep(wait);
        }

        private static void GoStraight(uint speed, int lot, MotorDirection direction)
        {
            rightDrive.Run(direction);
            leftDrive.Run(direction);
            rightDrive.SetSpeed(speed);
            leftDrive.SetSpeed(speed);
            Thread.Sleep(lot);
        }

        //removed button interrupt
        //private static void switchButton_OnInterrupt(uint data1, uint data2, DateTime time)
        //{
        //    if ((DateTime.Now.Ticks - _lastButtonPushed) > DebounceDelay)
        //    {
        //        _lastButtonPushed = DateTime.Now.Ticks;
        //        if (_curProgram > 2)
        //            _curProgram = 0;
        //        else
        //        {
        //            _curProgram++;
        //        }
        //    }
        //}
    }
}
