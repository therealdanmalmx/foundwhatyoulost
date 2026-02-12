using foundWhatYouLost.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=fwyl_db.db"));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

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

app.MapGet("/users/{id:Guid}", GetUserById);
async Task<Results<Ok<User>, NotFound>> GetUserById(Guid id)
{
    var user = await _db.Users.FindAsync(id);

    return user is not null
        ? TypedResults.Ok(user)
        : TypedResults.NotFound();
};

app.MapPost("/users",  CreateUser);
async Task<Results<Created<User>, NotFound>> CreateUser(User user)
{
    var newUser = new User(
        Guid.CreateVersion7(),
        user.Name,
        user.Email,
        user.Phone,
        user.ItemId
    );

    _db.Users.Add(newUser);
    await _db.SaveChangesAsync();

    return newUser is not null
        ? TypedResults.Created("/users/", newUser)
        : TypedResults.NotFound();
};

app.MapPut("/users/{id:guid}", UpdateUser);
async Task<Results<Ok<User>, NotFound, NoContent>> UpdateUser(Guid id, User user)
{
    var findUser = await _db.Users.FindAsync(id);

    if (findUser == null)
    {
        return TypedResults.NoContent();
    }

    _db.Entry(findUser).State = EntityState.Detached;

    var updateUser = new User(
        findUser.Id,
        user.Name ?? findUser.Name,
        user.Email ?? findUser.Email,
        user.Phone ?? findUser.Phone,
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
