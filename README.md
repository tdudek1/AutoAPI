# Automatic REST api for EF entities in .Net Core

This library automaticlly generates RESTful api for DbSets in DbContext.

## Getting Started

Create controller deriving from AutoAPIController

```c#
[Route("/api/data/{*query}")]
public class DataController : AutoAPI.AutoAPIController
{
	public DataController(DbContext context) : base(context)
	{

	}
}
```

Anontate Data Contexxt
```c#
[AutoAPIEntity(Route = "authors")]
public DbSet<Author> Authors { get; set; }
```

Register in ConfigureServices
```c#
services.AddTransient<DbContext>(x =>
{
	return new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlServer(Configuration.GetConnectionString("DataContext"));
});
services.AddAutoAPI<DataContext>();
```

Access at

```
Read all        GET /authors 
Read by id      GET /authors/1 
Create          POST /authors
Update          PUT /authors
Delete          DELETE /authors/1
```

### License

https://opensource.org/licenses/GPL-3.0