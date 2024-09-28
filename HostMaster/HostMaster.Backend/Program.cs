using HostMaster.Backend.Data;
using HostMaster.Backend.Repositories.Implementations;
using HostMaster.Backend.UnitsOfWork.Implementations;
using HostMaster.Backend.UnitsOfWork.Interfaces;
using HostMaster.Backend.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;
using HostMaster.Backend.Helpers;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddTransient<SeedDb>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();

builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();

builder.Services.AddScoped<IRoomsRepository, RoomsRepository>();
builder.Services.AddScoped<IRoomsUnitOfWork, RoomsUnitOfWork>();

builder.Services.AddScoped<IRoomPhotosRepository, RoomPhotosRepository>();
builder.Services.AddScoped<IRoomPhotosUnitOfWork, RoomPhotosUnitOfWork>();

builder.Services.AddScoped<IRoomTypesRepository, RoomTypesRepository>();
builder.Services.AddScoped<IRoomTypesUnitOfWork, RoomTypesUnitOfWork>();

builder.Services.AddScoped<IAccommodationsRepository, AccommodationsRepository>();

var app = builder.Build();

SeedData(app);
void SeedData(WebApplication app)
{
    var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopeFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();