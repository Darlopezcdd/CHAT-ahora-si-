using Chat.Mvc.Proxies;

namespace Chat.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient<IChatApiProxy, ChatApiProxy>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7230/api/");
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Mensajes}/{action=Index}/{id?}");
            });

            app.UseAuthorization();

            app.Run();
        }
    }
}
