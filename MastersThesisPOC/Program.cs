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
var result5 = pythonHelper!.GetStringPatternOfInteger(13)[..13];
Console.WriteLine(result5);

Console.WriteLine("Hardcoded pattern:");
string pattern = "000100111011";
Console.WriteLine(pattern);
Console.WriteLine();
Console.WriteLine();

string result = InsertPatternInMantissa(xmantissa, pattern, 5);
Console.WriteLine(result);


Console.WriteLine(newFloat.SignAsBitString + newFloat.ExponentAsBitString + result);

var resultfloat = ConvertToFloat(newFloat.SignAsBitString, newFloat.ExponentAsBitString, result);

Console.WriteLine(resultfloat);

var y = resultfloat * 13f;

Console.WriteLine(y);

var yfloat = new CustomFloat(y);

Console.WriteLine(yfloat.MantissaAsBitString);

static string InsertPatternInMantissa(string xmantissa, string pattern, int spot)
{
    // Create a char array from the mantissa string
    char[] mantissaChars = xmantissa.ToCharArray();

    // Loop through the pattern and insert it into the mantissa
    for (int i = 0; i < pattern.Length; i++)
    {
        int index = spot + i - 1; // Calculate the index in the mantissa
        mantissaChars[index] = pattern[i]; // Replace the character at the index
    }

    // Convert the char array back to a string and return it
    return new string(mantissaChars);
}

static float ConvertToFloat(string sign, string exponent, string mantissa)
{
    string binary = sign + exponent + mantissa;
    int intRep = Convert.ToInt32(binary, 2);
    float floatRep = BitConverter.ToSingle(BitConverter.GetBytes(intRep), 0);
    return floatRep;
}
