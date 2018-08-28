# Automatic REST api library for EF entities in .Net Core

This library automatically generates RESTful API for DbSets in DbContext.  This is very much work in progress right now so feel free to submit issues.

[![Build status](https://ci.appveyor.com/api/projects/status/nuls4kut9jv1wjsn/branch/master?svg=true)](https://ci.appveyor.com/project/tdudek1/autoapi/branch/master)


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

Annotate Data Context
```c#
[AutoAPIEntity(Route = "authors")]
public DbSet<Author> Authors { get; set; }
```


Annotate Data Context with policy authorization
```c#
[AutoAPIEntity(Route = "authors", POSTPolicy = "IsAdmin")]
public DbSet<Author> Authors { get; set; }
```

Register in ConfigureServices
```c#
services.AddDbContext<DataContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Data")));
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

- Logging
- Include related entities in results
- Improve routing/registration

#### License

https://opensource.org/licenses/GPL-3.0
