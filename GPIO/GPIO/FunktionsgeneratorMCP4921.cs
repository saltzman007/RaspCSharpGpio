using System;
using WiringPiLib;
using System.Collections.Generic;

namespace GPIO1
{
	//WiringPi Toggle = 1MHz
	//1 Output = cs + 16 bit + cs = 2 + 16*3 = 50 Toggles / Periode
	//Bei 10 Werten / periode == 500 Toggles / Periode   --> 2000 Hz
	//Für diese Methodik Optimum erreicht
	//Optimierung: schneller Toggeln (Faktor 4)
	//und parallel statt Seriell arbeiten! (Faktor 12)
	
	public class FunktionsgeneratorMCP4921
	{
		public enum Wellenform
		{
			Rechteck,
			Sinus,
			Saege,
			Dreieck
		}

		const int BitTiefe = DA_MCP4921_SPI.BitTiefe;
		//Sinus: In einer Periode kommt jeder Wert 2 mal vor, also haben wir 4096 * 2 definierte Punkte
		//Values von 0 bis 1
		int[] WaveTable = new int[BitTiefe];	//Frequenzunabhaengig!

		WirinPiWrapper wiringPiLib = new WirinPiWrapper();
		DA_MCP4921_SPI da;

		public FunktionsgeneratorMCP4921(Wellenform wellenform)
		{
			wiringPiLib.WiringPiSetupGpio();

			da = new DA_MCP4921_SPI(wiringPiLib, 18, 23, 25);

			GenerateWaveTable(wellenform);
		}

		public void Generate(int frequenz)
		{

			long periodendauer = 10000000 / frequenz;	//In 0,1 Mikrosekunden == in Ticks

			while (true) 
			{
				long ticks = DateTime.Now.Ticks % periodendauer;	//Number of 100 ns Ticks = 0,1 Mikrosekunden

				long position = (ticks * BitTiefe) / periodendauer;

				da.SetVoltage (WaveTable[position]);
			}
		}

		private void GenerateWaveTable(Wellenform wellenform)
		{
			for (int i = 0; i < BitTiefe; i++)
				WaveTable[i] = (int)(WaveValue (wellenform, i) * BitTiefe);			
		}

		private float WaveValue(Wellenform wellenform, int value)
		{
			switch (wellenform) 
			{
			case Wellenform.Saege:

				return ((float)value) / BitTiefe;

				case Wellenform.Dreieck:
					{
						float fvalue = (2 * (float)value) / BitTiefe;

						if (fvalue > 1)
							fvalue = 2 - fvalue;

						return fvalue;
					}
						
				case Wellenform.Rechteck:
					{
						if (value < BitTiefe / 2)
							return 0;
						return 1;
					}

				case Wellenform.Sinus:
					{
						double degrees = (double)value / (double)BitTiefe * 360;
						double sinus = Math.Sin (Math.PI / 180 * degrees);	
						return (float)(sinus / 2) + 1;
					}
				default: 
					return 0;
			}
		}
	}
}

