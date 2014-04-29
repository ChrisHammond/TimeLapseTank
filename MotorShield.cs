// - Christopher J Hammond http://www.chrishammond.com 
// this code is adapted from the following thread on netduino.com 
// http://forums.netduino.com/index.php?/topic/780-netduino-with-adafruit-motor-shield/page__view__findpost__p__8045
// the code I've put together is released under The MIT License (MIT)


using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace com.christoc.netduino.timelapsetank
{
    class MotorShield
    {
        
        public static Cpu.Pin Pwm_0A =  Pins.GPIO_PIN_D6; // M4
        public static Cpu.Pin Pwm_0B = Pins.GPIO_PIN_D5; // M3

        private static readonly MotorShield _instance = new MotorShield();
        public static MotorShield Instance { get { return _instance; } }

        private readonly OutputPort _motorLatch;
        private readonly OutputPort _motorClock;
        private OutputPort _motorEnable;
        private readonly OutputPort _motorData;

        internal byte LatchState;

        private MotorShield()
        {
            _motorLatch = new OutputPort(Pins.GPIO_PIN_D12, false);
            _motorClock = new OutputPort(Pins.GPIO_PIN_D4, false);
            _motorEnable = new OutputPort(Pins.GPIO_PIN_D7, false);
            _motorData = new OutputPort(Pins.GPIO_PIN_D8, false);
        }

        internal void LatchTx()
        {
            //LATCH_PORT &= ~_BV(LATCH);
            _motorLatch.Write(false);

            //SER_PORT &= ~_BV(SER);
            _motorData.Write(false);

            for (int i = 0; i < 8; i++)
            {
                //CLK_PORT &= ~_BV(CLK);
                _motorClock.Write(false);

                int mask = (1 << (7 - i));
                _motorData.Write((LatchState & mask) != 0);
                //CLK_PORT |= _BV(CLK);
                _motorClock.Write(true);
            }
            //LATCH_PORT |= _BV(LATCH);
            _motorLatch.Write(true);
        }

    }


    public enum MotorBits
    {
        Motor4A = 0,
        Motor4B = 6,
        Motor3A = 5,
        Motor3B = 7
    }

    public enum MotorDirection
    {
        Release, Forward, Reverse
    }

    public enum MotorHeaders
    {
        M3, M4
    }

}
