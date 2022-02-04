using DoiLibrary.Services;
using DoiLibrary.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Doi_WebApi
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
			AddDoiHttpClientToService(Configuration, services);

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Doi_WebApi", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Doi_WebApi v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		static public void AddDoiHttpClientToService(IConfiguration config, IServiceCollection services)
		{
			var username = config["DoiSettings:DoiApiUser"];
			var pwd = config["DoiSettings:DoiApiPwd"];

			//We are using 'DoiHttpClient' which is a typed HttpClient (https://www.stevejgordon.co.uk/httpclientfactory-named-typed-clients-aspnetcore).
			//See below article in case of problems eg. DNS, sockets exhaustion of HttpMessageHandler
			//https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
			services.AddHttpClient<IDoiHttpClient, DoiHttpClient>(client =>
			{
				client.BaseAddress = new Uri(config["DoiSettings:DoiDataciteServerUrl"]);
				client.DefaultRequestHeaders.Add("Accept", "application/vnd.api+json");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
						$"{username}:{pwd}")));
			});
		}
	}
}
