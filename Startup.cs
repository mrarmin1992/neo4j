using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Neo4jClient;

namespace HR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Ovaj metod se koristi za konfiguraciju servisa koji će biti dostupni unutar aplikacije.
        public void ConfigureServices(IServiceCollection services)
        {
            // Metod koji dodaje MVC servis u aplikaciju, omogućujući korištenje kontrolera za upravljanje HTTP zahtjevima.
            services.AddControllers();
            // Metod koji dodaje Swagger dokumentaciju za generiranje API dokumentacije.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR", Version = "v1" });
            });
            // Inicijalizacija BoltGraphClient objekta za komunikaciju s Neo4j bazom podataka.
            var client = new BoltGraphClient(new Uri("bolt://localhost:7687"),"neo4j","oblivion");
            client.ConnectAsync();
            services.AddSingleton<IGraphClient>(client);
        }

        // Ovaj metod konfigurira HTTP zahtjevni pipeline. U ovom slučaju, dodana je podrška za Swagger UI i zahtjeve za HTTPS.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Dodaje stranicu za prikazivanje detalja o greškama prilikom razvoja.
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR v1"));
            }
            // Automatsko preusmjeravanje HTTP zahtjeva na HTTPS.
            app.UseHttpsRedirection();
            // Omogućuje rutiranje HTTP zahtjeva prema odgovarajućim endpointima.
            app.UseRouting();
            // Omogućuje korištenje autorizacije u aplikaciji.
            app.UseAuthorization();
            // Konfigurira završne točke (endpoints) za obradu HTTP zahtjeva.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
