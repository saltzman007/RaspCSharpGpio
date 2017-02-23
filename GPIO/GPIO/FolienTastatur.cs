using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class FolienTastatur
	{
		WirinPiWrapper wiringPiLib = new WirinPiWrapper();

		int[] Rows = new int[] {15, 14, 18, 23};
		int[] Cols = new int[] {24, 8, 25, 7};

		char[][] Chars = {
			new char[4] {'1', '2', '3', 'A'}, 
			new char[4] {'4', '5', '6', 'B'}, 
			new char[4] {'7', '8', '9', 'C'}, 
			new char[4] {'*', '0', '#', 'D'}
		};

		public FolienTastatur ()
		{
			wiringPiLib.WiringPiSetupGpio();

			for (int i = 0; i < Rows.Length; i++) {
				wiringPiLib.PinMode (Rows [i], PinType.OUTPUT);
				wiringPiLib.DigitalWrite (Rows [i], 0);
				wiringPiLib.PinMode(Cols [i], PinType.INPUT) ;
				wiringPiLib.PullUpDnControl (Cols[i], PullUpType.PUD_DOWN);
			}
		}

		public void ReadWrite()
		{
			while (true) {
				for (int rowIndex = 0; rowIndex < Rows.Length; rowIndex++) {

					wiringPiLib.DigitalWrite (Rows [rowIndex], 1);

					for (int colIndex = 0; colIndex < Cols.Length; colIndex++) {
						if (wiringPiLib.DigitalRead (Cols [colIndex]) == 1) {
							//Console.WriteLine ("Reihe " + rowIndex + "Spalte " + colIndex + " gerueckt.");
							Console.WriteLine (Chars [rowIndex][colIndex]);
							while (wiringPiLib.DigitalRead (Cols [colIndex]) == 1) 
								Thread.Sleep (10);
						}
					}
					wiringPiLib.DigitalWrite (Rows [rowIndex], 0);
				}
			}

		}

	}
}

