using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class OszilloskopAD_MCP3201_SPI
	{
		static WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
		AD_MCP3201_SPI dac;

		public void DoVerySmartDisplay()
		{
			wiringPiLib.WiringPiSetupGpio ();
			dac = new AD_MCP3201_SPI(wiringPiLib, 26, 13, 19);

			while(true)
			{
				Console.WriteLine(dac.ReadVoltage());
				//Thread.Sleep (10);
			}
		}
	}
}

