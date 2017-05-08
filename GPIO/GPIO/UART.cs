using System;
using WiringPiLib;
using System.Threading;

namespace GPIO1
{
	public class UART
	{
		//http://spellfoundry.com/2016/05/29/configuring-gpio-serial-port-raspbian-jessie-including-pi-3/
		//$ sudo nano /boot/config.txt
		//enable_uart=1
		//Disable console
		//$ sudo systemctl stop serial-getty@ttyS0.service
		//$ sudo systemctl disable serial-getty@ttyS0.service
		//$ sudo nano /boot/cmdline.txt
		//remove the line: console=serial0,115200 and save and reboot 

		int fileDescriptor = 0;

		public UART ()
		{
			//Pi3 Path
			int fd = WiringPiWrapperDirect.serialOpen ("/dev/ttyS0", 9600);
			if (fd < 0)
				throw new ExecutionEngineException ("Serial Open fails.");

			WiringPiWrapperDirect.WiringPiSetupGpio ();
		
		}
	 
		public string WriteRead(string input)
		{
			string output = "";

			for (int i = 0; i < input.Length; i++) {
				WiringPiWrapperDirect.serialPutchar(fileDescriptor, input [i]);
				WiringPiWrapperDirect.serialFlush(fileDescriptor);

				while(WiringPiWrapperDirect.serialDataAvail(fileDescriptor) > 0)
				      output += WiringPiWrapperDirect.serialGetchar(fileDescriptor);
			}

			return output;
		}
	}
}

