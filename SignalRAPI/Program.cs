using SignalRAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddHostedService<ServerTimeNotifier>();

builder.Services.AddCors();
// var _defaultCorsPolicyName = "localhost";
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(_defaultCorsPolicyName,
//         policy =>
//         {
//             policy.WithOrigins("https://localhost:44324")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod().AllowCredentials();
//         });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.MapHub<NotificationHub>("notifications");

app.Run();


