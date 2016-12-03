using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class AD_MCP3201_SPI
	{
		IWiringPiWrapper WiringPiWrapper;
		int CS;
		int Clock;
		int Data;
		public const int BitTiefe = 4096;  	//12 bit 4096 werte

		public AD_MCP3201_SPI (IWiringPiWrapper iWiringPiWarpper, int cs, int clock, int data)
		{
			CS = cs;
			Clock = clock;
			Data = data;

			WiringPiWrapper = iWiringPiWarpper;

			WiringPiWrapper.PinMode(CS, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Clock, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data, PinType.INPUT) ;

			WiringPiWrapper.DigitalWrite (CS, 1);	//unselect
			WiringPiWrapper.DigitalWrite (Clock, 1);
		}

		/// <summary>
		/// Soll Performanter sein: Setze Wert von 0 bis BitTiefe, eg 12 bit 4096 werte
		/// </summary>
		/// <param name="absoluteVale">Absolute vale.</param>
		public int ReadVoltage()
		{
			WiringPiWrapper.DigitalWrite (CS, 0);	//select

			ReadBit();	
			ReadBit();	
			ReadBit();	//Die 16. fallende Flanke kommt nach dem LSB!
			ReadBit();	

			int value = 0;

			for (int i = BitTiefe / 2; i > 0; i = i / 2)
				if (ReadBit () == 1)
					value += i;

			WiringPiWrapper.DigitalWrite (CS, 1);	//unselect

			return value;
		}


		public float ReadVoltagePercent()
		{
			return (float)ReadVoltage () / BitTiefe;
		}

		int ReadBit()
		{
			WiringPiWrapper.DigitalWrite (Clock, 0);	
			int value =  WiringPiWrapper.DigitalRead(Data);	
			WiringPiWrapper.DigitalWrite (Clock, 1);	

			return value;
		}
	}
}

