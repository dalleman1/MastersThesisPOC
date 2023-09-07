using IronPython.Hosting;
using MastersThesisPOC;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.Python;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IAlgorithmHelper, AlgorithmHelper>()
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

    var (res, res2) = executer.ExecuteWithTrailingOnesWithNoRounding(dict, listOfFloats, i);

    executer.PrintMPerformance(res, res2, "Ones");
}

