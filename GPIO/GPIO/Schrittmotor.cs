using System;
using WiringPiLib;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GPIO1
{
	public enum Direction
	{
		Left,
		Right
	}

	public interface ISchrittmotor
	{
		void Step (Direction direction, int count, bool reset, bool saveMotorstate);
		void SetAnkle (double ankle, bool saveMotorstate);
		void Run (int rounds);
	}

	public class Schrittmotor : ISchrittmotor
	{
		class Motorstellung
		{
			public int i1;
			public int i2;
			public int i3;
			public int i4;
		}

		[Serializable] 
		class MotorState
		{
			public int Motorstellung;			//wo liegt an meinem Motor Strom, Index in Ablauf
			public int Position;				//in Welcher Position der Umdrehung stehe ich gerade 0 <= Position < StepsPerRound
			public int Offset;
		}

		WirinPiWrapper WiringPiLib;

		//GPIO Ports
		int In1{ get; set; }
		int In2{ get; set; }
		int In3{ get; set; }
		int In4{ get; set; }

		int StepsPerRound { get {return 4096;} }
		MotorState actualMotorState;

		void LoadMotorstateOnRestart ()
		{
			if (actualMotorState == null) 
			{
				try
				{
					Stream stream = new FileStream("Motorstate.bin", FileMode.Open);
					BinaryFormatter formatter = new BinaryFormatter();
					actualMotorState = (MotorState) formatter.Deserialize(stream);
					stream.Close();
				}
				catch(Exception e)
				{
					Console.WriteLine ("Kein Motorstate ladbar, das ist beim ersten Mal ok.");
					Console.WriteLine (e.Message);
					actualMotorState = new MotorState () { Motorstellung = 0, Position = 0, Offset = 0};
				}
			}
		}

		void SafeMotorState()
		{
			IFormatter formatter = new BinaryFormatter();  
			Stream stream = new FileStream("Motorstate.bin", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);  
			formatter.Serialize(stream, actualMotorState);  
			stream.Close();  
		}

		void CalculateNewMotorstellung (Direction direction)
		{
			if (direction == Direction.Left) 
			{
				actualMotorState.Position = actualMotorState.Position > 0 ? actualMotorState.Position - 1 : actualMotorState.Position = (StepsPerRound - 1);
				actualMotorState.Motorstellung = actualMotorState.Motorstellung > 0 ? actualMotorState.Motorstellung - 1 : actualMotorState.Motorstellung = Ablauf.Length - 1;
			}
			else 
			{
				actualMotorState.Position = (actualMotorState.Position + 1) % StepsPerRound;
				actualMotorState.Motorstellung = (actualMotorState.Motorstellung + 1) % Ablauf.Length;
			}
		}

		//Mit Reset kann die Position als 0 definiert werden
		public void Step(Direction direction, int count, bool calibrate, bool saveMotorstate)
		{
			for (int loop = 0; loop < count; loop++) 
			{
				CalculateNewMotorstellung (direction);
				SetMotorstellung (Ablauf [actualMotorState.Motorstellung]);
				Thread.Sleep (2);	//Das braucht der Schrittmotor wenn man Ausschaltet und das FileSystem, wenn man speichert!
				Ausschalten ();
				//WiringPiLib.DelayMicroseconds (1000);

				if (calibrate)
					actualMotorState.Offset++;

				if (saveMotorstate)
					SafeMotorState();
			}
		}

		public void SetAnkle(double ankle, bool saveMotorstate)
		{
			int sollPosition = (int)(((ankle / 360) * StepsPerRound) + actualMotorState.Offset) % StepsPerRound;

			if (Math.Abs(actualMotorState.Position - sollPosition) < (StepsPerRound / 720 ))
				return;

			int stepsRight = sollPosition - actualMotorState.Position;
			if (stepsRight < 0)
				stepsRight += StepsPerRound;
			int stepsLeft = actualMotorState.Position - sollPosition;
			if (stepsLeft < 0)
				stepsLeft += StepsPerRound;
			const int maxSteps = 4000;

			if (stepsLeft < stepsRight)
			{
				for (int step = 0; step < stepsLeft % maxSteps; step++)	//modulo: max x steps, ddamit dazwischen wieder gemessen werden kann
					Step(Direction.Left, 1, false, saveMotorstate);
			} 
			else
			{
				for (int step = 0; step < stepsRight % maxSteps; step++)
					Step(Direction.Right, 1, false, saveMotorstate);
			} 
		}

		public Schrittmotor (WirinPiWrapper wiringPiLib, int in1, int in2, int in3, int in4)
		{
			WiringPiLib = wiringPiLib;
			In1 = in1;
			In2 = in2;
			In3 = in3;
			In4 = in4;

			wiringPiLib.PinMode(in1, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in2, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in3, PinType.OUTPUT) ;
			wiringPiLib.PinMode(in4, PinType.OUTPUT) ;

			Ausschalten ();

			LoadMotorstateOnRestart ();
		}

		private void Ausschalten()
		{
			WiringPiLib.DigitalWrite (In1, 0);	
			WiringPiLib.DigitalWrite (In2, 0);	
			WiringPiLib.DigitalWrite (In3, 0);	
			WiringPiLib.DigitalWrite (In4, 0);	
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

		public void Run(int rounds)
		{
			int anzStellungen = Ablauf.Length;

			DateTime start = DateTime.Now;

			for(int i = 0; i < StepsPerRound * rounds; i++)
			{
				SetMotorstellung(Ablauf[i % anzStellungen]);
				WiringPiLib.DelayMicroseconds (1000);
			}

			DateTime stop = DateTime.Now;
			TimeSpan dauer = new TimeSpan (stop.Ticks - start.Ticks);

			Console.WriteLine ("Dauer: " + dauer + " für " + rounds + " Runden: " + new TimeSpan(dauer.Ticks / rounds) + " secs pro runde"); 

			Ausschalten ();
		}

		private void SetMotorstellung(Motorstellung motorstellung)
		{
			WiringPiLib.DigitalWrite (In1, motorstellung.i1);	
			WiringPiLib.DigitalWrite (In2, motorstellung.i2);	
			WiringPiLib.DigitalWrite (In3, motorstellung.i3);	
			WiringPiLib.DigitalWrite (In4, motorstellung.i4);	
		}
	}
}

