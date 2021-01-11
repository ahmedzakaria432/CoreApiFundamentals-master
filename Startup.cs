using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using CoreCodeCamp.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

namespace CoreCodeCamp
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services,IConfiguration configuration)
    {
      services.AddDbContext<CampContext>();
      services.AddScoped<ICampRepository, CampRepository>();
      services.AddAutoMapper();
            services.AddApiVersioning(opt => {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 1);
                
                opt.ReportApiVersions = true;
                //this is to read version from url
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                //this is for support both header and query string versioning  
                // opt.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("Ver"),
                //new HeaderApiVersionReader("X-Version")
                // );
                // new QueryStringApiVersionReader("Ver");
                // opt.ApiVersionReader = new HeaderApiVersionReader("X-version");
               
                /////////////////////////////////////////////////////
                ///this is to map controllers to specefic versions and map actions to versions
                ///we use this instead of attributes that we used in controllers 
                opt.Conventions.Controller<TalksController>().HasApiVersion(new
                     ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(1, 1)).Action(c => c.Delete(default(string),
                      default(int))).MapToApiVersion(1,1);
            }) ;
      services.AddMvc(opt=>opt.EnableEndpointRouting=false)
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
          

            app.UseMvc();
            
    }
  }
}
