using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class Schrittmotor
	{
		WirinPiWrapper wiringPiLib = new WirinPiWrapper();

		const int in1 = 2;
		const int in2 = 3;
		const int in3 = 4;
		const int in4 = 17;

		public Schrittmotor ()
		{
			wiringPiLib.WiringPiSetupGpio();

			wiringPiLib.PinMode(in1, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in2, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in3, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in4, PinType.OUTPUT) ;

			Ausschalten ();
		}

		private void Ausschalten()
		{
			wiringPiLib.DigitalWrite (in1, 0);	
			wiringPiLib.DigitalWrite (in2, 0);	
			wiringPiLib.DigitalWrite (in3, 0);	
			wiringPiLib.DigitalWrite (in4, 0);	
		}


		class Motorstellung
		{
			public int i1;
			public int i2;
			public int i3;
			public int i4;
		}

		Motorstellung[] Ablauf = {
			new Motorstellung()	{i1 = 1, i2 = 0, i3 = 0, i4 = 0}, 
			new Motorstellung()	{i1 = 1, i2 = 1, i3 = 0, i4 = 0}, 
			new Motorstellung()	{i1 = 0, i2 = 1, i3 = 0, i4 = 0}, 
			new Motorstellung()	{i1 = 0, i2 = 1, i3 = 1, i4 = 0}, 
			new Motorstellung()	{i1 = 0, i2 = 0, i3 = 1, i4 = 0}, 
			new Motorstellung()	{i1 = 0, i2 = 0, i3 = 1, i4 = 1}, 
			new Motorstellung()	{i1 = 0, i2 = 0, i3 = 0, i4 = 1}, 
			new Motorstellung()	{i1 = 1, i2 = 0, i3 = 0, i4 = 1}, 
		};

		public void Go()
		{
			int anzStellungen = Ablauf.Length;

			DateTime start = DateTime.Now;
			int stepsPerRound = 4096;
			int rundenZahl = 100;

			for(int i = 0; i < stepsPerRound * rundenZahl; i++)
			{
				SetMotorstellung(Ablauf[i % anzStellungen]);
				wiringPiLib.DelayMicroseconds (1000);
			}

			DateTime stop = DateTime.Now;
			TimeSpan dauer = new TimeSpan (stop.Ticks - start.Ticks);

			Console.WriteLine ("Dauer: " + dauer + " für " + rundenZahl + " Runden: " + new TimeSpan(dauer.Ticks / rundenZahl) + " secs pro runde"); 

			Ausschalten ();
		}

		private void SetMotorstellung(Motorstellung motorstellung)
		{
			wiringPiLib.DigitalWrite (in1, motorstellung.i1);	
			wiringPiLib.DigitalWrite (in2, motorstellung.i2);	
			wiringPiLib.DigitalWrite (in3, motorstellung.i3);	
			wiringPiLib.DigitalWrite (in4, motorstellung.i4);	
		}
	}
}

