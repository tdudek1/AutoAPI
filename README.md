# Automatic REST api library for EF entities in .Net Core

This library automatically generates RESTful API for DbSets in DbContext.  This is very much work in progress right now so feel free to submit issues.

[![Build status](https://ci.appveyor.com/api/projects/status/ar22x62t99bdv09v/branch/master?svg=true)](https://ci.appveyor.com/project/tdudek1/autoapi/branch/master)

**3.2 update to dotnet 6**

 - update to dotnet 6 and ef 6
 - fixed exception when key was a guid
 - added support for [Keyless] entities

**3.1 Update to dotnet 5** 

 - switches to use System.Text.Json from Json.NET - Newtonsoft for serialization and this make cause breaking changes
 - switches swagger support to use Microsoft.OpenApi.Models;
 - added extension method to configure System.Text.Json serializer options

**Version 2 breaks compatilbity as it uses a middleware instead of a controller to handle requests**

### Nuget

https://www.nuget.org/packages/Auto.Rest.API/


### Getting Started

Configure Auto API Service 

In Startup.cs ConfigureServices (path indicates base bath for db context)

```c#
public void ConfigureServices(IServiceCollection services)
{
    ...
    //generic argument is DbContext
    services.AddAutoAPI<DataContext>(options =>
    {
        options.Path = "/api/data";
        //optional 
        options.JsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
    });

    //register db context
    services.AddDbContext<DataContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Data")));
}
```


Register Middleware

In Startup.cs Configure

```c# 
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseAutoAPI();
}
```


Annotate Data Context


```c#
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    
    [AutoAPIEntity(Route = "authors", POSTPolicy = "IsAdmin", Authorize = true)]
    public DbSet<Author> Authors { get; set; }
    [AutoAPIEntity(Route = "Books")]
    public DbSet<Book> Books { get; set; }
}
```

##### Access at

```
Read all                GET     /api/data/authors 
Read by id              GET     /api/data/authors/1 
Count                   GET     /api/data/authors/count
PagedResult             GET     /api/data/authors/pagedresult
Filter/Sort/Paging      GET     /api/data/authors?filter[Name][like]=J.R.R.Tolkien&sort[Id]=desc&pageSize=10&page=2
Create                  POST    /api/data/authors
Update                  PUT     /api/data/authors/1
Delete                  DELETE  /api/data/authors/1
```

##### Authentication and Authorization

To require user to be authenticated set Authorize property of the AutoAPIEntity attribute to true.

Policy based authorization can be confgured by setting policy name property for entity or per http verb.

```c#
    
[AutoAPIEntity(Route = "authors", POSTPolicy = "IsAdmin", Authorize = true, ExposePagedResult = true)]
public DbSet<Author> Authors { get; set; }
    
```



##### Swagger

Add AutoApi routes to swagger document with DocumentFilter using **Swashbuckle.AspNetCore** (https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

```c#

services.AddSwaggerGen(a=>
{
    a.DocumentFilter<AutoAPISwaggerDocumentFilter>();
});

```

##### More filters

You can specify comparison operators in query string like this

````
?filter[propertyName][operator] = value
````

Supported operators are 

 - String Properties 
   - eq (Equal) neq (Not Equal) like (Like) nlike (Not Like) in (In) nin (Not In)
 - Guid Properties 
   - eq (Equal) neq (Not Equal) in (In) nin (Not In)
 - Value Type Properties
	- eq (Equal) neq (Not Equal) gt (Greater Than) lt (Less than) gteq (Greater Than or Equal) lteq (Less Than or Equal) in (In) nin (Not In)

By default multiple filters are joined with an AND operator to use OR use ?operator=or 

Filters can also be used with the count endpoint

##### Paged Result

You can access paged result like this

````
/data/api/authors/pagedresult?page=1&pageSize=2
````

This will produce result like below that will include current page, page size, number of pages and total items.

```json
{
  "items": [
    {
      "id": 1,
      "name": "Ernest Hemingway",
      "books": null,
      "dateOfBirth": "1899-07-21T00:00:00"
    },
    {
      "id": 2,
      "name": "Stephen King",
      "books": null,
      "dateOfBirth": "1947-09-21T00:00:00"
    }
  ],
  "page": 1,
  "pageCount": 1,
  "pageSize": 2,
  "total": 2
}
```



##### Include related entities

To include related entities you can use the include parameter

````
/data/api/authors?include=books
````

#### License

https://opensource.org/licenses/GPL-3.0
