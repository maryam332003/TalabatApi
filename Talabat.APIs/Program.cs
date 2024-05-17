using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Talabat.APIs.Extensions;
using Talabat.APIs.MiddleWares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;
using Talabat.Service;
using System.Text;
using Talabat.APIs.Models;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services
            webApplicationBuilder.Services.AddControllers();
            //Register Built-in APIs services at the container
            //webApplicationBuilder.Services.AddControllersWithViews();
            //webApplicationBuilder.Services.AddRazorPages();
            //webApplicationBuilder.Services.AddMvc();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();


            webApplicationBuilder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString(name: "DefaultConnection"));
            });
            webApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString(name: "IdentityConnection"));
            });

            //webApplicationBuilder.Services.AddSingleton<IEmailService, EmailService>();


            //var emailConfig =webApplicationBuilder.Configuration
            //                                     .GetSection("EmailConfiguration")
            //                                     .Get<EmailConfig>();

            //webApplicationBuilder.Services.AddSingleton(emailConfig);




            //webApplicationBuilder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepositories<>));
            //webApplicationBuilder.Services.AddAutoMapper(M => M.AddProfile(profile: new MappingProfiler()));
            ////webApplicationBuilder.Services.AddAutoMapper(typeof(MappingProfiler));
            //webApplicationBuilder.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = (actionContext) =>
            //    {
            //        var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
            //                                 .SelectMany(P => P.Value.Errors)
            //                                 .Select(E => E.ErrorMessage).ToArray();
            //        var validationErrorResponse = new ApiValidationErrorResponse()
            //        {
            //            Errors = errors
            //        };
            //        return new BadRequestObjectResult(validationErrorResponse);
            //    };
            //});
            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((ServiceProvider) =>
            {
                var connection = webApplicationBuilder.Configuration.GetConnectionString(name: "Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            webApplicationBuilder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            webApplicationBuilder.Services.AddApplicationServices();
            webApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                //options.Password.RequireDigit = true;
                //options.Password.RequiredUniqueChars = 2;
                //options.Password.RequireNonAlphanumeric = true;

            }).AddEntityFrameworkStores<AppIdentityDbContext>();
            webApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = webApplicationBuilder.Configuration[key: "JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = webApplicationBuilder.Configuration[key: "JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(webApplicationBuilder.Configuration[key: "JWT:Key"]))
                };
            }
            
            );

            webApplicationBuilder.Services.AddScoped<ITokenService, TokenService>();
            webApplicationBuilder.Services.AddCors(opts =>
            {

                opts.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyMethod();
                    config.WithOrigins(webApplicationBuilder.Configuration["FrontEndBaseUrl"]);
                });

            });

            #endregion

            var app = webApplicationBuilder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _context = services.GetRequiredService<StoreDbContext>();
            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            //Ask CLR to Create Object From StoreDbContext Explicitly

            var LoggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                //_context.Database.MigrateAsync(); 
                await _context.Database.MigrateAsync(); //Update Database(business)
                await StoreDbContextSeed.SeedAsync(_context); //Data Seeding
                await _IdentityDbContext.Database.MigrateAsync();  //Update Database (identity) 
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbcontextSeed.SeedUsersAsync(_userManager);
            }
            catch (Exception ex)
            {
                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, message: "An Error has been occured while applying Migrations");

                // await Console.Out.WriteLineAsync();
            }            // Configure the HTTP request pipeline.

            #region Configure
            app.UseMiddleware<ExceptionMiddleWare>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute(pathFormat: "/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.UseCors("MyPolicy");

            #endregion
            app.Run();
        }
    }
}
