using IronPython.Hosting;
using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Python;
using Microsoft.Extensions.DependencyInjection;

//Dependency Injection
var engine = Python.CreateEngine();

var services = new ServiceCollection();
services.AddSingleton(engine)
    .AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IAlgorithm, Algorithm>()
    .AddScoped<IProgramInstances, ProgramInstances>()
    .AddScoped<IPythonHelper, PythonHelper>();

var serviceProvider = services.BuildServiceProvider();

var pythonHelper = serviceProvider.GetService<IPythonHelper>();
var algoHelper = serviceProvider.GetService<IAlgorithmHelper>();
var algorithm = serviceProvider.GetService<IAlgorithm>();
var instance = serviceProvider.GetRequiredService<IProgramInstances>();

var dict = new Dictionary<float, string>();

var list = new List<float> { 2.2f, 3.4f, 3.21f, 4.59f, 10.0f, 8, 8.54f, 1.78f };

dict.Add(3f, "01");
dict.Add(5f, "0011");
dict.Add(7f, "001");
dict.Add(9f, "000111");
dict.Add(11f, "0001011101");
dict.Add(13f, "000100111011");

instance.ComputeBestMWithRounding(dict, list, 4, 20);

instance.ComputeBestMWithNoRounding(dict, list, 4, 0);

/*
var floatNumber = 19.6f;

var newFloat = new CustomFloat(floatNumber);

Console.WriteLine("Floating Point number: " + floatNumber + "\n");

Console.WriteLine("Mantissa: " + newFloat.MantissaAsBitString + "\n");

string xmantissa = newFloat.MantissaAsBitString;


//var result5 = pythonHelper!.GetStringPatternOfInteger(13)[..12];
var result5 = GetStringPatternOfFloat(13f)[..12];
var result6 = GetStringPatternOfInteger(13)[..12];
Console.WriteLine("Pattern of 13, but I generated it: " + result6);
Console.WriteLine("Pattern of the number 13: " + "000100111011");
//string pattern = "000100111011";
var pattern = result5;
Console.WriteLine(pattern);

var patternsResult = algoHelper!.FindPatterns(pattern);

Console.WriteLine("All possible patterns: \n");

foreach (var item in patternsResult)
{
    Console.WriteLine($"{item}");
}

Console.WriteLine();

uint mantissauint = newFloat.GetMantissaAsUInt();
Console.WriteLine("Uint mantissa of 19.6: " + mantissauint);

Console.WriteLine("Converted back to a string: " + newFloat.GetMantissaAsStringFromUint(mantissauint));

foreach (var Currentpattern in patternsResult)
{
    var res = algorithm!.RunAlgorithm(Currentpattern, mantissauint, 13f, floatNumber);
    Console.WriteLine($"Pattern used: {Currentpattern}");
    Console.WriteLine(res);
}


static string GetStringPatternOfFloat(float input)
{
    byte[] fpNumberBytes;
    uint mantissaFloat;
    string mantissaFloatBinary;

    using (MemoryStream memoryStream = new MemoryStream())
    {
        using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(1.0f / input);
            fpNumberBytes = memoryStream.ToArray();
        }
    }

    mantissaFloat = BitConverter.ToUInt32(fpNumberBytes, 0) & 0x007FFFFF;
    mantissaFloatBinary = Convert.ToString(mantissaFloat, 2).PadLeft(23, '0');

    return mantissaFloatBinary;
}

static string GetStringPatternOfInteger(float input)
{
    byte[] fpNumberBytes;
    uint mantissaInt;
    string mantissaIntBinary;

    using (MemoryStream memoryStream = new MemoryStream())
    {
        using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
        {
            binaryWriter.Write(1.0f / input);
            fpNumberBytes = memoryStream.ToArray();
        }
    }

    mantissaInt = BitConverter.ToUInt32(fpNumberBytes, 0) & 0x007FFFFF;
    mantissaIntBinary = Convert.ToString(mantissaInt, 2).PadLeft(23, '0');

    return mantissaIntBinary;
}
*/