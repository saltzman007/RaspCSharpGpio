# RaspCSharpGpio
# RaspCSharpGpio

This is a wrapper project for wiringpi in C#.
The wrapper comes with an interface, so you can mock it for testing and you also got a implementation with log4net-logging.
The direct way without the interface is the fastest. The implementation may look strange, but mono is rather slow with the dll-calls. These calls happen here just at starttime, the funnctionadresses are kept as static delegates. This brings the .net code to the speed dimensions of unmanaged gpio benchmarks.
