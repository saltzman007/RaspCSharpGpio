using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	/// <summary>
	/// Getestet mit dem LoggingWrapper, der direkte Zugriff ist 5 mal schneller, hier sind evtl noch ein paar waits notwendig!
	/// </summary>
	public class LCD1602_HD44780
	{
		IWiringPiWrapper WiringPiWrapper;
		const int RegisterSelectPin = 18; 	//H Data, L Command
		const int RegisterSelectData = 1; 
		const int RegisterSelectCommand = 0;
		const int EnablePin = 25;	 		//Das ist die Clock

		const int DeleayInMs = 100;

		const int D4Pin = 21;
		const int D5Pin = 20;
		const int D6Pin = 12;
		const int D7Pin = 26;

		public LCD1602_HD44780 (IWiringPiWrapper iWiringPiWarpper)
		{
			WiringPiWrapper = iWiringPiWarpper;
		}

		private void WriteHalfByte (int command)
		{
			WiringPiWrapper.DigitalWrite (D4Pin, command & 1);		//TODO war da was mit parallel schreiben in der WiringPiLib??
			WiringPiWrapper.DigitalWrite (D5Pin, command>>1 & 1);
			WiringPiWrapper.DigitalWrite (D6Pin, command>>2 & 1);
			WiringPiWrapper.DigitalWrite (D7Pin, command>>3 & 1);
			WiringPiWrapper.DigitalWrite (EnablePin, 1);
			Thread.Sleep (DeleayInMs);
			WiringPiWrapper.DigitalWrite (EnablePin, 0);
		}

		private void LCD_command(int command)
		{
			WiringPiWrapper.DigitalWrite (RegisterSelectPin, RegisterSelectCommand);

			WriteHalfByte (command >> 4);
			WriteHalfByte (command);
		}

		private void LCD_senddata(int data)
		{
			WiringPiWrapper.DigitalWrite (RegisterSelectPin, RegisterSelectData);

			WriteHalfByte (data >> 4);
			WriteHalfByte (data);
		}

		private void LCD_sendstring(string data)
		{
			LCD_command(0x01); 	//Clear, cursor = 0;

			foreach (char c in data.ToCharArray())
				if(c == '\n')
					LCD_command(0xC0); 	//New Line
				else
					LCD_senddata (c);
		}

		public void Init()
		{
			WiringPiWrapper.WiringPiSetupGpio ();

			WiringPiWrapper.PinMode(RegisterSelectPin, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(EnablePin, PinType.OUTPUT) ;

			WiringPiWrapper.PinMode(D4Pin, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(D5Pin, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(D6Pin, PinType.OUTPUT) ;
			WiringPiWrapper.PinMode(D7Pin, PinType.OUTPUT) ;

			WiringPiWrapper.DigitalWrite (EnablePin, 0);

			Thread.Sleep (DeleayInMs);	//Wait for more than 40ms after Vcc rises to 2.7V

			LCD_command(0x33);
			LCD_command(0x32);


			LCD_command(0x28);	//function set: 4 bit, 2 Zeilen, 5x8	//ACHTUNG DOTS immer zählen!!
			LCD_command(0x0C);	//Display Control: display on, cursor on, no blinking
			LCD_command(0x06);	//Entry Mode set: autoincrement, no display shift
		}

		public void Display(string text)
		{
			LCD_sendstring (text);
		}
	}
}

