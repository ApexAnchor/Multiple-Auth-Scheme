using Jwt.Token.Generator.Data;
using Jwt.Token.Generator.Extensions;
using Jwt.Token.Generator.Options;
using Jwt.Token.Generator.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite("DataSource = appdb");
    });
    
    builder.Services.AddTransient<IJwtTokenProvider, JwtTokenProvider>();

    builder.Services.ConfigureOptions<JwtOptionsSetup>();

    builder.Services.AddAuthentication(builder.Configuration);

}


var app = builder.Build();

{
    if (app.Environment.IsDevelopment())
    { 
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

