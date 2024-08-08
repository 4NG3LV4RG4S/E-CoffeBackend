using System.IO.Compression;
using System.Text;
using Ixhuatlan.E_Coffe.Backend.Entities.POCO.KeyVaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Ixhuatlan.E_Coffe.WebApi.Configurations;

internal static class ServicesConfigurations
{
    public static WebApplication ConfigurationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("BearerToken",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Ingresa el valor del token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                }
            );

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "BearerToken"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(builder.Configuration["Authentication:SecretKey"] ??
                                      throw new InvalidOperationException())),
                        ValidAudience = builder.Configuration["Authentication:ValidAudience"],
                        ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
                    };
                }
            );

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Ixhuatlan.E-Coffe.WebApi.", Version = "v1" });
        });

        builder.Services.AddControllers();

        builder.Services.AddResponseCompression(config =>
        {
            config.Providers.Add<BrotliCompressionProvider>();
            config.MimeTypes =
                ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream", "application/json"]);
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        if (builder.Configuration.GetSection(EnviromentOptions.SectionKey).Get<EnviromentOptions>() is
            { } enviromentOptions)
            builder.Configuration.AddJsonFile($"appsettings.{enviromentOptions.EnvironmentName}.json", true, true);

        builder.Services.AddHttpClient();
        if (builder.Configuration.GetSection(ConnectionStringsOptions.SectionKey).Get<ConnectionStringsOptions>() is
            { } connectionStringsOptions)
        {
            var configurationSection = builder.Configuration.GetSection(ConnectionStringsOptions.SectionKey);
            builder.Services.Configure<ConnectionStringsOptions>(configurationSection);

            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            builder.Services.TryAddSingleton(new HttpClient(clientHandler));
        }

        return builder.Build();
    }
}