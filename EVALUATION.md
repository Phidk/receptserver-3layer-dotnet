# Evaluering af Repo-skaber: Frontend & UX Kompetencer

**Evalueret projekt:** Receptserver System (Eksamensopgave E2025)  
**Skaber:** Phillip Møller (jf. domain-class-diagram.puml)  
**Evalueringsdato:** Februar 2026

---

## Overblik over Projektet

Repositoryet indeholder et komplet "Receptserver" system bygget med:
- **Backend:** .NET 8, Entity Framework Core, SQLite
- **Web frontend:** ASP.NET MVC med Razor Views og Bootstrap 5
- **Desktop klient:** WPF (Windows Presentation Foundation)
- **API:** REST API til integration
- **Tests:** 15 xUnit tests med InMemory database

Arkitekturen er velstruktureret med 3-lags opdeling (Domain, Application, Infrastructure) og demonstrerer solid forståelse for backend-mønstre.

---

## Evaluering (Skala 0-10)

### 1. UI-implementering: **5/10**

**Styrker:**
- Korrekt anvendelse af Bootstrap 5 til responsive layouts
- Proper brug af ASP.NET MVC patterns (Tag Helpers, ViewModels, Razor syntax)
- Formularer med sikkerhedsvalidering (AntiForgeryToken)
- WPF applikation med struktureret XAML
- Konsistent tabel- og kortlayout til datavisning
- God brug af Bootstrap-komponenter (cards, alerts, badges, buttons)

**Svagheder:**
- Meget minimal custom CSS (~20 linjer, primært fokusering på focus-states)
- Næsten udelukkende Bootstrap defaults uden personlig styling
- Ingen custom komponenter eller avancerede interaktioner
- WPF klienten er meget basal (simpel formular uden styling)
- Ingen JavaScript-interaktioner ud over biblioteksdefaults
- Gentaget kode på tværs af views (Index og Assigned er næsten identiske)

**Konkret observation:**
```css
/* site.css - Hele custom CSS filen */
html { font-size: 14px; }
@media (min-width: 768px) { html { font-size: 16px; } }
.btn:focus, .btn:active:focus... { box-shadow: ... }
```

---

### 2. UX-forståelse: **4/10**

**Styrker:**
- Klar brugerflow for receptsøgning og udlevering
- Fejlbeskeder vises tydeligt til brugeren
- Succesbekræftelse efter handlinger
- Logisk gruppering af information (cards for recepter)
- Dansk sprog brugt konsekvent i UI

**Svagheder:**
- Ingen synlig brugerresearch eller persona-overvejelser
- Meget lineære/basale workflows uden alternative stier
- Ingen tilgængelighedsovervejelser (accessibility)
- Ingen loading states eller progress indikatorer
- Hardcoded apotek-ID i "tildelte recepter" view (ingen reel brugerkontekst)
- Ingen bekræftelsesdialoger for vigtige handlinger (f.eks. udlevering)
- WPF giver minimal feedback - kun en tekstlinje for status

**Konkret observation:**
```csharp
// Hardcoded pharmacy ID - simulerer login i stedet for reel implementering
var pharmacyId = new Guid("45a1b2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c");
model.PharmacyName = "Aarhus Løve Apotek";
```

---

### 3. Design systemer: **3/10**

**Styrker:**
- Bruger Bootstrap som konsistent fundament
- Delt layout via `_Layout.cshtml` sikrer visuel konsistens
- ViewModels giver datastruktur-konsistens
- Fælles navigationsstruktur på tværs af sider

**Svagheder:**
- Intet custom komponentbibliotek
- Fuld afhængighed af Bootstrap defaults uden udvidelser
- Ingen design tokens eller CSS variabler
- Ingen dokumentation af designbeslutninger
- Gentaget markup på tværs af views (kunne abstrahere til partials)
- Ingen theming eller dark mode support
- WPF og MVC deler ikke design-sprog (ingen fælles identitet)

**Konkret observation:**
Views `Index.cshtml` og `Assigned.cshtml` i Apotek-mappen er næsten identiske (~95% ens kode) uden brug af shared components/partials.

---

### 4. Prototyping: **3/10**

**Styrker:**
- Klassediagram i PlantUML format (domain-class-diagram.puml)
- README indeholder detaljerede testscenarier (fungerer som pseudo-prototyper)
- Klar domænemodel-visualisering med noter

**Svagheder:**
- Ingen wireframes eller UI mockups
- Ingen klikbare prototyper
- Dokumentation er teknisk/udvikler-orienteret, ikke stakeholder-venlig
- Klassediagram er rent teknisk (ikke brugerrejse-fokuseret)
- Ingen brugerhistorier eller use-case diagrammer

**Konkret observation:**
Dokumentationen i `docs/` indeholder kun teknisk klassediagram, ingen UI/UX artefakter.

---

### 5. Performance: **4/10**

**Styrker:**
- Async/await patterns brugt konsistent gennem hele kodebasen
- Entity Framework med korrekte Includes (eager loading undgår N+1)
- SQLite for letvægts database
- Korrekt brug af CancellationTokens
- Minificerede CSS/JS biblioteksfiler inkluderet

**Svagheder:**
- Ingen synlige performance-optimeringer
- Ingen caching implementering
- Ingen lazy loading overvejelser i UI
- Ingen bundling/minification setup for custom assets
- Ingen overvågning eller logging for performance-problemer
- Håndtering af store datamængder ikke demonstreret
- Ingen pagination på receptlister

**Konkret observation:**
```csharp
// Henter alle recepter på én gang uden pagination
return await _dbContext.Prescriptions
    .Include(p => p.MedicationOrders)
    .Where(p => p.Patient.CprNumber == cprNumber && p.Status == PrescriptionStatus.Open)
    .ToListAsync(cancellationToken);
```

---

## Samlet Vurdering

| Kategori | Score | Bemærkning |
|----------|-------|------------|
| UI-implementering | 5/10 | Funktionel men baseret på defaults |
| UX-forståelse | 4/10 | Basal brugerforståelse, mangler dybde |
| Design systemer | 3/10 | Ingen systematisk tilgang |
| Prototyping | 3/10 | Minimal visualisering/afklaring |
| Performance | 4/10 | Grundlæggende async, mangler optimering |
| **Gennemsnit** | **3.8/10** | |

---

## Kontekstuel Bemærkning

Dette projekt er en **eksamensopgave** med tydeligt fokus på:
- Backend-arkitektur og clean code principper
- .NET patterns (DI, Repository, Service layer)
- Entity Framework Core og databasedesign
- Unit testing og testbarhed

Evalueringen afspejler at projektet **ikke primært handler om frontend/UX**, men om at demonstrere backend-kompetencer. Scoreringen skal ses i dette lys - skaberen viser solid evne til at implementere fungerende UI med eksisterende frameworks, men har ikke fokuseret på avanceret frontend-udvikling eller UX-design i dette projekt.

**Styrker der skinner igennem:**
- Velstruktureret kodebase med god separation of concerns
- Grundig dokumentation og testdækning
- Korrekt brug af industri-standard frameworks
- Konsistent dansk sprogbrug i UI og kommentarer
