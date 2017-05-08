using System;
using WiringPiLib;

namespace GPIO1
{
	public class KompassDisplay
	{
		ISchrittmotor Schrittmotor;

		public KompassDisplay (ISchrittmotor schrittmotor)
		{
			Schrittmotor = schrittmotor;
		}

		public void Calibrate()
		{
			Schrittmotor.Step (Direction.Right, 1, true, true);
		}

		//Classic: Die Nadel zeigt immer nach Norden, starre Verbindung Sensor Display
		public void ShowClassic(double ankle)
		{
			//Sensor gedreht, Nadel gedreht: die Nadel muss zurückdrehen
			Schrittmotor.SetAnkle (360 - ankle, true);
		}

		//Die Nadel zeigt in die Fahrtrichtung
		public void ShowNavigation(double ankle)
		{
			Schrittmotor.SetAnkle (ankle, true);
		}


	}
}

