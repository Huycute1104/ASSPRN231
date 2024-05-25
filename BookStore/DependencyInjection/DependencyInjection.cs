using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.UnitOfwork;

namespace BookStore.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<BStoreDBContext>(options => options.UseSqlServer(getConnection()));
            return services;
        }
        public static IServiceCollection addUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfwork, UnitOfwork>();
            return services;
        }



        public static string getConnection()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var str = config["ConnectionStrings:MyDB"];
            return str;
        }
    }
}
