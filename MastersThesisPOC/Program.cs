using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<IAlgorithm, Algorithm>()
    .AddScoped<TrailingZerosStrategy>()
    .AddScoped<TrailingOnesStrategy>()
    .AddScoped<IServiceExecuter, ServiceExecuter>(serviceProvider =>
            {
                var algorithmHelper = serviceProvider.GetRequiredService<IAlgorithmHelper>();
                var trailingZeros = new ProgramInstances(algorithmHelper, serviceProvider.GetRequiredService<TrailingZerosStrategy>());
                var trailingOnes = new ProgramInstances(algorithmHelper, serviceProvider.GetRequiredService<TrailingOnesStrategy>());
                return new ServiceExecuter(trailingZeros, trailingOnes);
            });

var serviceProvider = services.BuildServiceProvider();

var executer = serviceProvider.GetRequiredService<IServiceExecuter>();
var mathComputer = serviceProvider.GetRequiredService<IMathComputer>();

float number = 123.456f;

for (int i = 0; i < 10; i++)
{
    float modifiedNumber = mathComputer.AddLaplaceNoiseToMantissa(1.0f, number, 8, false);

    Console.WriteLine(modifiedNumber);
}


/*
var dict = new Dictionary<float, string>();

dict.Add(3f, "01");
dict.Add(5f, "0011");
dict.Add(5.5f, "0111010001");
dict.Add(7f, "001");
dict.Add(9f, "000111");
dict.Add(11f, "0001011101");
dict.Add(13f, "000100111011");
dict.Add(13.5f, "001011110110100001");

var listOfFloats = executer.GenerateFloats(1000);


for (int i = 0; i < 20; i++)
{
    Console.WriteLine("\n\n");
    Console.WriteLine("Starting from index: " + i + "\n");

    var (res, res2) = executer.ExecuteWithTrailingZerosWithRounding(dict, listOfFloats, i, 100);

    executer.PrintMPerformance(res, res2, "zeros");
}

*/

