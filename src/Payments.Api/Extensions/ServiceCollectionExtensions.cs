using System.Text;
using Confluent.Kafka;
using Microsoft.IdentityModel.Tokens;
using Payments.Application.Producers;
using Payments.Infrastructure.Producers;

namespace Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKafkaProducers(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = new ProducerConfig();
        configuration.GetSection("Kafka:PaymentsEvents:ProducerSettings").Bind(kafkaConfig);
        services.AddSingleton<IProducer<Null, string>>(
            _ => new ProducerBuilder<Null, string>(kafkaConfig)
                .SetValueSerializer(Serializers.Utf8)
                .Build());
        services.AddSingleton<IIntegrationEventProducer, KafkaIntegrationEventProducer>();
    }
    
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        const string jwtOptionsConfigurationKey = "JwtOptions:Key";
        services
            .AddAuthentication()
            .AddJwtBearer(options =>
            {
                var key = configuration[jwtOptionsConfigurationKey] 
                          ?? throw new ArgumentNullException(jwtOptionsConfigurationKey);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
    }
}