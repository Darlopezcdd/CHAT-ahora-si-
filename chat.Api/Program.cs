using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chat.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar DbContext
            builder.Services.AddDbContext<dbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("dbContext") ?? throw new InvalidOperationException("Connection string 'ChatDbContext' not found.")));

            // Configurar CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            // Agregar servicios al contenedor
            builder.Services.AddControllers()
              .AddJsonOptions(options =>
              {
                  options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
              });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configuración del pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Activar CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
