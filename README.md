# README

## Postavljanje korisnika za pokretanje projekta

Za uspecno pokretanje projekta potrebno je kreirati sledece korisnike:

| **username**     | **password** | **groups** |
| ---------------- | ------------ | ---------- |
| wcfAudit         | 1234         |            |
| wcfService       | 1234         | Admin      |
| wcfClient        | 1234         | Client     |
| wcfSpecialClient | 1234         | Admin      |

---

## Pokretanje projekta

- **Projekat `Service`** pokrece se sa korisnikom `wcfService`.
- **Projekat `Client`** pokrece se sa korisnikom `wcfClient` ili `wcfSpecialClient`.
- **Projekat `Audit`** pokrece se sa korisnikom `wcfAudit`.

**Redosled pokretanja projekata:**

1. `Audit`
2. `Service`
3. `Client`

---

## Resavanje problema sa `Audit` projektom

Ako se javljaju problemi sa `Audit` projektom, uradite sledece:

1. Pokrenite **PowerShell** kao administrator.
2. Unesite sledecu komandu:
   ```powershell
   New-EventLog -LogName "MyOIBLogs" -Source "Audit"
   ```
3. Ponovo pokrenite projekat.

### Alternativno resenje

Ako problem i dalje postoji:

1. Otvorite **Registry Editor**.
2. Navigirajte do putanje:
   ```
   Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application
   ```
3. Dodajte novi **Key** i nazovite ga `Audit`.
4. Unutar njega:

   - Dodajte novi **String Value**.
   - Kao **Name** postavite: `EventMessageFile`.
   - Kao **Value Data** postavite:

     ```
     C:\Windows\Microsoft.NET\Framework64\v4.0.30319\EventLogMessages.dll

     ```
