using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace WiringPiLib
{
	public class WiringPiWrapperLogging : IWiringPiWrapper
	{
		static string lastLogString = "";
		Dictionary<int, string> NamedPins = new Dictionary<int, string>();
		static ILog Log4Net = LogManager.GetLogger("WirinPiWrapperLogging");

		/// <summary>
		/// Optional: You if you call SetPinName(7, "EnablePin");
		/// you'll log "Write Pin EnablePin value 1" instead of "Write Pin 7 value 1"
		/// so it's just for easytoread logfiles
		/// </summary>
		/// <param name="pin">Pin.</param>
		/// <param name="name">Name.</param>
		public void SetPinName(int pin, string name)
		{
			NamedPins.Add (pin, name);
		}

		public int WiringPiSetupGpio ()
		{
			log4net.Config.XmlConfigurator.Configure();
			Log4Net.Info("WiringPiSetupGpio");

			int result = WiringPiWrapperDirect.WiringPiSetupGpio ();

			Log4Net.Info("WiringPiSetupGpio completed");

			return result;
		}

		public int DigitalRead(int pin)
		{
			int result = WiringPiWrapperDirect.digitalRead(pin);
			string pinName = NamedPins.Keys.Contains (pin) ? NamedPins [pin] : pin.ToString() ;

			string logstring = string.Format ("Read Pin {0} returns {1}", pinName, result);
			if(lastLogString.CompareTo(logstring) == 0)
				logstring = string.Format ("Polling Pin {0} returns {1}", pinName, result);

			if(lastLogString.CompareTo(logstring) != 0)
				Log4Net.Info(logstring);

			lastLogString = logstring;

			return result;
		}
		public void DigitalWrite(int pin, int value)
		{
			string pinName = NamedPins.Keys.Contains (pin) ? NamedPins [pin] : pin.ToString() ;
			Log4Net.Info(string.Format("Write Pin {0} value {1}", pinName, value));
			WiringPiWrapperDirect.digitalWrite (pin, value);
		}

		public void DigitalWriteByte(int value)
		{
			Log4Net.Info(string.Format("WriteByte value {0}", value));
			WiringPiWrapperDirect.digitalWriteByte (value);
		}

		public void PullUpDnControl(int pin, PullUpType value)
		{
			string pinName = NamedPins.Keys.Contains (pin) ? NamedPins [pin] : pin.ToString() ;
			Log4Net.Info(string.Format("PullUpDnControl Pin {0} value {1}", pinName, value));
			WiringPiWrapperDirect.pullUpDnControl (pin, value);
		}

		public void PinMode(int pin, PinType mode)
		{
			if(NamedPins.Keys.Contains (pin))
				Log4Net.Info(string.Format("PinMode Pin {0} ({1}) value {2}", NamedPins [pin], pin, mode));
			else
				Log4Net.Info(string.Format("PinMode Pin {0} value {1}", pin, mode));
			
			WiringPiWrapperDirect.pinMode (pin, mode);
		}
		public void DelayMicroseconds (uint howLong)
		{
			Log4Net.Info(string.Format("DelayMicroseconds {0}", howLong));
			WiringPiWrapperDirect.delayMicroseconds (howLong);
		}
		public uint Millis()
		{
			uint millis = WiringPiWrapperDirect.millis ();
			Log4Net.Info(string.Format("Millis {0}", millis));
			return millis;
		}
		public int WiringPiISR(int pin, EdgeType edgeType, ISRCallback method)
		{
			string pinName = NamedPins.Keys.Contains (pin) ? NamedPins [pin] : pin.ToString() ;
			int result = WiringPiWrapperDirect.wiringPiISR (pin, edgeType, method);
			Log4Net.Info(string.Format("WiringPiISR Pin {0} Edge {1} Method {2} returns {3}", pinName, edgeType, method, result));
			return result;
		}
		public void PwmSetMode(PwmType mode)
		{
			Log4Net.Info(string.Format("PwmSetMode {0}", mode));
			WiringPiWrapperDirect.pwmSetMode (mode);
		}
		public void PwmSetRange (uint range)
		{
			Log4Net.Info(string.Format("PwmSetRange {0}", range));
			WiringPiWrapperDirect.pwmSetRange (range);
		}
		public void PwmSetClock(int divisor)
		{
			Log4Net.Info(string.Format("PwmSetClock {0}", divisor));
			WiringPiWrapperDirect.pwmSetClock(divisor);
		}
		public void PwmWrite(int pin, int value)
		{
			string pinName = NamedPins.Keys.Contains (pin) ? NamedPins [pin] : pin.ToString() ;
			Log4Net.Info(string.Format("PwmWrite Pin {0} Value {1}", pinName, value));
			WiringPiWrapperDirect.pwmWrite(pin, value);
		}
	}
}