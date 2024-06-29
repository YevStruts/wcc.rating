using wcc.rating.data;
using wcc.rating.kernel.RequestHandlers;
using wcc.rating.api.Helpers;
using Microservices = wcc.rating.kernel.Models.Microservices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var environment = builder.Configuration.GetValue("Environment", "dev");

string microservicesSettingsPath = $"/{environment}/microservices";
var microservicesSettings = await AWSParameterStore.Instance().GetParametersByPathAsync(microservicesSettingsPath);
Microservices.Config mcsvcConfig = new Microservices.Config
{
    CoreUrl = microservicesSettings[$"{microservicesSettingsPath}/core-url"],
    RatingUrl = microservicesSettings[$"{microservicesSettingsPath}/rating-url"],
    WidgetUrl = microservicesSettings[$"{microservicesSettingsPath}/widget-url"]
};
builder.Services.AddSingleton<Microservices.Config>(mcsvcConfig);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRatingQuery).Assembly));
builder.Services.AddTransient<IDataRepository, DataRepository>();

var app = builder.Build();

string ravenDbUrl = $"/{environment}/wcc-rating/ravendb";
var ravenDbSettings = await AWSParameterStore.Instance().GetParameterAsync(ravenDbUrl);
DocumentStoreHolder.Init(ravenDbSettings);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// if (!app.Environment.IsDevelopment())
// {
//     app.UseHttpsRedirection();
// }

app.UseAuthorization();

app.MapControllers();

app.Run();
