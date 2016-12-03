using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class DA_8562FPZ_Parallel
	{
		IWiringPiWrapper WiringPiWrapper;

		int Data0 = 17;		//LSB
		int Data1 = 18;
		int Data2 = 27;
		int Data3 = 22;
		int Data4 = 23;
		int Data5 = 24;
		int Data6 = 25;
		int Data7 = 4;		//Bis hier sollen sie parallel als digitalWriteByte geschrieben werden
		int Data8 = 20;
		int Data9 = 21;
		int Data10 = 13;
		int Data11 = 19;	//MSB

		int CE = 26;
		public const int BitTiefe = 4096;  	//12 bit 4096 werte

		public DA_8562FPZ_Parallel (IWiringPiWrapper wiringPiWarpper)
		{
			//Soll mit DigitalWritebyte arbeiten, das schreibt in die ersten 8 GPIO parallel (oder mehr?)
			//Daher gibt es hier aber eine feste Verdrahtung!
			WiringPiWrapper = wiringPiWarpper;

			WiringPiWrapper.PinMode(Data0, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data1, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data2, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data3, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data4, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data5, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data6, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data7, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data8, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data9, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data10, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(Data11, PinType.OUTPUT) ;

			WiringPiWrapper.PinMode(CE, PinType.OUTPUT) ;

			WiringPiWrapper.DigitalWrite (CE, 1);

			SetVoltage (0);
		}

		/// <summary>
		/// Soll Performanter sein: Setze Wert von 0 bis BitTiefe, eg 12 bit 4096 werte
		/// </summary>
		/// <param name="absoluteVale">Absolute vale.</param>
		public void SetVoltage(int absoluteVale)
		{
			WiringPiWrapper.DigitalWriteByte (absoluteVale & 0xff);

			WiringPiWrapper.DigitalWrite (Data8, (absoluteVale & 256) > 0 ? 1 : 0);
			WiringPiWrapper.DigitalWrite (Data9, (absoluteVale & 512) > 0 ? 1 : 0);
			WiringPiWrapper.DigitalWrite (Data10, (absoluteVale & 1024) > 0 ? 1 : 0);
			WiringPiWrapper.DigitalWrite (Data11, (absoluteVale & 2048) > 0 ? 1 : 0);

			WiringPiWrapper.DigitalWrite (CE, 0);
			WiringPiWrapper.DigitalWrite (CE, 1);
		}
	}
}

