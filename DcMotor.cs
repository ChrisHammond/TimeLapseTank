// - Christopher J Hammond http://www.chrishammond.com 
// this code is adapted from the following thread on netduino.com 
// http://forums.netduino.com/index.php?/topic/780-netduino-with-adafruit-motor-shield/page__view__findpost__p__8045
// the code I've put together is released under The MIT License (MIT)

using System;
using SecretLabs.NETMF.Hardware;

namespace com.christoc.netduino.timelapsetank
{
   class DcMotor
    {

       private readonly SecretLabs.NETMF.Hardware.PWM _pwm;
        private readonly byte _motorBitA;
        private readonly byte _motorBitB;

        public DcMotor(MotorHeaders header)
        {
            switch (header)
            {
                case MotorHeaders.M3:
                    _motorBitA = (int)MotorBits.Motor3A;
                    _motorBitB = (int)MotorBits.Motor3B;
                    _pwm = new PWM(MotorShield.Pwm_0B);
                    break;
                case MotorHeaders.M4:
                    _motorBitA = (int)MotorBits.Motor4A;
                    _motorBitB = (int)MotorBits.Motor4B;
                    _pwm = new PWM(MotorShield.Pwm_0A);
                    break;
                default:
                    throw new InvalidOperationException("Invalid motor header specified.");
            }

            MotorShield.Instance.LatchState &= (byte)(~(1 << _motorBitA) & ~(1 << _motorBitB));
            MotorShield.Instance.LatchTx();

            _pwm.SetPulse(100, 0);
        }

        public void Run(MotorDirection dir)
        {
            switch (dir)
            {
                case MotorDirection.Release:
                    MotorShield.Instance.LatchState &= (byte)(~(1 << _motorBitA));
                    MotorShield.Instance.LatchState &= (byte)(~(1 << _motorBitB));
                    break;
                case MotorDirection.Forward:
                    MotorShield.Instance.LatchState |= (byte)(1 << _motorBitA);
                    MotorShield.Instance.LatchState &= (byte)(~(1 << _motorBitB));
                    break;
                case MotorDirection.Reverse:
                    MotorShield.Instance.LatchState &= (byte)(~(1 << _motorBitA));
                    MotorShield.Instance.LatchState |= (byte)(1 << _motorBitB);
                    break;
                default:
                    throw new InvalidOperationException("Invalid motor direction specified");
            }

            MotorShield.Instance.LatchTx();
        }

        public void SetSpeed(uint speed)
        {
            if (speed > 100)
            {
                speed = 100;
            }

            _pwm.SetDutyCycle(speed);
        }

    }
}
