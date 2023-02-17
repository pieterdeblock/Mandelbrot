
## Feedback Applied Programming

### Opgave 1: Mandelbrot fractaal

#### Algemeen

#### Architectuur (15%)

***Modulair, meerlagenmodel***

- [x] *Meerlagenmodel via mappen of klassebibliotheken*
- [x] *Dependency injection*
- [x] *Gebruik  MVVM Design pattern*

***'Separation of concern'***

- [x] *Domein-logica beperkt tot logische laag*
- [x] *Logische laag onafhankelijk van presentatielaag*


#### Programmeerstijl, Kwaliteit van de code (10%)

***Naamgeving***

- [x] *Naamgeving volgens C# conventie*
- [x] *Zinvolle, duidelijke namen*

* gebruik 'this' ankel als er verwarring mogelijk is.

***Korte methodes***

- [x] *maximale lengte ~20 lijnen*

***Programmeerstijl***

- [x] *Layout code*
- [ ] *Correct gebruik commentaar*
- [x] *Algemene programmeerstijl*

#### User interface, functionaliteit, UX (15%) 

- [x] *Layout UI*
- [x] *Goede UX*

* Een wijziging van de iteratielimiet heeft pas effect de volgend ekeer dat er een zoom- of pan-operatie gebeurt.
* Bij een hoge iteratielimiet en meerdere bewerkingen na elkaar wordt je toepassing traag.

***functionaliteit***

- [x] *Goede weergave fractaal*
- [x] *Weergave numerieke resultaten*
- [x] *Zooming*
- [x] *Panning*
- [x] *Aanpasbare iteratielimiet*
- [x] *instelbare kleurenweergave*

* Voor de muispositie geef je de pixelcoördinaten (in de bitmap) weer. Dat zouden de 'model'-coördinaten binnen de Mandelbrot ruimte moeten zijn.


#### Goede werking, snelheid, bugs (25%)


***juiste technieken gebruikt***

- [x] *Juiste berekening fractaal*
- [x] *Zooming & Panning goed verwerkt*

***Juiste werking***

- [x] *Goede werking*

***Snelheid, efficiëntie, concurrency***

- [x] *Goed gebruik concurrency*
- [x] *Efficiënte berekeningen*

* Een 'Parallel.For' nesten in een andere 'Parallel.For' is niet zo'n goed idee: de scheduling gebeurt dan niet optimaal.
* Wanneer er snel na elkaar ingezoomd of gepanned wordt dan reageer je toch pas nadat de vorige berekening helemaal afgewerkt is (terwijl die eigenlijk niet meer nodig is). Dat vertraagt de werking! Je kan die vorige berekening dan beter afbreken en onmiddellijk met de nieuwe starten.

***Bugs***

- [ ] *Geen bugs*

* Bij het resizen krijg je soms een 'IndexOutOfRange' exception.
* Er zijn doorlopend interne excepties bij 'MouseMove' Je vangt die op met een lege 'try..catch' maardat is geen goede techniek.

#### Installeerbare package voor distributie (10%)

- [x] *Installable package beschikbaar in repo*

#### Correct gebruik GIT (10%)

- [x] *Gebruik 'atomaire' commits*
- [x] *zinvolle commit messages*



#### Rapportering (15%)

- [x] *Structuur*
- [x] *Volledigheid*
- [x] *Technische diepgang*
- [x] *Professionele stijl*

* Je verslag is goed verzorgd maar blijft oms nog wat te oppervlakkig.
