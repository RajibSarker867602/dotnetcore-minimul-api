using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// adding db context
builder.Services.AddDbContext<ToDoAppDbContext>(options => options.UseInMemoryDatabase("ToDoAppDb"));

var app = builder.Build();

// app.MapGet("/", () => "Hello World!");

// get all the todo items
app.MapGet("/todoitems", async (ToDoAppDbContext db) => await db.ToDoItems.AsNoTracking().ToListAsync());

// get todo by id
app.MapGet("/todoitems/{id}", async (long id, ToDoAppDbContext db) => await db.ToDoItems.FirstOrDefaultAsync(c => c.Id == id));

// get completed todo items
app.MapGet("/todoitems/complete", async (ToDoAppDbContext db) => await db.ToDoItems.Where(c => c.IsCompleted == true).AsNoTracking().ToListAsync());

// create new items
app.MapPost("/todoitems", async (ToDoItem inputRequest, ToDoAppDbContext db) =>
{
    if (inputRequest is null) return Results.BadRequest("Inavalid input request!");

    await db.ToDoItems.AddAsync(inputRequest);
    return await db.SaveChangesAsync() > 0 ? Results.Created($"/todoitems/{inputRequest.Id}", inputRequest) : Results.BadRequest("Todo Item Create Operation Failed!");
});

// update todo item
app.MapPut("/todoitems/{id}", async (long id, ToDoItem inputRequest, ToDoAppDbContext db) =>
{
    if (inputRequest is null) return Results.BadRequest("Inavalid input request!");

    var existingItem = await db.ToDoItems.FirstOrDefaultAsync(c => c.Id == id);
    if (existingItem is null) return Results.BadRequest($"No items found by id {id}.");

    existingItem.Name = inputRequest.Name;
    existingItem.IsCompleted = inputRequest.IsCompleted;

    return await db.SaveChangesAsync() > 0 ? Results.Created($"/todoitems/{inputRequest.Id}", inputRequest) : Results.BadRequest("Todo Item Update Operation Failed!");
});

// delete item
app.MapDelete("/todoitems/{id}", async (long id, ToDoAppDbContext db) =>
{
    var existingItem = await db.ToDoItems.FirstOrDefaultAsync(c => c.Id == id);
    db.Remove(existingItem);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
