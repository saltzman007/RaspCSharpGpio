using System;
using System.Threading;
using WiringPiLib;
using System.Collections.Generic;


/// <summary>
/// D a A d D a reihe.
/// Der Funktionsgenerator erzeugt ein analoges Signal, das der AD Wandler digitalisert und wieder an einen DA Wandler gibt.
/// Im Oszilloskop lassen sich die zwei Analogsignale vergleichen.
/// 
/// Die SPI Bausteine mit dieser Frequenz sind zu langsam für sinnvolle Ergebnisse, immerhin kann die Form erahnt werden:-)
/// </summary>
namespace GPIO1
{
	public class DA_AD_DA_Reihe
	{
		WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
		AD_MCP3201_SPI ad;
		DA_MCP4921_SPI da;

		object sync = new object ();
		Queue<int> Messwerte = new Queue<int>();

		public DA_AD_DA_Reihe ()
		{
			wiringPiLib.WiringPiSetupGpio();

			ad = new AD_MCP3201_SPI(wiringPiLib, 26, 13, 19);		//cs clock data
			da = new DA_MCP4921_SPI(wiringPiLib, 22, 13, 27);		//cs clock data //Erkenntnis : Data kann bei spi nur auf einem Draht sein, wenn alle lesen oder alle schreiben!
		}

		public void DoWork()
		{
			Funktionsgenerator8562FPZ_Parallel saegefunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Saege);
			Thread funktionsthread = new Thread(() => saegefunktionsgenerator.Generate(5));
			funktionsthread.Start();

			Thread adcThread = new Thread(() => ADC());
			adcThread.Start();

			Thread dacThread = new Thread(() => DAC());
			dacThread.Start();

			Console.WriteLine ("Pressen sie die Anykey-Taste!");
			Console.ReadKey ();

			funktionsthread.Abort ();
			adcThread.Abort ();
			dacThread.Abort ();
		}

		void ADC()
		{
			while (true) 
			{
				int val = ad.ReadVoltage ();
				lock (sync) {
					Messwerte.Enqueue (val);
				}
			}
		}

		void DAC()
		{
			while (true) 
			{
				int val = -1;
				lock (sync) {
					if(Messwerte.Count > 0)
						val = Messwerte.Dequeue ();
				}

				if(val > -1)
					da.SetVoltage (val);
			}
		}

	}
}

