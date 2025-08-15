| Aula| SQL | API / MVC | auth | Monitoring |    Synch       | Infra     | Caching     |
| ---| --- | --- | --- | --- |    ---       | ---     | ---     |
| 1 | local | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 2 | CLOUD | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 3 | CLOUD | CLOUD |     -    |      -     |    s�ncrono    | az portal |     -       |
| 4 | CLOUD | CLOUD | MS ENTRA |      -     |    s�ncrono    | az portal |     -       |
| 5 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR |    s�ncrono    | az portal |     -       |
| 6 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | az portal |     -       |
| 7 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      |     -       |
| 8 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      | AZURE REDIS |

### Gera��o do banco de dados local

```bash
cd VollMed.WebAPI
dotnet ef database update
```

### appsettings.Development.json

VollMed.WebAPI

appsettings.Development.json

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "VollMedDB": "Server=tcp:vollmed20250808.database.windows.net,1433;Initial Catalog=VollMedDB;Persist Security Info=False;User ID=vollmed;Password=!v0llmed;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

VollMed.Web

appsettings.Development.json

```bash
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "VollMed_WebApi": {
    "Name": "VollMed.WebApi",
    "BaseAddress": "https://localhost:6001"
  }
}
```