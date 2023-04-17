using MastersThesisPOC;
using Microsoft.Extensions.DependencyInjection;

//Dependency Injection
var services = new ServiceCollection();


var serviceProvider = services.BuildServiceProvider();


//Program

var floatNumber = 19.6f;

var newFloat = new CustomFloat(floatNumber);

Console.WriteLine("Floating Point number: " + floatNumber+ "\n");

Console.WriteLine("Whole Bit Representation: " + newFloat.ToBitString() + "\n");

Console.WriteLine("Sign: " + newFloat.SignAsBitString + "\n");

Console.WriteLine("Exponent: " + newFloat.ExponentAsBitString + "\n");

Console.WriteLine("Mantissa: " + newFloat.MantissaAsBitString + "\n");