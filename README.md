# Automatic REST api library for EF entities in .Net Core

This library automatically generates RESTful API for DbSets in DbContext.  This is very much work in progress right now so feel free to submit issues.

[![Build status](https://ci.appveyor.com/api/projects/status/nuls4kut9jv1wjsn/branch/master?svg=true)](https://ci.appveyor.com/project/tdudek1/autoapi/branch/master)

### Nuget

https://www.nuget.org/packages/Auto.Rest.API/


### Getting Started

Create controller deriving from AutoAPIController for all entities (route must end with {*query} wild card)

```c#
[Route("/api/data/{*query}")]
public class DataController : AutoAPI.AutoAPIController
{
	public DataController(DataContext context) 
			: base(context)
	{

	}
}
```

If you want to use policy authorization derive like this

```c#
[Route("/api/authdata/{*query}")]
public class AuthorizedDataController : AutoAPI.AutoAPIController
{
    public AuthorizedDataController(DataContext context, IAuthorizationService authorizationService) 
			: base(context, authorizationService)
    {

    }
}

```

Annotate Data Context (use Policy properties for authorization per HTTP verb)


```c#
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    
    [AutoAPIEntity(Route = "authors", POSTPolicy = "IsAdmin")]
    public DbSet<Author> Authors { get; set; }
    [AutoAPIEntity(Route = "Books")]
    public DbSet<Book> Books { get; set; }
}
```

Register in ConfigureServices
```c#
services.AddDbContext<DataContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Data")));
services.AddAutoAPI<DataContext>();
```

Add AutoApi routes to swagger document with DocumentFilter using **Swashbuckle.AspNetCore** (https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
```c#

services.AddSwaggerGen(c =>
{
    c.DocumentFilter<AutoAPISwaggerDocumentFilter>(new List<string> { "/api/data/", "/api/authdata/" });
});

```


Access at

```
Read all		GET /api/data/authors 
Read by id		GET /api/data/authors/1 
Filter/Sort/Paging	GET /api/data/authors?filter[Name][like]=J.R.R.Tolkien&sort[Id]=desc&pageSize=10&page=2
Create			POST /api/data/authors
Update			PUT /api/data/authors/1
Delete			DELETE /api/data/authors/1
```

More filters

You can specify comparison operators in query string like this

````
?filter[propertyName][operator] = value
````

Supported operators are 

 - String Properties 
   - eq (Equal) neq (Not Equal) like (Like) nlike (Not Like)
 - Guid Properties 
   - eq (Equal) neq (Not Equal)
 - Value Type Properties
	- eq (Equal) neq (Not Equal) gt (Greater Than) lt (Less than) gteq (Greater Than or Equal) lteq (Less Than or Equal) 

By default multiple filters are joined with an AND operator to use OR use ?operator=or 

#### To Dos
- Convert to use Middleware (this will break compatibility)
- Include related entities in results


#### License

https://opensource.org/licenses/GPL-3.0
