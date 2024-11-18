using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddSingleton<IAcquiringBankService, AcquiringBankWebService>();
builder.Services.AddSingleton<PaymentGatewayConfiguration>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // This is required to stop the Controller action automatically returning a 400 on model validation errors
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();