﻿using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.Text;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<ILaplaceNoiseGenerator, LaplaceNoiseGenerator>()
    .AddScoped<ICsvReader, CsvReader>()
    .AddScoped<ICompressionMechanism, CompressionMechanism>()
    .AddScoped<ICompressionExecuter, CompressionExecuter>()
    .AddScoped<ICsvWriter, CsvWriter>()
    .AddScoped<IMetrics, Metrics>()
    .AddSingleton<ILogger>(sp => new LoggerConfiguration()
    .WriteTo.File($"C:\\Users\\mongl\\OneDrive\\Skrivebord\\logs\\log-{DateTime.Now:yyyyMMdd-HHmmss}.txt")
    .CreateLogger());

var serviceProvider = services.BuildServiceProvider();

var executer = serviceProvider.GetRequiredService<ICompressionExecuter>();
var csvReader = serviceProvider.GetRequiredService<ICsvReader>();
var csvWriter = serviceProvider.GetRequiredService<ICsvWriter>();
var logger = serviceProvider.GetRequiredService<ILogger>();

var dict = new Dictionary<float, string>();
var testDict = new Dictionary<float, string>();

dict.Add(3f, "01");
dict.Add(3.5f, "001");
dict.Add(5f, "0011");
dict.Add(7f, "001");
dict.Add(7.5f, "0001");
dict.Add(9f, "000111");
dict.Add(11f, "0001011101");
dict.Add(13f, "000100111011");
dict.Add(15f, "0001");
dict.Add(15.5f, "00001");
dict.Add(17f, "11100001");
dict.Add(19f, "101011110010100001");
dict.Add(19.5f, "101001000001");
dict.Add(21f, "100001");
dict.Add(23f, "01100100001");
dict.Add(31f, "00001");


//testDict.Add(3f, "01");
//testDict.Add(5f, "0011");
//testDict.Add(7f, "001");
testDict.Add(9f, "000111");
//testDict.Add(11f, "0001011101");
//testDict.Add(13f, "000100111011");
//testDict.Add(15f, "0001");

//testDict.Add(17f, "11100001");

var temperatureFloats = csvReader.ReadCsvColumn("C:\\Users\\mongl\\source\\repos\\MastersThesisPOC\\MastersThesisPOC\\melbourne-smart_city.csv");
var humidityFloats = csvReader.ReadCsvColumnHumidity("C:\\Users\\mongl\\source\\repos\\MastersThesisPOC\\MastersThesisPOC\\melbourne-smart_city.csv");
var voltageFloats = csvReader.ReadCsvColumnVoltage("C:\\Users\\mongl\\OneDrive\\Skrivebord\\household_power_consumption.csv");

var voltageExponent7 = voltageFloats.Where(x => ExtractExponent(x) == 7).ToList();

// Splitting the dataset based on exponent
var exponent2List = temperatureFloats.Where(x => ExtractExponent(x) == 2).ToList();
var exponent3List = temperatureFloats.Where(x => ExtractExponent(x) == 3).ToList();
var exponent4List = temperatureFloats.Where(x => ExtractExponent(x) == 4).ToList();
var exponent5List = temperatureFloats.Where(x => ExtractExponent(x) == 5).ToList();

// Splitting the dataset based on exponent
var humidityexponentList0 = humidityFloats.Where(x => ExtractExponent(x) == 0).ToList();
var humidityexponentList1 = humidityFloats.Where(x => ExtractExponent(x) == 1).ToList();
var humidityexponentList2 = humidityFloats.Where(x => ExtractExponent(x) == 2).ToList();
var humidityexponentList3 = humidityFloats.Where(x => ExtractExponent(x) == 3).ToList();
var humidityexponentList4 = humidityFloats.Where(x => ExtractExponent(x) == 4).ToList();
var humidityexponentList5 = humidityFloats.Where(x => ExtractExponent(x) == 5).ToList();
var humidityexponentList6 = humidityFloats.Where(x => ExtractExponent(x) == 6).ToList();


List<List<float>> listOfListsTemperature = new List<List<float>>() { exponent2List, exponent3List, exponent4List, exponent5List };
List<List<float>> listOfListsHumidity = new List<List<float>>() { humidityexponentList0, humidityexponentList1, humidityexponentList2, humidityexponentList3, humidityexponentList4, humidityexponentList5, humidityexponentList6};


var resultFromProgram = new List<float>();

var stopWatch = new Stopwatch();


foreach (var (M, pattern) in testDict)
{
     for (int i = 3; i < 4; i++)
     {
        stopWatch.Start();

        resultFromProgram = executer.RunProgram(M, pattern, voltageExponent7, i, 100);


        stopWatch.Stop();

        logger.Information($"Time Elapsed of Compression (s): {stopWatch.Elapsed.TotalSeconds}");

        stopWatch.Reset();
        stopWatch.Start();

        string nameOfOutput = $"M{M}-" + "voltage_compressible-" + Guid.NewGuid().ToString() + ".csv";

        string path = @"C:\Users\mongl\OneDrive\Skrivebord\Master thesis material\GD\\aaron-gd-aqp-main\" + nameOfOutput;

        string columnName = "voltage";

        csvWriter.WriteToCsv(columnName, resultFromProgram, path);

        string pathToScript = @"C:\Users\mongl\OneDrive\Skrivebord\Master thesis material\GD\aaron-gd-aqp-main\compressionTester.py";

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "\"C:\\Users\\mongl\\OneDrive\\Skrivebord\\Master thesis material\\GD\\aaron-gd-aqp-main\\.env\\Scripts\\python.exe\"";
        start.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\"", pathToScript, path, columnName);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        using (Process process = new Process())
        {
            process.StartInfo = start;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();

            process.WaitForExit();  // Wait for the process to finish

            using (StreamReader reader = process.StandardOutput)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line); // or log it using your logger

                    logger.Information(line);
                }
            }

        }

        stopWatch.Stop();
        logger.Information($"Time Elapsed of GD Compression (s): {stopWatch.Elapsed.TotalSeconds}");
        stopWatch.Reset();
        logger.Information($"___________________________________________________________________________");
        Serilog.Log.CloseAndFlush();
    }
}



// Function to extract the exponent from a float
int ExtractExponent(float value)
{
    byte[] bytes = BitConverter.GetBytes(value);
    int intRepresentation = BitConverter.ToInt32(bytes, 0);
    int exponentPart = (intRepresentation >> 23) & 0xFF;  // Extract the 8-bit exponent
    return exponentPart - 127;  // Return the effective exponent
}

