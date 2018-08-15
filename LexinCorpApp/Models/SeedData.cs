﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexinCorpApp.Models
{
    public static class SeedData
    {
        public static void EnsurePropulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices
                .GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
           if (!context.ClientTypes.Any())
           {
                context.ClientTypes.AddRange(new ClientType[] 
                {
                    new ClientType { Name = "Jurídicos"},
                    new ClientType { Name = "Despachos"},
                    new ClientType { Name = "Naturales"}
                });
                context.SaveChanges();
           }
           if (!context.BillingModes.Any())
            {
                context.BillingModes.AddRange(
                    new BillingMode[]
                    {
                        new BillingMode {Name = "ND-IVA"},
                        new BillingMode {Name = "ND"},
                        new BillingMode {Name = "Detalle de factura"},
                    }
                );
                context.SaveChanges();
            }
           if (!context.DocumentDeliveryMethods.Any())
            {
                context.DocumentDeliveryMethods.AddRange(
                    new DocumentDeliveryMethod[]
                    {
                        new DocumentDeliveryMethod {Name="Email"},
                        new DocumentDeliveryMethod {Name="Correo aéreo"},
                        new DocumentDeliveryMethod {Name="Fax"},
                        new DocumentDeliveryMethod {Name="Oficina"},
                    }
                );
                context.SaveChanges();
            }
            
        }
    }
}
