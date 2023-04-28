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

var (res2, nextbit) = algo.InfinitelyReplaceMantissaWithPattern(pattern, res);

Console.WriteLine("Pattern repeated in mantissa: " + res2);
Console.WriteLine("The next bit would have been:" + nextbit);

var res3 = algo.RemoveExtension(res2, pattern);

Console.WriteLine("Removed Extension: " + res3);

var roundedMantissa = RoundMantissa(res3, nextbit);

Console.WriteLine("Rounded mantissa as a string: " + roundedMantissa);

Console.WriteLine("Uint of rounded mantissa: " + Convert.ToUInt32(roundedMantissa, 2));


var result = algo.ConvertToFloat(newFloat.SignAsBitString, newFloat.ExponentAsBitString, roundedMantissa);

Console.WriteLine("New float value with new mantissa:" + result);

var newResult = result * 13;

Console.WriteLine("Float after being multiplied by M (13): " + newResult);

var customNewResult = new CustomFloat(newResult);

Console.WriteLine($"Sign of the result: {customNewResult.SignAsBitString}, Exponent: {customNewResult.ExponentAsBitString}, Mantissa: {customNewResult.MantissaAsBitString}");

Console.WriteLine("The delta of the new result is: " + Math.Abs(floatNumber - result));

static string RoundMantissa(string mantissaString, string nextBit)
{
    uint mantissa = Convert.ToUInt32(mantissaString, 2);
    uint result = mantissa;

    if (nextBit == "1")
    {
        result += 1;
    }
    else if (nextBit == "0")
    {
        // do nothing
    }
    else
    {
        throw new ArgumentException("nextBit must be either 0 or 1");
    }

    return Convert.ToString(result, 2).PadLeft(mantissaString.Length, '0');
}