using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class KompassHMC6343
	{
		//thx https://github.com/pophaax/Compass

		const int I2CdeviceID = 0x19;

		const int ORIENTATION_LEVEL = 0x72;
		const int ORIENTATION_SIDEWAYS = 0x73;
		const int ORIENTATION_FLATFRONT = 0x74;

		//Commands
		public const int POST_HEADING = 0x50;
		public const int POST_TILT = 0x55;
		public const int POST_MAG = 0x45;
		public const int POST_ACCEL = 0x40;
		const int READ_EEPROM = 0xE1;

		int fd;

		WirinPiWrapper WiringPiLib;

		public KompassHMC6343 (WirinPiWrapper wiringPiLib)
		{
			WiringPiLib = wiringPiLib;
			//Wait at least 500 milli-seconds for device initialization. The HMC6343 is in the default Run Mode
			Thread.Sleep (501);

			fd = wiringPiLib.WiringPiI2CSetup (I2CdeviceID);

			if (fd < 0)
				Console.WriteLine ("FUCKED UP wiringPiI2CSetup");

			Thread.Sleep (5);

			//Set Orientation
			wiringPiLib.WiringPiI2CWrite (I2CdeviceID, ORIENTATION_LEVEL);
		}

		public double ReadHeading()
		{
			WiringPiLib.WiringPiI2CWrite (fd, POST_HEADING);
			Thread.Sleep (2);
			byte[] response = new byte[6];
			for (int byteCount = 0; byteCount < 6; byteCount++)
				response [byteCount] = (byte)WiringPiLib.WiringPiI2CRead (fd);
			short head = (short)((response [0] << 8) | response [1]);

			//short pitch = (short)((response [2] << 8) | response [3]);
			//short roll = (short)((response [4] << 8) | response [5]);

			return ((double)head) / (10);	//Head in 10tel Grad
			//Console.WriteLine("Head:  " + head);
			//Console.WriteLine("Pitch: " + pitch);
			//Console.WriteLine("Roll:  " + roll);
		}

		//Erkenntnis: der arctan liefert das selbe Ergebnis wie Heading
		public double ReadMag()
		{
			WiringPiLib.WiringPiI2CWrite (fd, POST_MAG);
			Thread.Sleep (2);

			byte[] response = new byte[6];
			for (int i = 0; i < 6; i++)
				response [i] = (byte)WiringPiLib.WiringPiI2CRead (fd);

			//Post Mag Data. MxMSB, MxLSB, MyMSB, MyLSB, MzMSB, MzLSB
			short magX = (short)((response [0] << 8) | response [1]);
	        short magY = (short)((response [2] << 8) | response [3]);
	        short magZ = (short)((response [4] << 8) | response [5]);

			//Console.WriteLine("MagX:  " + magX);
			//Console.WriteLine("MagY: " + magY);
			//Console.WriteLine("MagZ:  " + magZ);

			double auslenkung = 180 + (Math.Atan2 (magY, magX) * 180 / Math.PI);
			Console.WriteLine ("ArcTan: " + auslenkung);
			return auslenkung;
		}
	}
}

