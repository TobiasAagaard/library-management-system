# Library Management System


### Aspire

The AppHost declares `db.AddDatabase("lmsdb")` and references that database from the desktop project, so Aspire supplies `ConnectionStrings__lmsdb` automatically when it starts the app:

```bash
dotnet run --project AppHost
```


