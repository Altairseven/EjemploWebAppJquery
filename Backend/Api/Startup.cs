using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Api.Configuration;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Newtonsoft.Json.Serialization;

namespace Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        internal class CustomAssemblyLoadContext : AssemblyLoadContext {
            public IntPtr LoadUnmanagedLibrary(string absolutePath) {
                return LoadUnmanagedDll(absolutePath);
            }

            protected override IntPtr LoadUnmanagedDll(String unmanagedDllName) {
                return LoadUnmanagedDllFromPath(unmanagedDllName);
            }

            protected override Assembly Load(AssemblyName assemblyName) {
                throw new NotImplementedException();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //Extrae El string de Conexion de appsettings.json, y se lo pone al modelo
            EmpresaDbContext.ConnectionString = Configuration.GetConnectionString("EmpresaDB");
            //tambien lo guarda en ContextClass.. por si se necesita para algo mas.. Como el Dapper.
            ContextClass.ConnectionString = EmpresaDbContext.ConnectionString;

            //Configura la Validacion de Tokens para los request.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "Empresa/api",
                        ValidAudience = "EmpresaApp",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("frase secreta para generar un array de bytes unico"))
                    };
                });



            //Configura el CORS, que es lo que permite que el api sea comunicado por otras aplicaciones. como el front end
            //sino. se usara.. solo podria ser utilizado por algo corriendo el la misma pc y con el msimo puerto.
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            //corsBuilder.WithOrigins("http://localhost:56573"); // for a specific url. Don't add a forward slash on the end!
            corsBuilder.AllowCredentials();
            corsBuilder.WithExposedHeaders("filename");

            services.Configure<RazorViewEngineOptions>(o => {
                // {2} is area, {1} is controller,{0} is the action    
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/Content/Templates/{0}" + RazorViewEngine.ViewExtension);
            });

            //se Encarga de que se respeten los nombres de las propiedades al conviertirse en jsons para los responses.
            //Si no estuviera, y mandaramos un objeto con la propiedad ID_Cliente, llegaria al frontend como iD_Cliente
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //continua con la configuracion del CORS
            services.AddCors(options => {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });
            services.Configure<MvcOptions>(options => {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            });

            string LibExtension = "";
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                LibExtension = ".so";
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                LibExtension = ".dylib";
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                LibExtension = ".dll";


            var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            var wkHtmlToPdfPath = Path.Combine(Directory.GetCurrentDirectory(), $"Libraries\\libwkhtmltox\\{architectureFolder}\\libwkhtmltox" + LibExtension);

            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(wkHtmlToPdfPath);

            // Add converter to DI
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        //Este es el Middleware... es decir.. todo lo que ocurre cada vez q hay una interaccion con el api..
        //si yo mando un request.. ademas de llegar a su controller. va a pasar por aca primero.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            //Muestra paginas de errores si se esta en ambiente de desarrollo, no deberian aparecer en produccion.
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            //Importartante para cuando se publique en produccion.. permite redirigir los headers cuando hay un reverse proxy como el del nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //aplica la configuracion del Cors que se setea arriba.
            app.UseCors("SiteCorsPolicy");


            //ContextClass Middleware : Guarda de los headers. el id de usuario.. o cualquier otra propiedad que se necesite mandar con todo request
            //si se estuviera trabajando con una aplicacion multi empresa y multi sucursal.. habria que agregar tambien propiedades, en le header.. y leerlas desde aca.

            app.Use(async (HttpContext Context, Func<Task> next) => {
                long _idUsuario;
                Int64.TryParse(Context.Request.Headers["idUsuario"].ToString(), out _idUsuario);
                ContextClass.idUsuario = _idUsuario;
                await next();

            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
