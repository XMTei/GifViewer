﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

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
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton<IFileProvider>(
				new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
			
			services.AddMvc();
			services.Configure<FormOptions>(x =>
			{
				x.ValueLengthLimit = int.MaxValue;
				x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc();

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
		}
	}
}
