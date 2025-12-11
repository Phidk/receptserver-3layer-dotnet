# Src layout
- `Domain` - entiteter, værdityper, domæneregler.
- `Application` - use cases/services, DTO'er, interfaces mod persistence.
- `Infrastructure` - EF Core DbContext, repositories, migrations, seeding, integration til API.
- `Api` - web API til lægehuse og apoteker.
- `Web.Mvc` - MVC UI for apoteker.
- `Client.Wpf` - WPF UI for lægehuse.
