# Taxi Traffic (Cities: Skylines II)

**Taxi Traffic** cuts down the taxis clogging your streets.

A taxi only carries a few people, so a city full of them can mean a lot of traffic for not many passengers. This mod nudges cims to use another travel option instead.

---

## How it works

Taxi Traffic uses the game's own **IgnoreTaxi** behavior.

- Cims you block are told to avoid taxis.
- Cims still waiting for an unassigned taxi can leave the pickup spot and find another way to travel.
- If a taxi is already dispatched, boarding, or carrying someone, that trip is left alone to finish.
- Taxi stands still work normally, including their usual standby taxis.
- If local taxi supply can't keep up, outside connections (OC) can send taxis into the city.
- At max taxi-avoid settings, testing showed few or no OC taxis coming into the city.

No Harmony patching and no attempt to shut down the whole taxi system.

---

## Options

Found in **Options → Taxi Traffic → Actions**.

| Setting | What it does |
|---|---|
| **Residents avoid taxis** | 0% = vanilla taxi choice. 100% = all local households avoid taxis. |
| **Commuters avoid taxis** | ON = commuters avoid taxis. |
| **Tourists avoid taxis** | ON = tourists avoid taxis. |
| **Game Defaults** | Returns to vanilla taxi choices: 0%, commuters OFF, tourists OFF. |

### Fresh install

Taxi Traffic starts strong so you can actually see the difference:

- **Residents avoid taxis: 100%**
- **Commuters avoid taxis: ON**
- **Tourists avoid taxis: ON**

Want some taxi traffic back? Lower the slider or turn either group off.

---

## What to expect

- Taxi traffic drops after the city runs for a few minutes.
- The Transportation InfoView monthly numbers take longer to catch up.
- A few taxis can still remain while active trips finish or taxis wait at stands.
- Taxi stands are left alone. Don't want standby taxis there? Remove the stands.

---

## Status

The Status tab gives a quick look at things like:

- current taxi passengers
- parked vs active taxis
- outside-connection taxis
- taxi trip purpose
- current Taxi Traffic blocking

Status is bonus info; the mod's main job is reducing taxi traffic.

---

## Safety

- **Game Defaults** returns Taxi Traffic's own changes back to vanilla behavior.
- Taxi Traffic tracks the cims it changed so it does not wipe vanilla's own IgnoreTaxi flags.
- No Harmony patching.

---

## Credits and thanks

- **River Mochi** — mod author
- **Noel / Noel2** (of MapExt) — testing and feedback
- Thumbnail: *World Class Traffic Jam 2* by joiseyshowaa, Freehold NJ — [CC BY-SA 2.0](https://commons.wikimedia.org/w/index.php?curid=63542844)

Source: <https://github.com/River-Mochi/cs2-TaxiTraffic>
