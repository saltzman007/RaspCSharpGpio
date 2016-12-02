using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiringPiLib
{

	public class WirinPiWrapper : IWiringPiWrapper
	{
		public int WiringPiSetupGpio ()
		{
			return WiringPiWrapperDirect.WiringPiSetupGpio ();
		}
		public int DigitalRead(int pin)
		{
			return WiringPiWrapperDirect.digitalRead(pin);
		}
		public void DigitalWrite(int pin, int value)
		{
			WiringPiWrapperDirect.digitalWrite (pin, value);
		}
		public void DigitalWriteByte(int value)
		{
			WiringPiWrapperDirect.digitalWriteByte (value);
		}

		public void PullUpDnControl(int pin, PullUpType value)
		{
			WiringPiWrapperDirect.pullUpDnControl (pin, value);
		}
		public void PinMode(int pin, PinType mode)
		{
			WiringPiWrapperDirect.pinMode (pin, mode);
		}
		public void DelayMicroseconds (uint howLong)
		{
			WiringPiWrapperDirect.delayMicroseconds (howLong);
		}
		public uint Millis()
		{
			return WiringPiWrapperDirect.millis ();
		}
		public int WiringPiISR(int pin, EdgeType edgeType, ISRCallback method)
		{
			return WiringPiWrapperDirect.wiringPiISR (pin, edgeType, method);
		}
		public void PwmSetMode(PwmType mode)
		{
			WiringPiWrapperDirect.pwmSetMode (mode);
		}
		public void PwmSetRange (uint range)
		{
			WiringPiWrapperDirect.pwmSetRange (range);
		}
		public void PwmSetClock(int divisor)
		{
			WiringPiWrapperDirect.pwmSetClock(divisor);
		}
		public void PwmWrite(int pin, int value)
		{
			WiringPiWrapperDirect.pwmWrite(pin, value);
		}
	}

}