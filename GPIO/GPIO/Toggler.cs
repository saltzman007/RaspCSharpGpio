using System;
using WiringPiLib;

namespace GPIO1
{
	public class Toggler
	{
		IWiringPiWrapper WiringPiWrapper;

		public Toggler (IWiringPiWrapper iWiringPiWarpper)
		{
			WiringPiWrapper = iWiringPiWarpper;
		}

		public void Toggle()
		{
			int pin = 18;
			
			WiringPiWrapper.WiringPiSetupGpio ();
			WiringPiWrapper.PinMode(pin, PinType.OUTPUT) ;

			while (true) 
			{
				WiringPiWrapper.DigitalWrite (pin, 0);
				WiringPiWrapper.DigitalWrite (pin, 1);
			}

		}

	}
}

