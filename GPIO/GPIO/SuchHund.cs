using System;
using WiringPiLib;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using log4net;

namespace GPIO1
{
	public class SuchHund
	{
		//Pinbelegung
		const int PWMServoPin	= 18;	//parallel read won't work: pwm uses port 18 also!
		readonly int[] Sensors = new int[] {23, 24, 25, 8, 7, 12, 16, 20 };
		const int SHUTDOWNPIN	=	21;  // 1 = on, 0 = Shutdown
		const int ACTIVPIN	=	17;		// 0 = lazy 1 = Active
		bool InvertSensorInput{ get; set; }	//true für weiße Linie auf schwarzem Boden
		static int SleepBetweenActions;
		float LenkDeltaInGrad{ get; set;}
		Servo LenkServo { get; set;}
		int NonPlausibleWaitTimerMilliSec{ get; set;}
		DateTime LastPlausibleSensorResult = DateTime.Now;
		bool TooLongUnplausibleSensorResult = false;
		static bool IsActive = false;

		static ILog Log4Net = LogManager.GetLogger("SuchHund");

		WirinPiWrapper WiringPiLib;

		void Initializer()
		{
			WiringPiLib = new WirinPiWrapper();
			WiringPiLib.WiringPiSetupGpio ();

			WiringPiLib.PinMode(SHUTDOWNPIN, PinType.INPUT) ;
			WiringPiLib.PullUpDnControl (SHUTDOWNPIN, PullUpType.PUD_UP);

			WiringPiLib.PinMode(ACTIVPIN, PinType.INPUT) ;
			WiringPiLib.PullUpDnControl (ACTIVPIN, PullUpType.PUD_DOWN);

			for (int i = 0; i < Sensors.Length; i++) {
				WiringPiLib.PinMode (Sensors [i], PinType.INPUT);
				WiringPiLib.PullUpDnControl(Sensors[i], PullUpType.PUD_DOWN);
			}

			SleepBetweenActions = Int32.Parse(ConfigurationManager.AppSettings ["Hund_SleepBetweenActions"]);

			int pwmRange = Int32.Parse(ConfigurationManager.AppSettings ["Hund_PWM_Range"]);
			int servohertz = Int32.Parse(ConfigurationManager.AppSettings ["HundServohertz"]);
			int servoMaximalAusschlagGrad = Int32.Parse(ConfigurationManager.AppSettings ["HundServoMaximalAusschlagGrad"]);
			int hundServoMilliSecLeft = Int32.Parse(ConfigurationManager.AppSettings ["HundServoMicroSecLeft"]);
			int hundServoMilliSecRight = Int32.Parse(ConfigurationManager.AppSettings ["HundServoMicroSecRight"]);
			InvertSensorInput = bool.Parse(ConfigurationManager.AppSettings ["HundInvertSensorInput"]);
			NonPlausibleWaitTimerMilliSec = Int32.Parse(ConfigurationManager.AppSettings ["HundNonPlausibleWaitTimerMilliSec"]);
			LenkDeltaInGrad = float.Parse(ConfigurationManager.AppSettings ["HundLenkDeltainGrad"]);

			LenkServo = new Servo (PWMServoPin, servohertz, pwmRange, servoMaximalAusschlagGrad, WiringPiLib, hundServoMilliSecLeft, hundServoMilliSecRight);
		}

		public SuchHund ()
		{
			Initializer ();
		}


		//Definition 1 ist Linie, 0 ist Boden
		private long ReadSensors ()
		{
			long result = 0;
			
			for (int i = 0; i < Sensors.Length; i++)
				if (WiringPiLib.DigitalRead(Sensors[i]) > 0)
					result = result | ((long)1 << i);

			if (InvertSensorInput)
				result = result ^ 0xffffffff;

			WriteResult (result);

			return result;
		}

		private bool SensorsPlausible(long sensorsResults)
		{ 
			bool result = true;

			//wer nix liest ist sinnlos
			if ((sensorsResults == 0) || (sensorsResults == 0xff))
				result = false;
			else {
				//Sinnvoll ist es, wenn 
				// 00001111 gerade, also Liniengrenze, Sollzustand
				// 00000000 oder 11111111 : auf der Linie / ganz daneben
				// 00011000  dünne linie ???
				//Bloedsinn ist 
				//11000011 Linie Boden Linie??
				//01010101 Muster lesefehler
				// Also: wenn ich von Links anfange, ist es sinnvoll, wenn hinter der ersten Linie 0..n Linien kommen und dann nur Boden
				int i = 0;
				while (((sensorsResults & ((long)1 << i)) == 0) && (i < Sensors.Length))
					i++;
				
				while (((sensorsResults & ((long)1 << i)) > 0) && (i < Sensors.Length))
					i++;

				while (((sensorsResults & ((long)1 << i)) == 0) && (i < Sensors.Length))
					i++;

				if (i < Sensors.Length)
					result = false;
			}

			if (result) 
			{
				LastPlausibleSensorResult = DateTime.Now;
				TooLongUnplausibleSensorResult = false;
				Log4Net.Info ("Plausibel");
			}
			else 
			{
				if((new TimeSpan(DateTime.Now.Ticks - LastPlausibleSensorResult.Ticks)).TotalMilliseconds > NonPlausibleWaitTimerMilliSec)
					TooLongUnplausibleSensorResult = true;
				
				Log4Net.Info ("NICHT plausibel");
			}

			return result;
		}


		enum Direction
		{
			GoLeft = -1,
			OnTrack = 0,
			GoRight = 1
		}

		private Direction GetSollDirection(long sensors)
		{
			int i = 0;
			while (((sensors & ((long)1 << i)) == 0) && (i < Sensors.Length))
				i++;

			while (((sensors & ((long)1 << i)) == 1) && (i < Sensors.Length))
				i++;

			//i steht jetzt am Linken Rand der Linie
			if (i < Sensors.Length / 2)
				return Direction.GoLeft;

			if (i == Sensors.Length / 2)
				return Direction.OnTrack;

			return Direction.GoRight;
		}

		private void Lenke(Direction direction)
		{
			Log4Net.Info ($"Lenke: {direction}, Alte Position: {LenkServo.Position}");

			if (direction == Direction.OnTrack) 
			{
				//Auf der perfekten Linie nix machen
				LenkServo.SetPosition (0);
			}
			
			if (direction == Direction.GoLeft) 
			{
				if(LenkServo.Position > 0)	//Wir sollen nach links, fahren aber noch rechts: Trägheit
					LenkServo.SetPosition (0);
				else if(LenkServo.Position > (LenkDeltaInGrad -90))
					LenkServo.SetPosition (LenkServo.Position - LenkDeltaInGrad);
			}

			if (direction == Direction.GoRight) 
			{
				if(LenkServo.Position < 0)
					LenkServo.SetPosition (0);		//Trägheit
				else if(LenkServo.Position < (90 + LenkDeltaInGrad))
					LenkServo.SetPosition (LenkServo.Position + LenkDeltaInGrad);
			}
		}

		public void SensorTest()
		{
			while (true) 
			{
				long sensorResult = ReadSensors ();

				if(SensorsPlausible (sensorResult))
					Log4Net.Info (GetSollDirection (sensorResult));

				if(TooLongUnplausibleSensorResult)
					Log4Net.Info ("Zu lange unplausibel");

				Thread.Sleep (500);
			}
		}

		private void WriteResult(long result)
		{
			string output = "Result: ";

			for (int i = 0; i < Sensors.Length; i++)
				if ((result & (1 << i)) > 0)
					output += "1";
				else
					output += "0";

			Log4Net.Info (output);
		}


		public void ServoTest()
		{

//				while (true) 
//				{
//					LenkServo.SetPosition (0);
//					LenkServo.SetPosition (5);
//					LenkServo.SetPosition (10);
//					LenkServo.SetPosition (15);
//					LenkServo.SetPosition (20);
//					LenkServo.SetPosition (25);
//					LenkServo.SetPosition (30);
//					LenkServo.SetPosition (35);
//					LenkServo.SetPosition (40);
//					LenkServo.SetPosition (45);
//					LenkServo.SetPosition (50);
//					LenkServo.SetPosition (55);
//					LenkServo.SetPosition (60);
//					LenkServo.SetPosition (65);
//					LenkServo.SetPosition (70);
//					LenkServo.SetPosition (75);
//					LenkServo.SetPosition (80);
//					LenkServo.SetPosition (85);
//					LenkServo.SetPosition (90);
//				}

 			LenkServo.SetPosition (-10);
			LenkServo.SetPosition (-30);

			while (true) 
			{
				for(int i = -45; i < 45; i++)
				{
					LenkServo.SetPosition(i);
					Thread.Sleep (40);
				}
				for(int i = 45; i > -45; i--)
				{
					LenkServo.SetPosition(i);
					Thread.Sleep (40);
				}
			}
		}

		void ActivCheck(bool sollState)
		{
			if (IsActive == sollState)
				return;

			Thread.Sleep (300);	//Entprellen

			IsActive = sollState;

			if (!IsActive)
				LenkServo.SetPosition (0);
		}

		public void Suche()
		{
			while (true) 
			{
				ActivCheck (WiringPiLib.DigitalRead (ACTIVPIN) == 1);
				if (!IsActive) 
				{
					Log4Net.Info ($"INACTIVE, Alte Position: {LenkServo.Position}");
					Thread.Sleep (300);
					continue;
				}

				if (WiringPiLib.DigitalRead (SHUTDOWNPIN) == 0)
					Process.Start ("/usr/bin/sudo", "/sbin/shutdown -h now");

				long sensorResult = ReadSensors ();

				if (SensorsPlausible (sensorResult)) 
				{
					Lenke (GetSollDirection (sensorResult));
				} 
				else if (TooLongUnplausibleSensorResult) 
				{
					Log4Net.Info ("Zu lange unplausibel");
					LenkServo.SetPosition(0);
				}
				Thread.Sleep (SleepBetweenActions);
			}
		}
	}
}