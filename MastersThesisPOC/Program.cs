using MastersThesisPOC;
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
    .AddScoped<IPredictM, PredictM>()
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
var mPredicter = serviceProvider.GetRequiredService<IPredictM>();

var testDict = new Dictionary<float, string>();

testDict.Add(3f, "01");
testDict.Add(5f, "0011");
testDict.Add(7f, "001");
testDict.Add(9f, "000111");
testDict.Add(11f, "0001011101");
testDict.Add(13f, "000100111011");
testDict.Add(15f, "0001");
testDict.Add(17f, "11100001");
testDict.Add(19f, "101011110010100001");
testDict.Add(19.5f, "101001000001");
testDict.Add(23f, "01100100001");


var voltageFloats = csvReader.ReadCsvColumnVoltage("C:\\Users\\mongl\\OneDrive\\Skrivebord\\household_power_consumption.csv");
var pondTempFloats = csvReader.ReadCsvColumPondTemp("C:\\Users\\mongl\\OneDrive\\Skrivebord\\IoTpond1.csv");
var humidityFloats = csvReader.ReadCsvColumnHumidityReal("C:\\Users\\mongl\\OneDrive\\Skrivebord\\humidity.csv");

var currentDataSet = pondTempFloats;
//var currentDataSet = voltageFloats;
//var currentDataSet = humidityFloats;

var prediction = mPredicter.PredictBestM(currentDataSet);

foreach (var (key, value) in prediction)
{
    Console.WriteLine($"Prediction is: {key},{value}");
}

var listOfBestPrediction = mPredicter.DetermineBestM(prediction, testDict);

foreach (var (determinedM, valueOfDeterminedM, score) in listOfBestPrediction) 
{ 
    Console.WriteLine($"Prediction of best M is: {determinedM}, {valueOfDeterminedM}, Score: {score}");
    logger.Information($"Prediction of best M is: {determinedM}, {valueOfDeterminedM}, Score: {score}");
}

//var bestPrediction = listOfBestPrediction.Take(1);
IEnumerable<(float, string, int)> bestPrediction = new List<(float, string, int)>
{
    (5f, "1100", 0)
};


var name = "Temperature";

logger.Information($"Max value of the dataset is: {currentDataSet.Max()}");
logger.Information($"Min value of the dataset is: {currentDataSet.Min()}");
logger.Information($"Avg of the dataset is: {currentDataSet.Average()}");
var resultFromProgram = new List<float>();

var stopWatch = new Stopwatch();
long totalMemory = GC.GetTotalMemory(false);
Console.WriteLine($"Total Memory Used Before(Bytes): {totalMemory}");
foreach (var (M, pattern, score) in bestPrediction)
{
     for (int i = 2; i < 18; i++)
     {
        var epsilonList = new [] { 0.01f, 0.025f, 0.05f, 0.075f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.5f, 2.0f, 5f, 10f};
        //var epsilonList = new[] {1.0f, 2.0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f, 75f, 100f };
        var substituteIndex = i;
        logger.Information($"Substitution Index: {i}");
        stopWatch.Start();
        resultFromProgram = executer.RunProgramFastNoiseBeforePattern(M, pattern, currentDataSet, 0, 20, 1.0f, 5.0f, i);
        //resultFromProgram = executer.RunProgramOnlyPrivacy(M, pattern, currentDataSet, i, 20, 1f, 5f);

        stopWatch.Stop();

        logger.Information($"Time Elapsed of Compression (s): {stopWatch.Elapsed.TotalSeconds}");

        stopWatch.Reset();
        stopWatch.Start();

        string nameOfOutput = $"M{M}-" + name + "_compressible-" + Guid.NewGuid().ToString() + ".csv";

        string path = @"C:\Users\mongl\OneDrive\Skrivebord\Master thesis material\GD\\aaron-gd-aqp-main\" + nameOfOutput;

        string columnName = name;

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
        long totalMemory2 = GC.GetTotalMemory(false);
        Console.WriteLine($"Total Memory Used After(Bytes): {totalMemory2}");
        logger.Information($"Memory used in Bytes: {totalMemory2-totalMemory}");
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

