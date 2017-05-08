using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiringPiLib
{

	public  interface IWiringPiWrapper
	{
		int WiringPiSetupGpio ();
		int DigitalRead(int pin);
		void DigitalWrite(int pin, int value);
		void DigitalWriteByte(int value);
		void PullUpDnControl(int pin, PullUpType value);
		void PinMode(int pin, PinType mode);
		void DelayMicroseconds (uint howLong);
		uint Millis();
		int WiringPiISR(int pin, EdgeType edgeType, ISRCallback method);
		void PwmSetMode(PwmType mode);
		void PwmSetRange (uint range);
		void PwmSetClock(int divisor); 
		void PwmWrite(int pin, int value);

		int WiringPiI2CSetup (int devId);
		int WiringPiI2CRead(int fd);
		int WiringPiI2CWrite (int fd, int data) ;
		int WiringPiI2CWriteReg8 (int fd, int reg, int data);
		int WiringPiI2CWriteReg16 (int fd, int reg, int data);
		int WiringPiI2CReadReg8 (int fd, int reg) ;
		int WiringPiI2CReadReg16 (int fd, int reg) ;
	}
}