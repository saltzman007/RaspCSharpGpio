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
		const int SHUTDOWN	=	21;
		bool InvertSensorInput{ get; set; }	//true für weiße Linie auf schwarzem Boden
		static int SleepBetweenActions;
		Servo LenkServo { get; set;}

		static ILog Log4Net = LogManager.GetLogger("SuchHund");

		WirinPiWrapper WiringPiLib;

		void Initializer()
		{
			WiringPiLib = new WirinPiWrapper();
			WiringPiLib.WiringPiSetupGpio ();

			WiringPiLib.PinMode(SHUTDOWN, PinType.INPUT) ;
			WiringPiLib.PullUpDnControl (SHUTDOWN, PullUpType.PUD_UP);

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
				
			while (((sensorsResults & ((long)1 << i)) > 0)&& (i < Sensors.Length))
				i++;

			while (((sensorsResults & ((long)1 << i)) == 0)&& (i < Sensors.Length))
				i++;

			if (i >= Sensors.Length) 
			{
				Log4Net.Info ("Plausibel");
				return true;
			}

			Log4Net.Info ("NICHT plausibel");
			return false;
		}


		enum Direction
		{
			Left = -1,
			Straight = 0,
			Right = 1
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
				return Direction.Left;

			if (i == Sensors.Length / 2)
				return Direction.Straight;

			return Direction.Right;
		}

		private void Lenke(Direction direction)
		{
			Log4Net.Info ($"Lenke: {direction}, Alte Position: {LenkServo.Position}");

			if (direction == Direction.Straight) 
			{
				LenkServo.SetPosition (0);
			}
			
			if (direction == Direction.Left) 
			{
				if(LenkServo.Position > -90)
					LenkServo.SetPosition (LenkServo.Position - 1);
			}

			if (direction == Direction.Right) 
			{
				if(LenkServo.Position < 90)
					LenkServo.SetPosition (LenkServo.Position + 1);
			}
		}

		public void SensorTest()
		{
			while (true) 
			{
				long sensorResult = ReadSensors ();

				if(SensorsPlausible (sensorResult))
					Log4Net.Info (GetSollDirection (sensorResult));

				Thread.Sleep (1000);
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
			while (true) 
			{
				for(int i = -90; i < 90; i++)
				{
					LenkServo.SetPosition(i);
					Thread.Sleep (5);
				}
				for(int i = 90; i > -90; i--)
				{
					LenkServo.SetPosition(i);
					Thread.Sleep (5);
				}
			}
		}

		public void Suche()
		{
			while (true) 
			{
				if (WiringPiLib.DigitalRead (SHUTDOWN) == 0)
					Process.Start ("/usr/bin/sudo", "/sbin/shutdown -h now");

				long sensorResult = ReadSensors ();

				if (SensorsPlausible (sensorResult)) 
				{
					Lenke (GetSollDirection (sensorResult));
					Thread.Sleep (SleepBetweenActions);
				}
			}
		}
	}
}