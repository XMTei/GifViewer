using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GifViewer
{
    public class Startup
    {
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
        {
			//services.AddSingleton<IFileProvider>(
			//	new PhysicalFileProvider(
			//		Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

			//services.AddMvc();
			//services.Configure<FormOptions>(x =>
			//{
			//	x.ValueLengthLimit = int.MaxValue;
			//	x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
			//});
			services.AddControllers();
			//services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			//指定相应的html
			DefaultFilesOptions DefaultFile = new DefaultFilesOptions();
			DefaultFile.DefaultFileNames.Clear();
#if DEBUG
			DefaultFile.DefaultFileNames.Add("index.html");
#else
			DefaultFile.DefaultFileNames.Add("index.min.html");
#endif
			app.UseDefaultFiles(DefaultFile);
			//无指定时显示wwwroot/default.html or index.html
			//app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.MapHub<ChatHub>("/chatHub");
			});
		}
	}
}
