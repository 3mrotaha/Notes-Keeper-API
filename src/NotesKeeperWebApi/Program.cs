using Serilog;
using NotesKeeperWebApi.Middleware;
using NotesKeeperWebApi.Configuration;
using DinkToPdf;
using DinkToPdf.Contracts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAndConfigureControllers();

// api versioning
builder.Services.AddAndConfigureApiVersioning();

// serilog
builder.Host.AddAndConfigureLoggers();

// swagger configuration 
builder.Services.AddAndConfigureSwagger();

// adding cors
builder.Services.AddAndConfigureCors(builder.Configuration);


// configure dbcontext and Identity
builder.Services.AddDbContextAndIdentity(builder.Configuration);


// add auth and authZ
builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

// add rate limiting
builder.Services.AddAndConfigureRateLimiters();

// add application services
builder.Services.AddApplicationServices();
builder.Services.AddBackgroundJobs();

// html to pdf converter service
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

var app = builder.Build();

// configure web app -> middlewares, serilog, swagger, development env.. etc
app.AddAppMiddlewares();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseCors("AllowAngularLocalhost"); // called before UseAuthorization/UseAuthentication

app.UseAuthentication();
app.UseAuthorization();


/**
 * endpoint routing for the controllers, this will map the incoming HTTP requests to the corresponding controller actions based on the route templates defined in the controllers
 */
app.MapControllers();


app.Run();

public partial class Program { }
