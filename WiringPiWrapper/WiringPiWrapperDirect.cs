using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiringPiLib
{
	public enum PinType 
	{
		INPUT			 = 0, 
		OUTPUT			 = 1, 
		PWM_OUTPUT		 = 2,
		GPIO_CLOCK		 = 3
	}

	public enum PullUpType
	{
		PUD_OFF			 = 0, 
		PUD_DOWN		 = 1, 
		PUD_UP			 = 2
	}

	public enum  PwmType
	{
		PWM_MODE_MS		= 0,
 		PWM_MODE_BAL	= 1
	}

	public enum EdgeType 
	{
		INT_EDGE_SETUP		= 0, 
		INT_EDGE_FALLING	= 1,
		INT_EDGE_RISING		= 2,
	 	INT_EDGE_BOTH		= 3
	}

	public delegate void ISRCallback();

	public delegate int WiringPiSetup();	//0 .. 16 = GPIO 0 .. 16
	public delegate int WiringPiSetupGpio();//Direkte Verqwendung der Pin Nummern
	public delegate int WiringPiSetupPhys();//Physikalische Pin Nummern, wasimmer das ist
	public delegate int WiringPiSetupSys();	//Über sys/class/gpio, also langsam
	public delegate int DigitalRead(int pin);
	public delegate void DigitalWrite(int pin, int value);
	public delegate void DigitalWriteByte(int value);
	public delegate void PullUpDnControl(int pin, PullUpType value);
	public delegate void PinMode(int pin, PinType mode);
	public delegate void DelayMicroseconds (uint howLong);
	public delegate uint Millis();
	public delegate int WiringPiISR(int pin, EdgeType edgeType, ISRCallback method);
	public delegate void PwmSetMode(PwmType mode);
	public delegate void PwmSetRange (uint range);
	public delegate void PwmSetClock(int divisor); 
	public delegate void PwmWrite(int pin, int value);

	//public delegate int SerialOpen (char *device, int baud);
	public delegate int SerialOpen (string device, int baud);	//Ist string gut??
	public delegate void SerialClose (int fd);
	public delegate void  SerialPutchar (int fd, char c);
	//public delegate void  SerialPuts (int fd, char *s) ;
	public delegate void  SerialPuts (int fd, string s) ;
	public delegate int SerialDataAvail (int fd);
	public delegate int SerialGetchar (int fd) ;
	public delegate void SerialFlush (int fd) ;

	public delegate int WiringPiI2CSetup (int devId);
	public delegate int WiringPiI2CRead(int fd);
	public delegate int WiringPiI2CWrite (int fd, int data) ;
	public delegate int WiringPiI2CWriteReg8 (int fd, int reg, int data);
	public delegate int WiringPiI2CWriteReg16 (int fd, int reg, int data);
	public delegate int WiringPiI2CReadReg8 (int fd, int reg) ;
	public delegate int WiringPiI2CReadReg16 (int fd, int reg) ;

	public class WiringPiWrapperDirect
	{
		public static DigitalRead digitalRead = digitalReadRaw;
		public static DigitalWrite digitalWrite = digitalWriteRaw;
		public static DigitalWriteByte digitalWriteByte = digitalWriteByteRaw;
		public static WiringPiSetupGpio WiringPiSetupGpio = WiringPiSetupGpioRaw;
		public static WiringPiSetup WiringPiSetup = WiringPiSetupRaw;
		public static WiringPiSetupPhys WiringPiSetupPhys = WiringPiSetupPhysRaw;
		public static WiringPiSetupSys WiringPiSetupSys = WiringPiSetupSysRaw;
		public static PinMode pinMode = pinModeRaw;
		public static DelayMicroseconds delayMicroseconds = delayMicrosecondsRaw;
		public static Millis millis = millisRaw;
		public static PullUpDnControl pullUpDnControl = pullUpDnControlRaw;
		public static WiringPiISR wiringPiISR = wiringPiISRRaw;
		public static PwmSetMode pwmSetMode = pwmSetModeRaw;
		public static PwmSetRange pwmSetRange = pwmSetRangeRaw;
		public static PwmSetClock pwmSetClock = pwmSetClockRaw;
		public static PwmWrite pwmWrite = pwmWriteRaw;

		public static SerialOpen serialOpen = serialOpenRaw;
		public static SerialClose serialClose = serialCloseRaw;
		public static  SerialPutchar serialPutchar = serialPutcharRaw;
		public static  SerialPuts serialPuts = serialPutsRaw;
		public static SerialDataAvail serialDataAvail = serialDataAvailRaw;
		public static SerialGetchar serialGetchar = serialGetcharRaw;
		public static SerialFlush serialFlush = serialFlushRaw;

		public static WiringPiI2CSetup wiringPiI2CSetup = wiringPiI2CSetupRaw;
		public static WiringPiI2CRead wiringPiI2CRead = wiringPiI2CReadRaw;
		public static WiringPiI2CWrite wiringPiI2CWrite = wiringPiI2CWriteRaw;
		public static WiringPiI2CWriteReg8 wiringPiI2CWriteReg8 = wiringPiI2CWriteReg8Raw;
		public static WiringPiI2CWriteReg16 wiringPiI2CWriteReg16 = wiringPiI2CWriteReg16Raw;
		public static WiringPiI2CReadReg8 wiringPiI2CReadReg8 = wiringPiI2CReadReg8Raw;
		public static WiringPiI2CReadReg16 wiringPiI2CReadReg16 = wiringPiI2CReadReg16Raw;

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiSetup")]
		private static extern int WiringPiSetupRaw();

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupGpio")]
		private static extern int WiringPiSetupGpioRaw();

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupSys ")]
		private static extern int WiringPiSetupSysRaw();

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiSetupPhys ")]
		private static extern int WiringPiSetupPhysRaw ();

		[DllImport("libwiringPi.so", EntryPoint = "pinMode")]
		private static extern void pinModeRaw(int pin, PinType mode);

		[DllImport("libwiringPi.so", EntryPoint = "digitalWrite")]
		private static extern void digitalWriteRaw(int pin, int value);

		[DllImport("libwiringPi.so", EntryPoint = "digitalWriteByte")]
		private static extern void digitalWriteByteRaw(int value);

		[DllImport("libwiringPi.so", EntryPoint = "pwmWrite")]
		private static extern void pwmWriteRaw(int pin, int value);

		[DllImport("libwiringPi.so", EntryPoint = "digitalRead")]
		private static extern int digitalReadRaw(int pin);

		[DllImport("libwiringPi.so", EntryPoint = "pullUpDnControl")]
		private static extern void pullUpDnControlRaw(int pin, PullUpType pud);

		[DllImport("libwiringPi.so", EntryPoint = "analogRead")]
		private static extern void analogRead (int pin);

		[DllImport("libwiringPi.so", EntryPoint = "analogWrite")]
		private static extern void analogWrite (int pin, int value);

		[DllImport("libwiringPi.so", EntryPoint = "pwmSetMode")]
		private static extern void pwmSetModeRaw(PwmType mode);

		[DllImport("libwiringPi.so", EntryPoint = "pwmSetRange")]
		private static extern void pwmSetRangeRaw (uint range);

		[DllImport("libwiringPi.so", EntryPoint = "pwmSetClock")]
		private static extern void pwmSetClockRaw(int divisor); 

		[DllImport("libwiringPi.so", EntryPoint = "millis")]
		private static extern uint millisRaw();

		[DllImport("libwiringPi.so", EntryPoint = "delay")]
		private static extern void delay(uint howLong);

		[DllImport("libwiringPi.so", EntryPoint = "delayMicroseconds")]
		private static extern void delayMicrosecondsRaw (uint howLong);

		[DllImport("libwiringPi.so", EntryPoint = "micros")]
		private static extern uint micros ();

		[DllImport("libwiringPi.so", EntryPoint = "piHiPri")]
		private static extern int piHiPri(int priority);

		[DllImport("libwiringPi.so", EntryPoint = "waitForInterrupt")]
		private static extern int waitForInterrupt(int pin, int timeOut);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiISR")]
		private static extern int wiringPiISRRaw(int pin, EdgeType edgeType, ISRCallback method);  //no idea on how to convert these

		[DllImport("libwiringPi.so", EntryPoint = "piThreadCreate")]
		private static extern int piThreadCreate (string name);              //string name

		[DllImport("libwiringPi.so", EntryPoint = "piLock")]
		private static extern void piLock (int keyNum);

		[DllImport("libwiringPi.so", EntryPoint = "piUnlock")]
		private static extern void piUnlock (int keyNum);

		[DllImport("libwiringPi.so", EntryPoint = "piBoardRev")]
		private static extern void piBoardRev ();

		[DllImport("libwiringPi.so", EntryPoint = "wpiPinToGpio")]
		private static extern void wpiPinToGpio (int wPiPin);

		[DllImport("libwiringPi.so", EntryPoint = "setPadDrive")]
		private static extern void setPadDrive (int group, int value);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CSetup")]
		private static extern int wiringPiI2CSetupRaw (int devId);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CRead")]
		private static extern int wiringPiI2CReadRaw(int fd);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWrite")]
		private static extern int wiringPiI2CWriteRaw (int fd, int data) ;

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWriteReg8")]
		private static extern int wiringPiI2CWriteReg8Raw (int fd, int reg, int data);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CWriteReg16")]
		private static extern int wiringPiI2CWriteReg16Raw (int fd, int reg, int data);

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CReadReg8")]
		private static extern int wiringPiI2CReadReg8Raw (int fd, int reg) ;

		[DllImport("libwiringPi.so", EntryPoint = "wiringPiI2CReadReg16")]
		private static extern int wiringPiI2CReadReg16Raw (int fd, int reg) ;

		//pwm evtl Blödsinn: https://dzone.com/articles/controlling-led-raspberry-pi-2
		[DllImport("libwiringPi.so", EntryPoint = "softPwmCreate")]
		private static extern void Create(int pin, int initialValue, int pwmRange);

		[DllImport("libwiringPi.so", EntryPoint = "softPwmWrite")]
		private static extern void Write(int pin, int value);

		[DllImport("libwiringPi.so", EntryPoint = "softPwmStop")]
		private static extern void Stop(int pin);

		[DllImport("libwiringPi.so", EntryPoint = "serialOpen")]
		private static extern int serialOpenRaw ([MarshalAs(UnmanagedType.LPStr)]string device, int baud) ;
		[DllImport("libwiringPi.so", EntryPoint = "serialClose")]
		private static extern void serialCloseRaw (int fd);
		[DllImport("libwiringPi.so", EntryPoint = "serialPutchar")]
		private static extern void  serialPutcharRaw (int fd, char c);
		[DllImport("libwiringPi.so", EntryPoint = "serialPuts")]
		private static extern void  serialPutsRaw (int fd, [MarshalAs(UnmanagedType.LPStr)]string s) ;
		[DllImport("libwiringPi.so", EntryPoint = "serialDataAvail")]
		private static extern int serialDataAvailRaw (int fd) ;
		[DllImport("libwiringPi.so", EntryPoint = "serialGetchar")]
		private static extern int serialGetcharRaw (int fd) ;
		[DllImport("libwiringPi.so", EntryPoint = "serialFlush")]
		private static extern void serialFlushRaw (int fd) ;
	}
}