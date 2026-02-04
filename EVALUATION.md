# Evaluering af Repo-skaber: Frontend & UX Kompetencer

**Evalueret projekt:** Receptserver System (Eksamensopgave E2025)  
**Skaber:** Phillip Møller (jf. domain-class-diagram.puml)  
**Evalueringsdato:** Februar 2026

---

## Vigtig Kontekst

Dette er en **backend-fokuseret eksamensopgave** i .NET. Projektet demonstrerer primært:
- 3-lags arkitektur (Domain, Application, Infrastructure)
- Entity Framework Core og databasedesign
- REST API design
- Unit testing (15 tests)
- Dependency Injection og clean code

**Evalueringen tager højde for projektets scope** - elementer der ikke ville forventes i et backend-eksamenprojekt noteres men tæller ikke negativt.

---

## Overblik over Projektet

- **Backend:** .NET 8, Entity Framework Core, SQLite
- **Web frontend:** ASP.NET MVC med Razor Views og Bootstrap 5
- **Desktop klient:** WPF (Windows Presentation Foundation)
- **API:** REST API til integration

---

## Evaluering (Skala 0-10)

### 1. UI-implementering: **6/10**

**Styrker:**
- Korrekt anvendelse af Bootstrap 5 til responsive layouts
- Proper brug af ASP.NET MVC patterns (Tag Helpers, ViewModels, Razor syntax)
- Formularer med sikkerhedsvalidering (AntiForgeryToken)
- WPF applikation med struktureret XAML og ScrollViewer
- Konsistent tabel- og kortlayout til datavisning
- God brug af Bootstrap-komponenter (cards, alerts, badges, buttons)
- Korrekt state-håndtering via TempData for beskeder

**Observationer (uden for projektets scope):**
- Minimal custom CSS - passende for et backend-projekt hvor Bootstrap dækker behovet
- WPF er funktionel men simpel - tilstrækkeligt til at demonstrere API-integration

**Vurdering:** Demonstrerer kompetent implementering af UI med standard frameworks. Alle nødvendige komponenter fungerer korrekt.

---

### 2. UX-forståelse: **6/10**

**Styrker:**
- Klar og logisk brugerflow for receptsøgning og udlevering
- Fejlbeskeder og succesbekræftelser vises tydeligt
- Logisk informationsgruppering (cards for recepter med tilhørende ordinationer)
- Dansk sprog brugt konsekvent - tilpasset målgruppen
- Intuitiv navigation mellem funktioner
- Fornuftig datavisning (tabel med præparat, dosis, status)

**Observationer (uden for projektets scope):**
- Hardcoded apotek-ID simulerer login - acceptabelt for demo/eksamen
- Ingen loading states - unødvendigt ved lokal SQLite database
- Ingen brugerresearch/personas - ikke forventet i teknisk eksamensprojekt

**Vurdering:** Viser grundlæggende forståelse for brugervenlige flows. UI'et er logisk opbygget og let at forstå.

---

### 3. Design systemer: **5/10**

**Styrker:**
- Bootstrap som konsistent fundament - industristandard valg
- Delt layout via `_Layout.cshtml` sikrer visuel konsistens
- ViewModels giver klar datastruktur mellem controller og view
- Fælles navigationsstruktur på tværs af sider
- Konsistent brug af Bootstrap-klasser

**Observationer (uden for projektets scope):**
- Intet custom komponentbibliotek - ikke nødvendigt for projektets størrelse
- Gentaget markup i views - kunne refaktoreres til partials, men fungerer fint
- Ingen design dokumentation - passende for eksamensprojekt

**Vurdering:** Pragmatisk tilgang med Bootstrap. For et projekt af denne størrelse er det en fornuftig beslutning at bruge etablerede frameworks frem for custom systemer.

---

### 4. Prototyping: **5/10**

**Styrker:**
- PlantUML klassediagram dokumenterer domænemodellen klart
- README med detaljerede testscenarier fungerer som funktionel specifikation
- Diagrammet inkluderer noter der forklarer forretningsregler
- Klar visualisering af relationer mellem entiteter

**Observationer (uden for projektets scope):**
- Ingen UI wireframes - ikke forventet i backend-eksamen
- Teknisk dokumentation frem for stakeholder-venlig - passende for målgruppen

**Vurdering:** Dokumentationen er tilstrækkelig og professionel for projekttypen. PlantUML diagrammet viser evne til at kommunikere teknisk design.

---

### 5. Performance: **6/10**

**Styrker:**
- Async/await patterns brugt konsistent gennem hele kodebasen
- Entity Framework med korrekte Includes (eager loading undgår N+1 problemer)
- SQLite - letvægts og passende til projektets skala
- Korrekt brug af CancellationTokens i alle async metoder
- Minificerede biblioteksfiler (Bootstrap, jQuery)

**Observationer (uden for projektets scope):**
- Ingen pagination - datasættet er lille (4 recepter i seed data)
- Ingen caching - unødvendigt for lokal demo med få brugere
- Ingen bundling af custom assets - kun ~20 linjer custom CSS

**Vurdering:** Performance-best practices er fulgt hvor det giver mening. Koden er forberedt til skalering med async patterns.

---

## Samlet Vurdering

| Kategori | Score | Bemærkning |
|----------|-------|------------|
| UI-implementering | 6/10 | Kompetent brug af standard frameworks |
| UX-forståelse | 6/10 | Logiske flows og klar brugerfeedback |
| Design systemer | 5/10 | Pragmatisk Bootstrap-tilgang |
| Prototyping | 5/10 | Tilstrækkelig teknisk dokumentation |
| Performance | 6/10 | Korrekte async patterns |
| **Gennemsnit** | **5.6/10** | |

---

## Konklusion

Skaberen demonstrerer **solid kompetence** i at implementere fungerende og brugervenlig UI med etablerede frameworks. Valget af Bootstrap og standard ASP.NET MVC patterns er fornuftigt for projekttypen.

**Hvad evalueringen viser:**
- Evne til at omsætte krav til fungerende brugergrænseflade
- Forståelse for grundlæggende UX-principper (feedback, logiske flows)
- Pragmatisk tilgang til teknologivalg
- Korrekt brug af async/await og EF Core patterns

**Bemærk:** Scoreringen afspejler hvad der *faktisk er demonstreret* i projektet. Den siger ikke nødvendigvis noget om skaberens fulde potentiale inden for frontend/UX - blot at dette projekt ikke var fokuseret på disse områder.
