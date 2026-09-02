# Taxi Traffic (Cities: Skylines II)

Reduces taxi traffic by nudging cims to pick other travel options.

---

## How it works

Taxi Traffic uses the game's own taxi-avoidance behavior.

- Selected cims avoid taxis.
- Active taxi trips are left alone to finish.
- Taxi stands and outside-connection taxis still work normally.

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

### Defaults on a new install

- **Residents avoid taxis: 50%**
- **Commuters avoid taxis: OFF**
- **Tourists avoid taxis: OFF**

Want stronger taxi reduction? Raise the slider or turn either group on.

---

## What to expect

- Taxi traffic drops after the city runs for a few minutes.
- The Transportation InfoView monthly numbers take longer to catch up.
- A few taxis can still remain while active trips finish or taxis wait at stands.
- Taxi stands are left alone. Don't want standby taxis there? Remove the stands.

---

## Status

The Status tab gives a quick look at:

- current passengers, including local and OC
- parked and active taxis
- outside-connection taxis
- taxi trip purpose
- current Taxi Traffic blocks

Status is bonus info; the mod's main job is reducing taxi traffic.

---

## Safety

- **Game Defaults** returns Taxi Traffic's own changes back to vanilla behavior.
- Taxi Traffic tracks the cims it changed so it does not wipe vanilla's own IgnoreTaxi flags.
- No Harmony patching.

---

## Credits and thanks

- **River Mochi** — mod author
- **Noel / Noel2** (of MapExt): testing
- **Winterqt** (Grass sprites): testing
- Thumbnail: *World Class Traffic Jam 2* by joiseyshowaa, Freehold NJ — [CC BY-SA 2.0](https://commons.wikimedia.org/w/index.php?curid=63542844)

Source: <https://github.com/River-Mochi/cs2-TaxiTraffic>


### License

Taxi Traffic is licensed under GPL-3.0-or-later with the Cities: Skylines II Linking Exception.

The shared files in `Code/Utils/` are separately licensed under the MIT License; see `LICENSE-MIT`.
