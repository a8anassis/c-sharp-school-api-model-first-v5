using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.Helpers;
using UsersStudentsAPIApp.Repositories;
using UsersStudentsAPIApp.Services;
using UsersStudentsMVCApp.Configuration;

namespace UsersStudentsAPIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<UsersTeachersAPITestDbContext>(options => options.UseSqlServer(connString));

            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddRepositories();

            //builder.Services.AddAutoMapper(typeof(MapperConfig)); // Singleton

            // Per request scope
            builder.Services.AddScoped(provider =>
                new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MapperConfig());
                })
            .CreateMapper());


            ///Add Authentication
            /*  builder.Services.AddAuthentication(options =>
              {
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              }).AddJwtBearer(options =>
              {
                  options.IncludeErrorDetails = true;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = false,
                      ValidateIssuer = false,
                      ValidateAudience = false,
                      RequireExpirationTime = false,
                      ValidateLifetime = false,
                      /// return new JsonWebToken(token); in .NET 8
                      /// Override the default token signature validation an do NOT validtae the signature
                      /// Just return the token
                      SignatureValidator = (token, validator) => { return new JwtSecurityToken(token); }
                  };
              });
  */

            // For production

            builder.Services.AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           }).AddJwtBearer(options =>
           {
               options.IncludeErrorDetails = true;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = "https://codingfactory.aueb.gr",

                   ValidateAudience = true,
                   ValidAudience = "https://api.codingfactory.aueb.gr",

                   ValidateLifetime = true, // ensure not expired

                   ValidateIssuerSigningKey = true,

                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                       //.GetBytes(builder.Configuration.GetConnectionString("SecretKey")!))
                       .GetBytes(builder.Configuration["Authentiation: SecretKey"]))

               };
           });

            /// System.Text.JSON
            /*builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                // Adding a converter for string enums
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });*/

            // If NewtonSoft would be used for json serialization / deserialization
            // We have to add the NuGet dependencies and the following config

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "School API", Version = "v1" });
                // Non-nullable reference are properly documented
                options.SupportNonNullableReferenceTypes();
                options.OperationFilter<AuthorizeOperationFilter>();
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, 
                    new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });
            });

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
            });

            builder.Services.AddCors(options => {
                options.AddPolicy("AngularClient",
                    b => b.WithOrigins("http://localhost:4200") // Assuming Angular runs on localhost:4200
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseRouting(); included in MapControllers

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            /// Global Error Handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}