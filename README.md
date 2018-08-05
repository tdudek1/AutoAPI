# Automatic REST api library for EF entities in .Net Core

This library automatically generates RESTful API for DbSets in DbContext.  This is very much work in progress right now so feel free to submit issues.

[![Build status](https://ci.appveyor.com/api/projects/status/nuls4kut9jv1wjsn/branch/master?svg=true)](https://ci.appveyor.com/project/tdudek1/autoapi/branch/master)


### Getting Started

Create controller deriving from AutoAPIController for all entities

```c#
[Route("/api/data/{*query}")]
public class DataController : AutoAPI.AutoAPIController
{
	public DataController(DbContext context) : base(context)
	{

	}
}
```

Annotate Data Context
```c#
[AutoAPIEntity(Route = "authors")]
public DbSet<Author> Authors { get; set; }
```

Register in ConfigureServices
```c#
services.AddTransient<DbContext>(x =>
{
	return new DataContext(new DbContextOptionsBuilder<DataContext>()
              .UseSqlServer(Configuration.GetConnectionString("DataContext"));
});
services.AddAutoAPI<DataContext>();
```

Access at

```
Read all		GET /api/data/authors 
Read by id		GET /api/data/authors/1 
Filter/Sort/Paging	GET /api/data/authors?filter[Name]=J.R.R.Tolkien&sort[Id]=desc&pageSize=10&page=2
Create			POST /api/data/authors
Update			PUT /api/data/authors/1
Delete			DELETE /api/data/authors/1
```

#### To Dos

- Authentication
- Filtering (operators and expresssions)
- Include related entities in results
- Improve routing/registration

#### License

https://opensource.org/licenses/GPL-3.0
