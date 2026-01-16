using Domain.Contracts;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data;
using Persistance.Repositories;
using Shared;
using StackExchange.Redis;
using System.Text;

namespace Taleb_Discount.Extentions
{
    public static class InfrasturctureServicesExtentions
    {
        public static IServiceCollection AddInfrasturctureServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IFeedBackRepository, FeedBackRepository>();
            Services.AddScoped<IVendorRepository, VendorRepository>();
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<ITransactionRepository, TransactionRepository>();
            Services.AddScoped<IUserRepository, UserRepository>();


            Services.AddSingleton<IConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis")));

            Services.ConfigureJWT(Configuration);

            return Services;
        }
        public static IServiceCollection ConfigureJWT(this IServiceCollection Services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            Services.AddAuthentication(options =>
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
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });
            Services.AddAuthorization();
            return Services;
        }
    }
}