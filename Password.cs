using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;

namespace Electro
{
    //Extension Method Password Settings
    public static class Password
    {
        public static void PasswordSetting(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
          {
              options.Password.RequireDigit = true;
              options.Password.RequireNonAlphanumeric = false;
              options.Password.RequireUppercase = false;
              options.Password.RequireLowercase = false;
              //Time user is locked out after fail login attempts
              options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
              options.Lockout.MaxFailedAccessAttempts = 5;
              options.Lockout.AllowedForNewUsers = true;
          });
           //Removes Identity Part from the URL
            services.AddMvc().AddRazorPagesOptions(o => o.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/", model =>
     {
         foreach (var selector in model.Selectors)
         {
             var attributeRouteModel = selector.AttributeRouteModel;
             attributeRouteModel.Order = -1;
             attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
         }
     })
      );



        }
    }
}