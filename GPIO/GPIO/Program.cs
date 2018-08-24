using System;
using System.Threading;
//using Raspberry.IO.GeneralPurpose;
using WiringPiLib;
using System.Configuration;

namespace GPIO1
{
	class MainClass
	{
		static void TasterWiringPi ()
		{
			Console.WriteLine ("Dehre");
			
			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(18, PinType.INPUT) ;

			WiringPiWrapperDirect.digitalWrite (17, 0);

			bool lastButtonPressed = false;
			bool ledBurning = false;

			//while(WirinPiWrapper.millis() < 30000)
			for (var i = 0; i < 100000000; i++) 
			{
				bool buttonPressed = WiringPiWrapperDirect.digitalRead(18) > 0;

				//Draufdrücken
				if(buttonPressed && !lastButtonPressed)
				{
					Console.WriteLine ("Press");
					if (ledBurning)
						WiringPiWrapperDirect.digitalWrite (17, 0);
					else
						WiringPiWrapperDirect.digitalWrite (17, 1);

					ledBurning = !ledBurning;

					lastButtonPressed = true;
				}
				//Loslassen
				if(!buttonPressed && lastButtonPressed)
				{
					Console.WriteLine ("Release");
					lastButtonPressed = false;
					Thread.Sleep (30);	//Prellschalter
				}
			}

			WiringPiWrapperDirect.digitalWrite (17, 0);
			Console.WriteLine ("sers.");
		}

		static int GetOnTime(int period, long atemdauer, long counter)
		{
			float periodenzahl = atemdauer / period;
			double angel = Math.PI * counter / periodenzahl;
			return (int)(Math.Sin (angel) * period);
		}


		public static void WiringPiBenchmarkDelegates ()
		{
			Console.WriteLine ("Servus Luigi.");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(18, PinType.INPUT) ;

			DateTime start = DateTime.Now;
			const int testCount = 100000000;
			for (int i = 0; i < testCount; i++) {
				WiringPiWrapperDirect.digitalWrite (17, i& 1);
			}

			TimeSpan duration = new TimeSpan (DateTime.Now.Ticks - start.Ticks);
			Console.WriteLine ("WiringPi WriteTime  = " + (duration.TotalMilliseconds / testCount) + "ms,  " + testCount / (duration.TotalMilliseconds * 1000)  + " MHz");

			WiringPiWrapperDirect.digitalWrite (17, 0);

//			start = DateTime.Now;
//			int soll;
//			for (int i = 0; i < testCount; i++) {
//				soll = i & 1;
//				WirinPiWrapper.digitalWrite (17, soll);
//				//WirinPiWrapper.delayMicroseconds (1);	//Seltener Fehler, delay drückt von 2 MHz auf 0,2 MHz fehlerlos
//				for (int y = 0; y < 30; y++)			//0,55 MHz fehlerfrei!
//					;
//				if (WirinPiWrapper.digitalRead(18)  != soll)
//					Console.WriteLine ("Scheisndregg " + i);
//			}
//
//			duration = new TimeSpan (DateTime.Now.Ticks - start.Ticks);
//			Console.WriteLine ("WiringPi WriteReadTime  = " + (duration.TotalMilliseconds / testCount) + "ms,  " + testCount / (duration.TotalMilliseconds * 1000)  + " MHz");
		}

		public static void WiringPiBenchmarkWriteReadWrite ()
		{
			Console.WriteLine ("Servus Luigi.");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(19, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(18, PinType.INPUT) ;

			DateTime start = DateTime.Now;

			WiringPiWrapperDirect.digitalWrite (17, 0);

			int testCount = 1000000000;

			start = DateTime.Now;
			int soll;
			int ist;
			for (int i = 0; i < testCount; i++) {
				soll = i & 1;
				WiringPiWrapperDirect.digitalWrite (17, soll);
				//WirinPiWrapper.delayMicroseconds (1);	//Seltener Fehler, delay drückt von 2 MHz auf 0,2 MHz fehlerlos
				//for (int y = 0; y < 30; y++)			//0,55 MHz fehlerfrei!
				//	;
				ist = WiringPiWrapperDirect.digitalRead(18);
				WiringPiWrapperDirect.digitalWrite (19, ist);

				//if (WirinPiWrapper.digitalRead(18)  != soll)
				//	Console.WriteLine ("Scheisndregg " + i);
			}

			TimeSpan duration = new TimeSpan (DateTime.Now.Ticks - start.Ticks);
			Console.WriteLine ("WiringPi WriteReadTime  = " + (duration.TotalMilliseconds / testCount) + "ms,  " + testCount / (duration.TotalMilliseconds * 1000)  + " MHz");
		}


		static void Pwm ()
		{

			Console.WriteLine ("Servus Luigi.");
			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;

			int testCount = 10000000;

			for (int loopCount = 0; loopCount < testCount; loopCount++) 
			{
				WiringPiWrapperDirect.digitalWrite (17, 1);
				WiringPiWrapperDirect.delayMicroseconds (7);
				//Thread.Sleep (7);
				WiringPiWrapperDirect.digitalWrite (17, 0);
				//Thread.Sleep (3);
				WiringPiWrapperDirect.delayMicroseconds (3);
			}
				
			Console.WriteLine ("Scheiss World!");
		}

		static void Buzzer()
		{
			//Ungewöhnliches Beispiel, PNP, wir steuern also mit negativ an, d.h. wir beepen, wenn wir kein Signal haben
			//Hier PNP

			Console.WriteLine ("Servus Luigi.");
			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;

			int testCount = 5
				;

			for (int loopCount = 0; loopCount < testCount; loopCount++) 
			{
				WiringPiWrapperDirect.digitalWrite (17, 1);
				Thread.Sleep (400);
				WiringPiWrapperDirect.digitalWrite (17, 0);
				Thread.Sleep (400);
			}

			Console.WriteLine ("Scheiss World!");
		}

		//http://www.sunfounder.com/index.php?c=case_incs&a=detail_&id=126&name=Super%20Kit%20For%20Raspberry%20Pi

		public static void GleichstromHBrueckeL293D()
		{
			const int EnablePin = 25;
			const int MotorPin1 = 12;
			const int MotorPin2 = 16;

			
			Console.WriteLine ("Setup.");

			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(EnablePin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(MotorPin1, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(MotorPin2, PinType.OUTPUT) ;

			WiringPiWrapperDirect.digitalWrite (MotorPin1, 0);
			WiringPiWrapperDirect.digitalWrite (MotorPin2, 0);

			WiringPiWrapperDirect.digitalWrite (EnablePin, 1);
			Console.WriteLine ("Enabled");

			WiringPiWrapperDirect.digitalWrite (MotorPin1, 1);
			Console.WriteLine ("Pin1 activiert.");
			Thread.Sleep (2000);
			WiringPiWrapperDirect.digitalWrite (MotorPin1, 0);
			Console.WriteLine ("Pin1 deactiviert.");

			Thread.Sleep (1000);

			WiringPiWrapperDirect.digitalWrite (MotorPin2, 1);
			Console.WriteLine ("Pin2 activiert.");
			Thread.Sleep (2000);
			WiringPiWrapperDirect.digitalWrite (MotorPin2, 0);
			Console.WriteLine ("Pin2 deactiviert.");

			Console.WriteLine ("Press 2 disable");
			Console.ReadKey ();
			WiringPiWrapperDirect.digitalWrite (EnablePin, 0);
			Console.WriteLine ("disabled");

		}

		const int ClickPin = 17;			
		const int DataPin = 22;
		const int ClockPin = 5;
		const int ClickPinValue = 4;			//Umwandlung GreyCode -> bin
		const int DataPinValue = 1;
		const int ClockPinValue = 2;
		static int BinaryRotaryValue = 0;
		static AutoResetEvent RotaryChanged = new AutoResetEvent(false);

		public static void RotaryPoller()
		{
			Console.WriteLine ("Polling");

			while (true) 
			{
				int binaryRotaryValue = 0;
				
				if (WiringPiWrapperDirect.digitalRead (DataPin) == 0)
					binaryRotaryValue += DataPinValue;
				if (WiringPiWrapperDirect.digitalRead (ClockPin) == 0)
					binaryRotaryValue += ClockPinValue;
				if (WiringPiWrapperDirect.digitalRead (ClickPin) == 0)	//Pull Up/Down Vertauschung
					binaryRotaryValue += ClickPinValue;

				if (binaryRotaryValue != BinaryRotaryValue) {
					BinaryRotaryValue = binaryRotaryValue;
					RotaryChanged.Set ();
				}
				Thread.Sleep (1);	//Nachdem Prellen ist doof 
			}
			
		}

		public static void RotaryEncoder()
		{
			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(ClickPin, PinType.INPUT) ;
			WiringPiWrapperDirect.pinMode(DataPin, PinType.INPUT) ;
			WiringPiWrapperDirect.pinMode(ClockPin, PinType.INPUT) ;

			WiringPiWrapperDirect.pullUpDnControl (ClickPin, PullUpType.PUD_UP);	//!!
			WiringPiWrapperDirect.pullUpDnControl (DataPin, PullUpType.PUD_DOWN);
			WiringPiWrapperDirect.pullUpDnControl (ClockPin, PullUpType.PUD_DOWN);

			Console.WriteLine ("completed.");

			Thread rotaryPoller = new Thread (RotaryPoller);
			rotaryPoller.Start ();
			bool mouseDown = false;

			Int32 last4Positions = 0;
			do {

				Console.WriteLine (BinaryRotaryValue);

				if(BinaryRotaryValue == 4)
				{
					Console.WriteLine("MouseDown");
					mouseDown = true;
					continue;
				}
				if(mouseDown)
				{
					Console.WriteLine("MouseUp");
					mouseDown = false;
				}

				//Links = 1320
				//Rechts = 2310
				last4Positions = last4Positions << 8;
				last4Positions += BinaryRotaryValue;

				if(last4Positions == 0x01030200)
					Console.WriteLine("LeftRotation");

				if(last4Positions == 0x02030100)
					Console.WriteLine("RightRotation");

			} while(RotaryChanged.WaitOne ());
		}

		public static void IsrCounter()
		{
			m_isrCounter++;
			//Console.WriteLine (m_isrCounter);
		}

		static int m_isrCounter;

		public static void HardwarePwmAndISR()
		{
			const int isrPin = 4;
			const int pwmPin = 18;

			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(isrPin, PinType.INPUT) ;
			WiringPiWrapperDirect.pullUpDnControl (isrPin, PullUpType.PUD_DOWN);
			//WirinPiWrapper.wiringPiISR (isrPin, EdgeType.INT_EDGE_RISING, IsrCounter);

			WiringPiWrapperDirect.pwmSetMode (PwmType.PWM_MODE_MS);
			WiringPiWrapperDirect.pinMode (pwmPin, PinType.PWM_OUTPUT);

			Console.WriteLine ("completed.");

			int loopCount = 1000;
			int clock = 2;					//Teile den max Prozessortakt in Count Teile
			uint range = 2;					//Mach jedes dieser Teile durch 2 Teilbar

			WiringPiWrapperDirect.pwmSetClock (clock);
			WiringPiWrapperDirect.pwmSetRange (range) ;

			DateTime start = DateTime.Now;
			m_isrCounter = 0;

			WiringPiWrapperDirect.pwmWrite (pwmPin, 1);	//Schreibe 1 von 2 Teilen Plus, dann 1/2 0
			while (m_isrCounter < loopCount)
				;

			DateTime end = DateTime.Now;
			float frequenzKhz = (float)(loopCount / (new TimeSpan(end.Ticks - start.Ticks).TotalMilliseconds));
			Console.WriteLine (string.Format("Clock: {0}, Range: {1} Frequenz in KHz: {2}", clock, range, frequenzKhz));
			Console.WriteLine (string.Format("Resultierende PWI Grundfrequez: {0} KHz", clock * frequenzKhz));

			//Hier ist ein Oszi nötig: schreibe ich falsch oder Messe ich falsch?
			//Angeblich ist die interne Clock 19.2MHz
			//Ich komme bei einer Teilung von 64 auf 4,5KHz --> interne Clock 64 * 4,5 = 288KHz?
			//Bei kleinerer Teilung bleibt er stehen (32), danach (ab 16) freeze, der nur mit kaltem reboot zu beenden ist.

			//Vermute, da läuft am ISR ein Stack voll?
			//Stimmt wohl: Ohne ISR schreibt er auch mit clock 2 range 2 data 1: Das sollten dann 10MHZ sein: Oszi??

			//Fazit ISR ist eine feine Sache aber nicht für Hochfrequenz, 3-4 KHz hält das aus, dahinter dann wohl nur primitive Methoden 
			//ledoder volle Threadpools etc


			//Console.WriteLine ("completed.");
		}

		//Das zuletzt geschriebene Bit liegt an Ausgang 0 an
		//Nachdem Q7S nicht benötigt wird, reicht ein StorageClockPin am Ende
		private static void WriteByte(int state)
		{
			for (int i = 1; i < 0x100; i =  i << 1) {
				if ((state & i) > 0)
					WiringPiWrapperDirect.digitalWrite (SerialDataPin, 1);
				else
					WiringPiWrapperDirect.digitalWrite (SerialDataPin, 0);

				Pulse (ShiftClockPin);

				Thread.Sleep (1);
			}
		}

		//Das zuletzt geschriebene Bit liegt an Ausgang 0 an
		//Returnwert ist der hinausgeschobene Teil
		private static int WriteByteReturn(int state)
		{
			int shiftout = 0;
			int soll;

			WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);

			for (int i = 1; i < 0x100; i =  i << 1) {

				if (WiringPiWrapperDirect.digitalRead (ShiftOutPin) > 0)
					shiftout |= i;

				soll = (state & i) > 0 ? 1 : 0;
				WiringPiWrapperDirect.digitalWrite (SerialDataPin, soll);

				Pulse (ShiftClockPin);

				for (int sleep = 0; sleep < 40; sleep++) // Verzögerung von In zu Out
					;
			}

			WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);
			return shiftout;
		}

		static void Pulse(int pin)
		{
			WiringPiWrapperDirect.digitalWrite (pin, 0);

			//Thread.Sleep (100);

			WiringPiWrapperDirect.digitalWrite (pin, 1);
		}

		const int ShiftClockPin = 21; 		//SH_CP
		const int SerialDataPin = 16; 		//SD
		const int StorageClockPin = 20; 	//ST_CP
		const int ShiftOutPin = 12;			//Q7S
		const int NotOutEnabledPin = 26;	// !OE

		private static void	SchieberegisterSingle()	//Shifting Register
		{
			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(SerialDataPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(StorageClockPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(ShiftClockPin, PinType.OUTPUT) ;

			WiringPiWrapperDirect.digitalWrite (SerialDataPin, 0);
			WiringPiWrapperDirect.digitalWrite (ShiftClockPin, 0);

			Console.WriteLine ("completed.");

			for (int i = 0; i < 8; i++) {	

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);
				Thread.Sleep (1);

				WriteByte ( 1 << i);

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);

				Thread.Sleep (1000);
			}
		}

		//Hier wird nicht gechained, sondern nur die Ausgabe gelesen
		//Mit OE wird vermieden, dass die Zwischenzustaände ausgegeben werden und flackern
		private static void	SchieberegisterChained()
		{
			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(SerialDataPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(StorageClockPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(ShiftClockPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(NotOutEnabledPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(ShiftOutPin, PinType.INPUT);
			WiringPiWrapperDirect.pullUpDnControl (ShiftOutPin, PullUpType.PUD_DOWN);

			WiringPiWrapperDirect.digitalWrite (SerialDataPin, 0);
			WiringPiWrapperDirect.digitalWrite (ShiftClockPin, 0);

			Console.WriteLine ("completed.");

			WriteByteReturn (0);
			int lastInput = 0;
			int state; 
			int output;

			int loopCount = 10000;

			DateTime start = DateTime.Now;
			for (int loop = 0; loop < loopCount; loop++) {

				WiringPiWrapperDirect.digitalWrite(NotOutEnabledPin, 1);

				for (int i = 0; i < 8; i++) {	

					state = 1 << i;
					output = WriteByteReturn (state);

					if (output != lastInput)
						Console.WriteLine ("Schätze da hast Du mächtig Scheisse am Hacken.");
					lastInput = state;
				}

				WiringPiWrapperDirect.digitalWrite(NotOutEnabledPin, 0);
			}

			DateTime stop = DateTime.Now;

			Console.WriteLine(loopCount + " Loops in " + new TimeSpan(stop.Ticks - start.Ticks).TotalMilliseconds + " ms.");

		}

		//		aaaaa
		//		f	b
		//		f	b
		//		ggggg
		//		e	c
		//		e	c
		//		ddddd


		// 	Pin 0 = a		128
		// 	Pin 1 = b		64	
		//	Pin 2 = c		32
		//	Pin 3 = d		16
		//	Pin 4 = e		8
		//	Pin 5 = f		4
		//	Pin 6 = g		2
		//	Pin 7 = Point	1

		//Um Eins zu schreiben muss Pin B und C high sein, also 64 und 32 
		//Wenn ich mein Byte vom kleinsten an schreibe: 64 + 32 = 96
		static int[] SegmentCode = new int[] {
			128 + 64 + 32 + 16 + 8 + 4, //0
			64 + 32,					//1
			128 + 64 + 2 + 8 + 16, 		//2
			128 + 64 + 32 + 16 + 2,		//3
			4 + 32 + 2 + 64,			//4
			128 + 4 + 2 + 32 + 16, 		//5
			128 + 4 + 8 + 16 + 32 + 2,	//6
			128 + 64 + 32, 				//7
			254, 						//8
			128 + 4 + 64 + 2 + 32 + 16,	//9
			8 + 4 + 128 + 64 + 32 + 2,	//a
			4 + 8 + 16 + 32 + 2,		//b
			128 + 4 + 8 + 16, 			//c
			64 + 32 + 16 + 8 + 2,		//d
			128 + 4 + 8 + 2 + 16, 		//e
			128 + 4 + 8 + 2				//f
			};


		private static void	SevenSegment()
		{
			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(SerialDataPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(StorageClockPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(ShiftClockPin, PinType.OUTPUT) ;

			WiringPiWrapperDirect.digitalWrite (SerialDataPin, 0);
			WiringPiWrapperDirect.digitalWrite (ShiftClockPin, 0);

			Console.WriteLine ("completed.");

			for (int i = 0; i < 16; i++) 
			{

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);
				//Thread.Sleep (1);

				WriteByte (SegmentCode [i]);

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);

				Thread.Sleep (1000);
			}

			for (int i = 0; i < 16; i++) 
			{

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);

				WriteByte (SegmentCode [i] + 1);	//Der Punkt

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);

				Thread.Sleep (1000);
			}


		}

		static void Clear (int chainCount)
		{
			WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);
			for (int i = 0; i < chainCount; i++)
				WriteByte (0);
			WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);
		}

		private static void	SevenSegmentChained()
		{
			Console.Write ("Setup ");

			WiringPiWrapperDirect.WiringPiSetupGpio ();

			WiringPiWrapperDirect.pinMode(SerialDataPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(StorageClockPin, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(ShiftClockPin, PinType.OUTPUT) ;

			WiringPiWrapperDirect.digitalWrite (SerialDataPin, 0);
			WiringPiWrapperDirect.digitalWrite (ShiftClockPin, 0);

			Console.WriteLine ("completed.");

			const int chainCount = 2;

			Clear (chainCount);
			Thread.Sleep (1000);

			for (int i = 0; i < 16; i++) 
			{

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 0);

				WriteByte (SegmentCode [i] + i%2);

				WiringPiWrapperDirect.digitalWrite (StorageClockPin, 1);

				Thread.Sleep (1000);
			}

			Clear (chainCount);
		}

		const int RegisterSelectPin = 18; 	//H Data, L Command
		const int RegisterSelectData = 1; 
		const int RegisterSelectCommand = 0;
		const int EnablePin = 25;	 		//Das ist die Clock
		//const int BusyPin = 24;				//D7 ist ein Ausgang als D7Pin und ein Eingang als BusyPin!

		const int D4Pin = 21;
		const int D5Pin = 20;
		const int D6Pin = 12;
		const int D7Pin = 26;

		private static void LCD1602_HD44780Logging()
		{
			//Schieberegister ist sinnlos: Ich brauch 3 Pins fürs register und dann immer noch 3 für die LCD und habe dann nicht direkt die 
			//Möglichkeit, PIN 7 auf lazy abzufragen
			//Zeitkritisch ist das nicht, so dass die 4Bit Variante GPIOPinverbrauch / Timing den besseren Wirkungsgrad bringen sollte!

			WiringPiWrapperLogging wiringPiLib = new WiringPiWrapperLogging ();
			wiringPiLib.SetPinName (18, "RegisterSelectPin");
			wiringPiLib.SetPinName (25, "EnablePin");
			wiringPiLib.SetPinName (21, "D4Pin");
			wiringPiLib.SetPinName (20, "D5Pin");
			wiringPiLib.SetPinName (12, "D6Pin");
			wiringPiLib.SetPinName (26, "D7Pin");

			LCD1602_HD44780 lcd = new LCD1602_HD44780 (wiringPiLib);

			lcd.Init ();
			lcd.Display ("Faxe world");
			Thread.Sleep (1000);
			lcd.Display ("End of the\nInternet.");
		}

		private static void LCD1602_HD44780()
		{
			//Schieberegister ist sinnlos: Ich brauch 3 Pins fürs register und dann immer noch 3 für die LCD und habe dann nicht direkt die 
			//Möglichkeit, PIN 7 auf lazy abzufragen
			//Zeitkritisch ist das nicht, so dass die 4Bit Variante GPIOPinverbrauch / Timing den besseren Wirkungsgrad bringen sollte!

			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();

			LCD1602_HD44780 lcd = new LCD1602_HD44780 (wiringPiLib);

			lcd.Init ();
			lcd.Display ("Faxe world");
		}

		private static void Toggle()
		{
			//1.06MHZ
			
			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
			Toggler toggler = new Toggler(wiringPiLib);

			toggler.Toggle ();
		}

		private static void ToggleLogging()
		{
			//CONSOLE + RollingtFile
			//260 HZ!!

			WiringPiWrapperLogging wiringPiLib = new WiringPiWrapperLogging ();

			Toggler toggler = new Toggler(wiringPiLib);

			toggler.Toggle ();
		}

		private static void ToggleDirect()
		{
			//1.26 MHZ, laut benchmarks wiringpi direct = 4MHZ, beste c lib bis 22MHz 
			WiringPiWrapperDirect.WiringPiSetupGpio ();
			WiringPiWrapperDirect.pinMode(18, PinType.OUTPUT) ;

			while (true) 
			{
				WiringPiWrapperDirect.digitalWrite (18, 0);
				WiringPiWrapperDirect.digitalWrite (18, 1);
			}
		}

		private static void AD_MCP4921_SPI()
		{
			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
//			WiringPiWrapperLogging wiringPiLib = new WiringPiWrapperLogging ();
//			wiringPiLib.SetPinName (18, "CS");
//			wiringPiLib.SetPinName (23, "Clock");
//			wiringPiLib.SetPinName (25, "SDI");

			wiringPiLib.WiringPiSetupGpio ();

			DA_MCP4921_SPI ad = new DA_MCP4921_SPI(wiringPiLib, 18,23,25);

			while (true) 
			{
				ad.SetVoltage (0);
				Thread.Sleep (50);
				ad.SetVoltage (50);
				Thread.Sleep (50);
				ad.SetVoltage (100);
				Thread.Sleep (50);
				ad.SetVoltage (50);
				Thread.Sleep (50);
			}
		}

		private static void AD_MCP4921_SPI_Toggler()
		{
			//Verzerrt bisschen, schafft 50 KHZ

			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
			wiringPiLib.WiringPiSetupGpio ();

			DA_MCP4921_SPI ad = new DA_MCP4921_SPI(wiringPiLib, 18, 23, 25);

			while (true) 
			{
				ad.SetVoltage (0);
				ad.SetVoltage (100);
			}
		}

		static void Go (Thread funktionsthread)
		{
			funktionsthread.Start ();
			Thread.Sleep (10000);
			funktionsthread.Abort ();
		}

		private static void FunktionsgeneratorSpi()
		{
			//Hier ist bei 1600Hz Schluß mit Sinus, Bottleneck ist aber nicht SPI sondern DateTime-Auflösung im ms Bereich
			
			FunktionsgeneratorMCP4921 sinusfunktionsgenerator = new GPIO1.FunktionsgeneratorMCP4921 (GPIO1.FunktionsgeneratorMCP4921.Wellenform.Sinus);
			FunktionsgeneratorMCP4921 dreieckfunktionsgenerator = new GPIO1.FunktionsgeneratorMCP4921 (GPIO1.FunktionsgeneratorMCP4921.Wellenform.Dreieck);
			FunktionsgeneratorMCP4921 rechteckfunktionsgenerator = new GPIO1.FunktionsgeneratorMCP4921 (GPIO1.FunktionsgeneratorMCP4921.Wellenform.Rechteck);
			FunktionsgeneratorMCP4921 saegefunktionsgenerator = new GPIO1.FunktionsgeneratorMCP4921 (GPIO1.FunktionsgeneratorMCP4921.Wellenform.Saege);

			FunktionsgeneratorMCP4921[] generators = new FunktionsgeneratorMCP4921[]{sinusfunktionsgenerator, dreieckfunktionsgenerator, rechteckfunktionsgenerator, saegefunktionsgenerator };

			foreach (FunktionsgeneratorMCP4921 funktionsgenerator in generators) {
				for (int i = 0; i < 6; i++) {
					int frequenz = 50 * (int)Math.Pow (2, i);
					Console.WriteLine (frequenz);
					Thread funktionsthread = new Thread (() => funktionsgenerator.Generate (frequenz));
					Go (funktionsthread);
				}
			}
				
			Console.WriteLine ("Out Apple Amen");
		}

		private static void DigitalWriteByte()
		{
			WiringPiWrapperDirect.WiringPiSetupGpio ();	
			//Schreibt auch bei 0xffff nur in diese 8 Pins!

			WiringPiWrapperDirect.pinMode(4, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(17, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(18, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(22, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(23, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(24, PinType.OUTPUT) ;
			WiringPiWrapperDirect.pinMode(25, PinType.OUTPUT) ;

			while (true) 
			{
				WiringPiWrapperDirect.digitalWriteByte(0);

				WiringPiWrapperDirect.digitalWriteByte(1);	//17
				WiringPiWrapperDirect.digitalWriteByte(2);	//18
				WiringPiWrapperDirect.digitalWriteByte(4);	//27
				WiringPiWrapperDirect.digitalWriteByte(8);	//22
				WiringPiWrapperDirect.digitalWriteByte(16);	//23
				WiringPiWrapperDirect.digitalWriteByte(32);	//24
				WiringPiWrapperDirect.digitalWriteByte(64);	//25
				WiringPiWrapperDirect.digitalWriteByte(128);//4

				//WiringPiWrapperDirect.digitalWriteByte(0xff);
			}
		}

		private static void AD_8562_Parallel()
		{
			WirinPiWrapper wiringPiLib = new WirinPiWrapper ();
			wiringPiLib.WiringPiSetupGpio ();

			DA_8562FPZ_Parallel da = new DA_8562FPZ_Parallel (wiringPiLib);

			while (true) 
			{
				da.SetVoltage (0);
				Thread.Sleep (50);
				da.SetVoltage (2047);
				Thread.Sleep (50);
				da.SetVoltage (4095);
				Thread.Sleep (50);
			}
		}


		private static void FunktionsgeneratorParallel()
		{
			Funktionsgenerator8562FPZ_Parallel sinusfunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Sinus);
			Funktionsgenerator8562FPZ_Parallel dreieckfunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Dreieck);
			Funktionsgenerator8562FPZ_Parallel rechteckfunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Rechteck);
			Funktionsgenerator8562FPZ_Parallel saegefunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Saege);

			Funktionsgenerator8562FPZ_Parallel[] generators = new Funktionsgenerator8562FPZ_Parallel[]{sinusfunktionsgenerator, dreieckfunktionsgenerator, rechteckfunktionsgenerator, saegefunktionsgenerator };

			foreach (Funktionsgenerator8562FPZ_Parallel funktionsgenerator in generators) {
				Console.WriteLine (funktionsgenerator.GetType().ToString());
				for (int i = 0; i < 6; i++) {
					int frequenz = 2 * (int)Math.Pow (10, i);
					Console.WriteLine (frequenz);
					Thread funktionsthread = new Thread (() => funktionsgenerator.Generate (frequenz));
					Go (funktionsthread);
				}
			}
			Console.WriteLine ("Out Apple Amen");
		}


		private static void Oszilloskop()
		{
			Funktionsgenerator8562FPZ_Parallel saegefunktionsgenerator = new GPIO1.Funktionsgenerator8562FPZ_Parallel (GPIO1.Funktionsgenerator8562FPZ_Parallel.Wellenform.Saege);
			Thread funktionsthread = new Thread(() => saegefunktionsgenerator.Generate(1));
			funktionsthread.Start();

			OszilloskopAD_MCP3201_SPI osci = new OszilloskopAD_MCP3201_SPI();
			Thread osciThread = new Thread(() => osci.DoVerySmartDisplay());
			osciThread.Start ();

			Thread.Sleep (10000);

			funktionsthread.Abort ();
			osciThread.Abort ();
		}

		private static void Schrittmotor()
		{
			WirinPiWrapper wiringPiLib = new WirinPiWrapper();
			wiringPiLib.WiringPiSetupGpio ();

			Schrittmotor schrittmotor = new Schrittmotor (wiringPiLib, 1,2,3,4);
			schrittmotor.Run (100);
		}

		private static void FolienTastatur()
		{
			FolienTastatur folientastatur = new GPIO1.FolienTastatur ();
			folientastatur.ReadWrite ();
		}

		private static void UART()
		{
			UART uart = new UART();
			string result = uart.WriteRead("bullshit");
			Console.WriteLine (result);
		}

		private static void DA_AD_DA_Reihe()
		{
			DA_AD_DA_Reihe dad = new GPIO1.DA_AD_DA_Reihe();

			dad.DoWork ();
		}

		private static void Kompass()
		{
			int TasterPin = 26;

			WirinPiWrapper wiringPiLib = new WirinPiWrapper();
			wiringPiLib.WiringPiSetupGpio ();

			wiringPiLib.PinMode(TasterPin, PinType.INPUT) ;	//37 liest am Taster, um die Nadel manuell zu bewegen

			ISchrittmotor schrittMotor = new Schrittmotor (wiringPiLib, 19,13,6,5);

			schrittMotor.Run (1);

			KompassHMC6343 kompass = new KompassHMC6343(wiringPiLib);
			KompassDisplay display = new KompassDisplay (schrittMotor);

			while (true) 
			{
				while(wiringPiLib.DigitalRead (TasterPin) == 1)
				{
					Console.WriteLine ("Calibrate");
					display.Calibrate();
				}

				double heading = kompass.ReadHeading ();
				Console.WriteLine ("Heading: " + heading);
				//display.ShowClassic (heading);
				display.ShowNavigation (heading);
			}
		}

		private static void Gordonsleeper()
		{
			DateTime start = DateTime.Now;
			
			for(int i = 0; i < 1000000; i++)
				WiringPiWrapperDirect.delayMicroseconds (1);

			DateTime end = DateTime.Now;

			Console.WriteLine ("Eine Sekunde dauert " + (new TimeSpan(end.Ticks-start.Ticks)).TotalMilliseconds + " Millisekunden");
		}


		private static void SuchHund()
		{
			SuchHund suchHund = new GPIO1.SuchHund();

			//suchHund.ServoTest ();
			//suchHund.SensorTest ();
			suchHund.Suche ();
		}



		public static void Main (string[] args)
		{
			//Blinky ();
			//Taster();
			//TasterWiringPi();
			//TasterEventDriven ();
			//Benchmark();
//			RgbSoftwarePwm();
			//BenchmarkMultithread ();
//			IGpioConnectionDriver connection = new GpioConnectionDriver();
//			Console.WriteLine("GpioConnectionDriver");
//			DABenchmarkMultithread (connection);
//			Console.WriteLine("MemoryGpioConnectionDriver");
//			DABenchmarkMultithread (new MemoryGpioConnectionDriver ());
//			Console.WriteLine("Wiring Pi");
			//WiringPiBenchmark ();
			//WiringPiBenchmarkDelegates ();
			//WiringPiBenchmarkDelegates ();
			//WiringPiBenchmarkWriteReadWrite ();
			//Pwm();
			//Buzzer();
			//GleichstromHBrueckeL293D();
			//RotaryEncoder();
			//HardwarePwmAndISR();
			//SchieberegisterSingle();
			//SchieberegisterChained();
			//SevenSegment();
			//SevenSegmentChained();
//			LCD1602_HD44780Logging();
//			LCD1602_HD44780 ();
			//Toggle ();
			//ToggleLogging();
			//ToggleDirect();
			//AD_MCP4921_SPI();
			//AD_MCP4921_SPI_Toggler();
			//FunktionsgeneratorSpi();
			//Oszilloskop();
			//DA_AD_DA_Reihe();
			//DigitalWriteByte();
			//AD_8562_Parallel();
			//FunktionsgeneratorParallel();
			//Gordonsleeper();
			//Schrittmotor ();
			//FolienTastatur();
			//UART();
			//Kompass ();
			SuchHund();
		}
	}
}
