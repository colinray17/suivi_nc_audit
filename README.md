# Suivi des non-conformités d'audit

Application full-stack de gestion des non-conformités relevées lors d'audits qualité, développée dans le cadre d'un portfolio de transition vers un poste de développeur .NET/Angular.

## Stack technique

- **Backend** : .NET 10, ASP.NET Core Web API, Entity Framework Core, SQLite
- **Frontend** : Angular 22 (standalone components, signals)

## Fonctionnalités

- Gestion des audits et de leurs non-conformités liées (relation one-to-many)
- Filtrage des non-conformités par statut (Ouverte / En cours / Clôturée)
- Tri par gravité et par date d'échéance
- Mise à jour du statut directement depuis le tableau

## Lancer le projet en local

**Backend**

```bash
cd SuiviNonConformites.Api
dotnet run --launch-profile http
```

API disponible sur `http://localhost:5135`, Swagger sur `/swagger`.

**Frontend**

```bash
cd suivi-non-conformites-app
ng serve
```

Application disponible sur `http://localhost:4200`.
