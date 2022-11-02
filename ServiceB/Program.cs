using Dapr.Client;
using ServiceB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDaprClient();

builder.Services.AddSingleton<IHelperService>(sc =>
    new HelperService(DaprClient.CreateInvokeHttpClient("serviceA")));


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

// app.UseHttpsRedirection();
app.UseCloudEvents();

app.UseAuthorization();

app.MapSubscribeHandler();
app.MapControllers();

app.Run();