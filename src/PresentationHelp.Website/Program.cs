using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Microsoft.AspNetCore.Mvc.Formatters;
using PresentationHelp.Website.CompositionRoot;
using PresentationHelp.Website.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new MelvilleServiceProviderFactory(true, IocConfiguration.Configure));
new AspNetServiceRegistration(builder.Services).Configure();

var app = builder.Build();

// Configure the HTTP request pipeline.
new SetupWebPipeline(app).Configure();

app.Run();
