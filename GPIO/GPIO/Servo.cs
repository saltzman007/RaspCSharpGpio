using System;
using WiringPiLib;

namespace GPIO1
{
	public class Servo
	{
		public float Position { get; private set;}
		int Hertz{ get; set;}
		WirinPiWrapper WiringPiLib{ get; set;}
		uint PwmRange{ get; set;}
		int PwmPin{ get; set;}
		int MaximalAusschlagGrad{ get; set;}
		int MicroSecLeft{ get; set;}
		float MicroSecRange{ get; set;}
		int PeriodeMicroSecs{ get; set;}

		public Servo (int pwmPin, int hertz, int pwmRange, int maximalAusschlagGrad,  WirinPiWrapper wiringPiLib, int microSecLeft, int microSecRight)
		{
			WiringPiLib = wiringPiLib;

			PwmRange = (uint)pwmRange;
			//pwmFrequency in Hz = 19.2e6 Hz / pwmClock / pwmRange.
			int PwmClock = 19200000 / pwmRange / hertz;

			WiringPiLib.PinMode(pwmPin, PinType.PWM_OUTPUT) ;

			WiringPiLib.PwmSetClock (PwmClock);
			WiringPiLib.PwmSetMode (PwmType.PWM_MODE_MS);	//Servos likes mark / Space approach
			WiringPiLib.PwmSetRange(PwmRange);

			PwmPin = pwmPin;

			MaximalAusschlagGrad = maximalAusschlagGrad;

			MicroSecLeft = microSecLeft;

			MicroSecRange = microSecRight - MicroSecLeft;

			PeriodeMicroSecs = 1000000 / hertz;
			Position = 1;
			SetPosition (0);
		}

		public void SetPosition(float sollPosition)	//-90 bis + 90
		{
			if (sollPosition == Position)
				return;

			if (Math.Abs (sollPosition) > 90)
				throw new ArgumentOutOfRangeException ("Maximalausschlag von 90 Grad kann nicht überschritten werden.");
			
			if (Math.Abs (sollPosition) > MaximalAusschlagGrad)
				return;

			Position = sollPosition;
				
			sollPosition = MaximalAusschlagGrad * Math.Sign (sollPosition);
				
			//Gängig ist ein 50-Hz-Signal (20 ms Periodenlänge), welches zwischen 500 Mikrosekunden (linker Anschlag, 0 Grad) und 2500 Mikrosekunden (rechter Anschlag, 180 Grad) auf High-Pegel und den Rest der Periodenlänge auf Low-Pegel ist.

			float anteilVon180Grad = (sollPosition + 90) / 180;

			float microSec = (MicroSecRange * anteilVon180Grad) + MicroSecLeft;

			int pwmValue = (int) ((microSec * PwmRange) / PeriodeMicroSecs);

			WiringPiLib.PwmWrite (PwmPin, pwmValue);
		}
	}
}

