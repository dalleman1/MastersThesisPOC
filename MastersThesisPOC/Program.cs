using IronPython.Hosting;
using MastersThesisPOC;
using Microsoft.Extensions.DependencyInjection;

//Dependency Injection
var engine = Python.CreateEngine();

var services = new ServiceCollection();
services.AddSingleton(engine)
    .AddScoped<IPythonHelper, PythonHelper>();

var serviceProvider = services.BuildServiceProvider();

var pythonHelper = serviceProvider.GetService<IPythonHelper>();

var floatNumber = 19.6f;

var newFloat = new CustomFloat(floatNumber);

Console.WriteLine("Floating Point number: " + floatNumber + "\n");

Console.WriteLine("Whole Bit Representation: " + newFloat.ToBitString() + "\n");

Console.WriteLine("Sign: " + newFloat.SignAsBitString + "\n");

Console.WriteLine("Exponent: " + newFloat.ExponentAsBitString + "\n");

Console.WriteLine("Mantissa: " + newFloat.MantissaAsBitString + "\n");

string xmantissa = newFloat.MantissaAsBitString;

Console.WriteLine("First 13 of Python pattern string:");
var result5 = pythonHelper!.GetStringPatternOfInteger(13)[..12];
Console.WriteLine(result5);

Console.WriteLine("Hardcoded pattern:");
string pattern = "000100111011";
Console.WriteLine(pattern);
Console.WriteLine();
Console.WriteLine();

uint mantissauint = newFloat.GetMantissaAsUInt();
Console.WriteLine("Uint mantissa of 19.6: " + mantissauint);

Console.WriteLine("Converted back to a string: " + newFloat.GetMantissaAsStringFromUint(mantissauint));

var algo = new Algorithm();

var res = algo.ExtendMantissaAndGetStringRepresentation(mantissauint, pattern);

Console.WriteLine("Extended mantissa as a string: " + res);