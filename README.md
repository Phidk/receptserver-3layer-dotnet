# Receptserver System - Eksamensopgave E2025

Systemet er bygget på en 3-lags arkitektur og anvender **Entity Framework Core** til persistens. Løsningen demonstrerer brugen af .NET 8, MVC, Web API, WPF, LINQ, Lambda-udtryk og Dependency Injection.

## Architecture & Design

Løsningen er opdelt i følgende lag for at sikre separation of concerns og testbarhed:

1.  **Domain (Core):**
    *   Indeholder forretningslogik og entities (`Prescription`, `MedicationOrder`, `Patient`, `Clinic`, `Pharmacy`).
    *   Ingen dependencies til database eller UI.
    *   Håndterer regler som "Kan kun udlevere hvis antal < tilladt" og "Luk recept hvis alle ordinationer er udleveret".

2.  **Application (Service):**
    *   Indeholder Use Cases (`PrescriptionService`) og DTO'er.
    *   Orkestrerer kald til repositories og validerer input (CPR-validering, Ydernummer-tjek).

3.  **Infrastructure (Data):**
    *   Implementerer persistens via **EF Core** og **SQLite**.
    *   Indeholder `ReceptDbContext`, Repositories (`PrescriptionRepository`, `ClinicRepository`) og database-migrations.
    *   Seeder databasen med testdata ved opstart (hvis databasen ikke findes).

4.  **Presentation (UI):**
    *   **Api:** REST API, der udstiller funktionalitet til eksterne systemer (bruges af Lægehus-klienten).
    *   **Web.Mvc:** Webapplikation til **Apoteker**. Bruger HTML Helpers, ViewModels og Razor.
    *   **Client.Wpf:** Desktop-applikation til **Lægehuse**. Kommunikerer med systemet via REST API'et.

## Prerequisites

*   .NET 8 SDK installeret.
*   Windows OS (krævet for at køre WPF-klienten).

## Folder Structure

*   `src/`
    *   `Domain`: Entities og logik.
    *   `Application`: Services og interfaces.
    *   `Infrastructure`: EF Core kontekst og repositories.
    *   `Api`: Web API endpoint.
    *   `Web.Mvc`: Apoteks-frontend.
    *   `Client.Wpf`: Lægehus-klient.
*   `tests/`
    *   `Domain.Tests`: xUnit tests (15 stk) af domæne- og servicelogik (InMemory database).
*   `docs/`
    *   `domain-class-diagram.pdf`: Klassediagram over domænet.

---

## Run-and-Test Workflow

Systemet bruger en lokal SQLite-database (`recept.db`), som oprettes automatisk med testdata ved første kørsel.

**VIGTIGT: For at nulstille data til en "frisk" test:**
1.  Stop alle kørende applikationer.
2.  Slet filen `recept.db` i root af mappen.
3.  Genstart applikationerne (databasen genskabes automatisk).

### Start af applikationer

Det anbefales at køre API og MVC fra terminaler og WPF fra Visual Studio eller en separat terminal.

1.  **Start API (Backend):**
    ```powershell
    dotnet run --project src/Api
    ```
    *Kører typisk på http://localhost:5000 eller 5001.*

2.  **Start MVC (Apotek):**
    ```powershell
    dotnet run --project src/Web.Mvc
    ```
    *Kører default på http://localhost:5228.*

3.  **Start WPF (Lægehus):**
    ```powershell
    dotnet run --project src/Client.Wpf
    ```

---

## Manual Test Workflow

Nedenstående scenarier gennemgår systemets funktionalitet fra start til slut. Det forudsættes, at databasen er frisk (slettet før start).

### 1. Apotek: Søgning og Udlevering
*Test af MVC-interface og forretningslogik for udlevering.*

1.  Gå til Apotek-forsiden: [http://localhost:5228/Apotek](http://localhost:5228/Apotek)
2.  Indtast CPR: `1205851234` (Sofie Madsen).
3.  **Forvent:** En åben recept vises med ordinationen "Panodil". Status er "Udleveret: 1 af 5".
4.  Klik på knappen **"Udlever"**.
5.  **Forvent:** Siden opdateres. Status ændres til "Udleveret: 2 af 5". Knappen er stadig synlig (da der er flere udleveringer tilbage).

### 2. Lægehus: Oprettelse af Recept
*Test af WPF-klient og API-integration.*

1.  Åbn WPF-applikationen.
2.  Udfyld følgende (brug testdata fra seeding):
    *   **Ydernummer:** `1001`
    *   **ClinicId:** `d83b4c1a-5e2f-4b6a-9c8d-1e2f3a4b5c6d`
    *   **Patient CPR:** `3333333333`
    *   **Navn:** `Ny Patient`
    *   **Præparat:** `Ipren`
    *   **Dosis:** `1 stk ved behov`
    *   **Antal udleveringer:** `1`
3.  Klik **"Opret recept"**.
4.  **Forvent:** Statusbesked i bunden af vinduet: "Recept oprettet."
5.  **Verificering:** Gå til MVC-applikationen (`/Apotek`) og søg på CPR `3333333333`. Recepten bør nu vises.

### 3. Valgfri: Tildelt Apotek
*Test af filtrering på tildelte recepter.*

1.  Gå til [http://localhost:5228/Apotek/Assigned](http://localhost:5228/Apotek/Assigned)
2.  Denne side simulerer, at man er logget ind som "Aarhus Løve Apotek".
3.  **Forvent:** Recepten for "Sofie Madsen" (CPR `1205851234`) vises, da den specifikt er tildelt dette apotek.
4.  **Bemærk:** Andre recepter (f.eks. Lars Nielsen) vises ikke her.

### 4. Valgfri: Admin - Lukning af udløbne recepter
*Test af batch-job logik.*

1.  Gå til Admin-siden: [http://localhost:5228/Admin](http://localhost:5228/Admin)
2.  Klik på knappen **"Kør Job"** under "Luk Udløbne Recepter".
3.  **Forvent:** En besked vises: "Job udført. Lukkede 1 udløbne recept(er)." (Det er recepten for "Gerda Olesen" fra 2021).
4.  **Verificering:** Gå til `/Apotek` og søg på CPR `0101409999`. Der bør ikke længere være åbne recepter.

---

## Automated Tests

Løsningen indeholder en lille suite af unit tests (xUnit), der tester domænelogik, repositories og services uden brug af en fysisk database (InMemory).

Kør tests fra root af projektet:

```powershell
dotnet test
```

**Forventet output:**
> Test summary: total: 15, failed: 0, succeeded: 15, skipped: 0.
