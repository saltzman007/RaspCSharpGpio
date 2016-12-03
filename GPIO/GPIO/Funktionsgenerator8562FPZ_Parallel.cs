using System;
using WiringPiLib;
using System.Collections.Generic;

namespace GPIO1
{
	public class Funktionsgenerator8562FPZ_Parallel
	{
		private float eigenfrequenz;	//Die ausgegebene Frequenz, wenn der WaveTable ohne Delay komplett ausgegeben wird, also 2*4096 Werte

		public enum Wellenform
		{
			Rechteck,
			Sinus,
			Saege,
			Dreieck
		}

		const int BitTiefe = DA_8562FPZ_Parallel.BitTiefe;
		const int WaveTableLength = BitTiefe * 2;	//Muss sonst in der Schleife berechnet werden, macht 3 %
		//Sinus: In einer Periode kommt jeder Wert 2 mal vor, also haben wir 4096 * 2 definierte Punkte
		//Values von 0 bis 1
		int[] WaveTable = new int[WaveTableLength];	//Frequenzunabhaengig!

		WirinPiWrapper wiringPiLib = new WirinPiWrapper();
		DA_8562FPZ_Parallel da;

		public Funktionsgenerator8562FPZ_Parallel(Wellenform wellenform)
		{
			wiringPiLib.WiringPiSetupGpio();

			da = new DA_8562FPZ_Parallel(wiringPiLib);

			GenerateWaveTable(wellenform);

			GetEigenFrequenz ();
		}

		public void GetEigenFrequenz()
		{
//			int count = 1000000;
//			DateTime start = DateTime.Now;
//			for(int i = 0; i < count; i++)
//				da.SetVoltage(0);
//
//			DateTime end = DateTime.Now;
//			TimeSpan dauer = new TimeSpan (end.Ticks - start.Ticks);
//			Console.WriteLine (count + " mal set voltage dauert " + dauer.TotalMilliseconds + "ms.");

			int counter = 0;

			//.net warmup
			for(counter = 0; counter < 10000; counter++)
				da.SetVoltage (0);

			DateTime end = DateTime.Now;	//Die Zeit zum Allokieren des end DateTimes in der Schleife verfälscht bis 4 sec erheblich!!!
			DateTime start = DateTime.Now;
			for(counter = 0; counter < BitTiefe * 10; counter++)
				da.SetVoltage (WaveTable[counter % (WaveTableLength)]);
			
			end = DateTime.Now;
			TimeSpan dauer = new TimeSpan (end.Ticks - start.Ticks);

			eigenfrequenz = 5000 / (float)dauer.TotalMilliseconds;	//10 Durchläufe sind 5 Schwingungen, 1000ms = 1 sec
			Console.WriteLine(eigenfrequenz);
		}


		public void Generate(int frequenz)
		{
			float factor = frequenz / eigenfrequenz;

			int counter = 0;

			while (true) 
			{
				da.SetVoltage (WaveTable[(int)(counter++ * factor) % (WaveTableLength)]);
			}
		}

		private void GenerateWaveTable(Wellenform wellenform)
		{
			for (int i = 0; i < BitTiefe * 2; i++)
				WaveTable[i] = (int)(WaveValue (wellenform, i) * (BitTiefe -1));			
		}

		private float WaveValue(Wellenform wellenform, int value)
		{
			switch (wellenform) 
			{
			case Wellenform.Saege:

				return ((float)value / 2) / BitTiefe;

				case Wellenform.Dreieck:
					{
						float fvalue = ((float)value) / BitTiefe;

						if (fvalue > 1)
							fvalue = 2 - fvalue;

						return fvalue;
					}
						
				case Wellenform.Rechteck:
					{
						if (value < BitTiefe)
							return 0;
						return 1;
					}

				case Wellenform.Sinus:
					{
						double degrees = (double)value / (2 * (double)BitTiefe) * 360;
						double sinus = Math.Sin (Math.PI / 180 * degrees);	
						return (float)(sinus / 2) + 0.5f;
					}
				default: 
					return 0;
			}
		}
	}
}

