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
app.MapGet("/users/{id:Guid}", async (Guid id) => {
    var user = await _db.Users.FindAsync(id);

    if(user == null)
    {
        return null;
    }

    return Results.Accepted("/uses", user);
});

app.MapPost("/users",  async (User user) =>
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
    return Results.Created("/users", newUser);
});

app.MapPut("/users/{id:guid}", async (Guid id, User user) =>
{
    var findUser = await _db.Users.FirstOrDefaultAsync(u=> u.Id == id);

    if (findUser == null)
    {
        return Results.BadRequest($"User with id: {id} coul not be found");
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

    return Results.Accepted("/users/{id}", updateUser);
});

app.MapGet("/items", async () => await _db.Items.ToListAsync());
app.MapGet("/items/{id:guid}", async (Guid id) =>
{
    var oneItem = await _db.Items.FindAsync(id);

    if(oneItem == null)
    {
        return null;
    }

    return Results.Accepted("/items/{id}", oneItem);

});

app.MapPost("/items", async (Item item) => {

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
    return Results.Created("/items", newItem);
});


app.UseAuthentication();
app.UseAuthorization();

app.Run();
