namespace PaymentGateway.Api.Configuration;

public class PaymentGatewayConfiguration
{
    public IConfiguration Configuration { get; }

    public PaymentGatewayConfiguration(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public string? AcquiringBankUrl => Configuration["AcquiringBankUrl"];
}