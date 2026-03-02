using foundWhatYouLost.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using foundWhatYouLost.Services;
using foundWhatYouLost.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=fwyl_db.db"));

builder.Services.AddDefaultIdentity<User>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]!)
            )
        };
    });

builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
_db.Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/users", async () => await _db.Users.ToListAsync());

app.MapGet("/users/{id}", GetUserById);
async Task<Results<Ok<User>, NotFound>> GetUserById(Guid id)
{
    var user = await _db.Users.FindAsync(id);

    return user is not null
        ? TypedResults.Ok(user)
        : TypedResults.NotFound();
};

app.MapPost("/users/register", CreateUser);
async Task<Results<Created<UserRegistrationResponse>, BadRequest<UserRegistrationResponse>>> CreateUser(IUserRegistrationService _userServices, UserRegistrationRequest userRegistration)
{
    var result = await _userServices.RegisterUser(userRegistration);

    if (!result.isSuccessful)
    {
        return TypedResults.BadRequest(new UserRegistrationResponse(false, result.Errors));
    }

    return TypedResults.Created("/user", result);
};

app.MapPost("/users/login", LoginUser);
async Task<Results<Ok<UserLoginResponse>, BadRequest<UserLoginResponse>>> LoginUser(IUserLoginService _loginService, UserLoginRequest request)
{
    var result = await _loginService.LoginUser(request);

    if (!result.IsSuccessful)
    {
        return TypedResults.BadRequest(new UserLoginResponse(false, result.Errors));
    }

    return TypedResults.Ok(result);
}

app.MapPut("/users/{id}", UpdateUser);
async Task<Results<Ok<User>, NotFound, NoContent>> UpdateUser(string id, User user)
{
    var findUser = await _db.Users.FindAsync(id);

    if (findUser == null)
    {
        return TypedResults.NoContent();
    }

    _db.Entry(findUser).State = EntityState.Detached;

    var updateUser = new User(
        findUser.Id,
        user.Name = findUser.Name ?? string.Empty,
        user.UserName ?? findUser.UserName ?? string.Empty,
        user.Email ?? findUser.Email ?? string.Empty,
        user.PhoneNumber ?? findUser.PhoneNumber ?? string.Empty,
        user.ItemId ?? findUser.ItemId
    );

    _db.Users.Update(updateUser);
    await _db.SaveChangesAsync();

    return updateUser is not null
     ? TypedResults.Ok(updateUser)
     : TypedResults.NotFound();
};

app.MapGet("/items", async () => await _db.Items.ToListAsync());

app.MapGet("/items/{id:guid}", GetItemById);
async Task<Results<Ok<Item>, NotFound, NoContent>> GetItemById(Guid id)
{
    var singleItem = await _db.Items.FindAsync(id);

    if(singleItem == null)
    {
        return TypedResults.NoContent();
    }

    return singleItem is not null
        ? TypedResults.Ok(singleItem)
        : TypedResults.NotFound();
};

app.MapPost("/items", CreateItem);
async Task<Results<Created<Item>, NotFound>> CreateItem(Item item)
{
    var newItem = new Item(
        Guid.CreateVersion7(),
        item.Latitude,
        item.Longitude,
        item.City,
        item.Street,
        item.Number,
        item.Photo,
        item.Title,
        item.Description,
        item.UserId
    );

    _db.Items.Add(newItem);
    await _db.SaveChangesAsync();
    return newItem is not null
        ? TypedResults.Created("/items", newItem)
        : TypedResults.NotFound();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();
