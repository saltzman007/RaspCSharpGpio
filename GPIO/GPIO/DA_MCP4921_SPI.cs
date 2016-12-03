using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	///Funktioniert mit 3 und 5 Volt Vcc!	
	public class DA_MCP4921_SPI
	{
		IWiringPiWrapper WiringPiWrapper;
		int CS;
		int Clock;
		int SDI;
		public const int BitTiefe = 4096;  	//12 bit 4096 werte

		public DA_MCP4921_SPI (IWiringPiWrapper iWiringPiWarpper, int cs, int clock, int sdi)
		{
			CS = cs;
			Clock = clock;
			SDI = sdi;

			WiringPiWrapper = iWiringPiWarpper;

			WiringPiWrapper.PinMode(CS, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Clock, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(SDI, PinType.OUTPUT) ;

			WiringPiWrapper.DigitalWrite (CS, 1);	//unselect
		}

		/// <summary>
		/// Soll Performanter sein: Setze Wert von 0 bis BitTiefe, eg 12 bit 4096 werte
		/// </summary>
		/// <param name="absoluteVale">Absolute vale.</param>
		public void SetVoltage(int absoluteVale)
		{
			WiringPiWrapper.DigitalWrite (CS, 0);	//select

			//Send Config Bits
			SendBit(0);		//Ausgang A, Seite 18 DataSheet
			SendBit(0);		//Unbuffered
			SendBit(1);		//Direkt Prozente der ReferenzSpannung
			SendBit(1);		//Output enabled

			for (int i = BitTiefe; i > 0; i = i / 2)
				if((i & absoluteVale) > 0)
					SendBit (1);
			else
					SendBit (0);

			WiringPiWrapper.DigitalWrite (CS, 1);	//unselect
		}

		void SendBit(int bit)
		{
			WiringPiWrapper.DigitalWrite (SDI, bit);	
			WiringPiWrapper.DigitalWrite (Clock, 1);	
			WiringPiWrapper.DigitalWrite (Clock, 0);	
		}
	}
}

