using LexincorpApp.Models.ExternalServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public static class SeedData
    {
        public static void EnsurePropulated(IApplicationBuilder app)
        {
            try
            {
                ApplicationDbContext context = app.ApplicationServices
               .GetRequiredService<ApplicationDbContext>();
                var crypto = app.ApplicationServices.GetRequiredService<ICryptoManager>();
                var guid = app.ApplicationServices.GetRequiredService<IGuidManager>();
                //context.Database.Migrate();
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
                if (!context.Departments.Any())
                {
                    context.Departments.AddRange(
                        new Department[]
                        {
                        new Department{Name="Corporativo"},
                        new Department{Name="Propiedad intelectual"},
                        new Department{Name="Contabilidad"},
                        new Department{Name="Litigio"},
                        new Department{Name="Fiscal"},
                        new Department{Name="Sector regulado"}
                        }
                    );
                    context.SaveChanges();
                }

                if (!context.Retainers.Any())
                {
                    context.Retainers.AddRange(
                        new Retainer[]
                        {
                        new Retainer{SpanishName="Retainer Representación Legal"},
                        new Retainer{SpanishName="Retainer Asesoría Legal"},
                        new Retainer{SpanishName="Retainer Recursos Humanos"},
                        new Retainer{SpanishName="Retainer Contabilidad"}
                        }
                    );
                    context.SaveChanges();
                }
                if (!context.Users.Any(
                    ))
                {
                    string generatedGuid = guid.GenerateGuid();
                    string generatedPassword = generatedGuid.Substring(generatedGuid.Length - 12, 12);
                    //To hardcode the default admin password as a default one 
                    generatedPassword = "LexincorpNic2018";
                    string cryptoPassword = crypto.HashString(generatedPassword);
                    User adminUser = new User
                    {
                        IsAdmin = true,
                        Username = "webAdmin",
                        Password = cryptoPassword,
                        Attorney = new Attorney
                        {
                            Address = "-",
                            AdmissionDate = DateTime.Now,
                            AssignedPhoneNumber = "-",
                            DepartmentId = 1,
                            Email = "-",
                            EmergencyContact = "-",
                            EmergencyContactPhoneNumber = "-",
                            IdentificationNumber = "----",
                            Name = "Administrador",
                            NotaryCode = "-",
                            PersonalPhoneNumber = "-",
                            VacationCount = 0
                        }
                    };
                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Ocurrió un error al inicializar la base de datos de la aplicación");
                //Log.CloseAndFlush();
            }
           
        }
    }
}
