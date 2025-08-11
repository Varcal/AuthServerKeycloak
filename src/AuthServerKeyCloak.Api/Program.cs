using AuthServerKeyCloak.Api.Handlers;
using AuthServerKeyCloak.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


namespace AuthServerKeyCloak.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var url = builder.Configuration.GetValue<string>("Keycloak:BaseUrl");

            builder.Services.AddHttpClient<IKeycloakServices, KeycloakServices>(c =>
            {
                c.BaseAddress = new Uri(url);
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = url;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        RoleClaimType = "roles"
                    };
                });

            builder.Services.AddSingleton<IAuthorizationHandler, RealmAdminHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RealmAdmin", policy => policy.Requirements.Add(new RealmAdminRequirement()));
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer Keycloak API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Bearer {seu_token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    // Caminho RELATIVO ao /swagger, assim evita erro de rota
                    c.SwaggerEndpoint("v1/swagger.json", "AuthServer Keycloak API v1");
                    c.RoutePrefix = "swagger"; // Abre em /swagger
                });
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
