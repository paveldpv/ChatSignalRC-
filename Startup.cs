using ChatService.Hubs;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;


namespace ChatService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {

                    builder.WithOrigins("http://localhost:3000", "http://192.168.31.173:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()                    
                    .AllowCredentials();
                });
            });
            services.AddSingleton<IDictionary<string, UserConnection>>(opt => new Dictionary<string, UserConnection>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
