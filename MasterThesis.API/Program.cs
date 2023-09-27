using IronPython.Hosting;
using MastersThesisPOC.Algorithm;
using MastersThesisPOC.CustomMath;
using MastersThesisPOC.Python;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var engine = Python.CreateEngine();

builder.Services.AddSingleton(engine)
    .AddScoped<IAlgorithmHelper, AlgorithmHelper>()
    .AddScoped<IMathComputer, MathComputer>()
    .AddScoped<IPythonHelper, PythonHelper>();

// Configure CORS to allow any origin, method, and header. Adjust this to your needs.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use CORS policy
app.UseCors();

app.MapControllers();

app.Run();
