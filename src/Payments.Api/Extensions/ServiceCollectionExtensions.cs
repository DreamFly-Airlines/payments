using System.Security.Claims;
using System.Text;
using Confluent.Kafka;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payments.Api.Authorization;
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
    
    public static void AddAuthenticationWithJwt(this IServiceCollection services, IConfiguration configuration)
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

    public static void AddSwaggerGenWithJwtAuthentication(this IServiceCollection services)
    {
        const string securityName = "Bearer";
        const string schemeName = "bearer";
        const string tokenTypeName = "JWT";
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(securityName, new OpenApiSecurityScheme
            {
                Description = $"{tokenTypeName} Authorization header using the {schemeName} scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = schemeName,
                BearerFormat = tokenTypeName
            });

            var securityRequirement = new OpenApiSecurityRequirement();
            var securityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, Id = securityName
                }
            };
            securityRequirement[securityScheme] = [];
            options.AddSecurityRequirement(securityRequirement);
        });
    }

    public static void AddAuthorizationWithPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.HasNameIdentifier, policy => policy.RequireClaim(ClaimTypes.NameIdentifier));
    }
}