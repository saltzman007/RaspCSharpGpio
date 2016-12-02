using System;
using System.Threading;
using WiringPiLib;
using System.Collections.Generic;
using System.Text;


namespace Humidor
{
	class MainClass
	{
		private class Atmo
		{
			public int Temperatur{ get; set;}
			public int RelativeHumidity{ get; set;}
		}

		public static void Main (string[] args)
		{
			do{
				GenerateHumidity();	//Initialize DampfPIN 

				try
				{
					while (GetAtmo().RelativeHumidity <  69)	//www.humidorbau.de klingt vernünftig, 
					{
						GenerateHumidity();
						Thread.Sleep(60000);	//60 sec zwischen Verdampfen und Messen
					}

					Thread.Sleep(15*60*1000);	//Alle ViertelStunde regulieren
				}
				catch(Exception e)
				{
					Console.WriteLine(DateTime.Now + e.Message);
				}

			}while (true);
		}

		static Int64 GetByts (List<KeyValuePair<int, int>> intervals, int GrenzWert)
		{
			if (intervals.Count < 80)
				throw new ArgumentOutOfRangeException ("Zu wenig Bits empfangen.");

			Int64 result = 0; 
			Int64 mask = 1; 

			intervals.Reverse ();
			StringBuilder binary = new StringBuilder();

			for(int i = 0; i < 80; i++)
			{
				KeyValuePair<int, int> kvp = intervals[i];
				
				if (kvp.Key == 1) 
				{
					if (kvp.Value > GrenzWert) 
					{
						binary.Append ("1");
						result = result | mask;
					} 
					else 
					{
						binary.Append ("0");
					}
					mask = mask << 1;
				}
			}
			return result;
		}

		static void DHT22Communication (int[] Counter)
		{
			int value;
			int lastValue = 1;
			int transitionCount = 0;
			int loopCount = 0;
			int loops = Counter.Length;

			int DataPin = 4;

			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
			wiringPiLib.WiringPiSetupGpio ();
			Thread.Sleep (2);
			wiringPiLib.PinMode (DataPin, PinType.INPUT);
			//Um den ersten Aufruf nicht in der Messzeit zu haben
			value = wiringPiLib.DigitalRead (DataPin);
			Thread.Sleep (2);
			wiringPiLib.PinMode (DataPin, PinType.OUTPUT);
			Thread.Sleep (2);
			wiringPiLib.DigitalWrite (DataPin, 1);
			Thread.Sleep (2000);
			wiringPiLib.DigitalWrite (DataPin, 0);
			Thread.Sleep (1);
			wiringPiLib.DigitalWrite (DataPin, 1);
			wiringPiLib.PinMode (DataPin, PinType.INPUT);
			for (; loopCount < loops; loopCount++) {
				value = wiringPiLib.DigitalRead (DataPin);
				Counter [loopCount] = value;
				if (value != lastValue) {
					transitionCount++;
					lastValue = value;
				}
			}
		}

		static Atmo GetAtmo()
		{	
			int[] Counter = new int[10000];
			DHT22Communication (Counter);

			List<KeyValuePair<int, int>> intervals = GetIntervals (Counter);
			int GrenzWert = GetAverageHighTime (intervals);
			Int64 bytes = GetByts (intervals, GrenzWert);
			Int32 resultData = CheckCheckSum (bytes);
			return CalculateAtmo (resultData);
		}

		static Atmo CalculateAtmo (Int32 resultData)
		{
			Atmo atmo = new Atmo();

			atmo.Temperatur = ((resultData & 0xffff) + 5) / 10;
			atmo.RelativeHumidity = (((resultData >> 16) & 0xffff) + 5) / 10;

			Console.WriteLine (DateTime.Now + " " + atmo.Temperatur + "°Celsius, " + atmo.RelativeHumidity + "% relative Luftfeuchtigkeit.");

			return atmo;
		}

		static Int32 CheckCheckSum (Int64 bytes)
		{
			int checkist = (int)bytes & 0xff;

			bytes = bytes >> 8;
			int result = (int)bytes;

			int checksoll = 0;
			for (int i = 0; i < 4; i++) 
			{
				checksoll += (int)bytes & 0xff;
				bytes = bytes >> 8;
			}

			checksoll = checksoll & 0xff;

			if (checksoll != checkist)
				throw new ArithmeticException ("CheckSummenFehler");

			return result;
		}


		static int GetAverageHighTime (List<KeyValuePair<int, int>> intervals)
		{
			int count = 0;
			int sum = 0;
			//Auf das erste high kann man wohl verzichten!
			foreach (KeyValuePair<int, int> kvp in intervals) 
			{
				if (kvp.Key == 1) 
				{
					count++;
					sum += kvp.Value;
				}
			}
			return sum / count;
		}
			

		static List<KeyValuePair<int, int>> GetIntervals(int[] Counter)
		{
			int last = Counter [0];
			int lastChange = 0;
			List<KeyValuePair<int, int>> intervals = new List<KeyValuePair<int, int>> ();

			for (int i = 0; i < Counter.Length; i++) 
			{
				if (Counter [i] != last) 
				{
					intervals.Add(new KeyValuePair<int, int>(last, i - lastChange));
					lastChange = i; 
					last = Counter [i];
				}
			}
			return intervals;
		}

		static void GenerateHumidity()
		{			
			Console.WriteLine (DateTime.Now + " " + "Dampfing");

			int DampfPin = 5;

			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
			wiringPiLib.WiringPiSetupGpio ();

			wiringPiLib.PinMode (DampfPin, PinType.OUTPUT);

			wiringPiLib.DigitalWrite (DampfPin, 1);
			Thread.Sleep (2000);
			wiringPiLib.DigitalWrite (DampfPin, 0);

			Console.WriteLine (DateTime.Now + " " + "Ausgedampft");
		}
	}
}
