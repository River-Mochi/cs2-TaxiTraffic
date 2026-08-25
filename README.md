# Taxi Traffic (Cities: Skylines II)

**Taxi Traffic** cuts down the number of taxis clogging your streets.

A taxi only carries 1–4 passengers, so a city full of them creates a lot of traffic for very few people actually moved. This mod encourages your citizens to travel another way instead — walking, cycling, driving, or public transport if you've built it.

You stay in control: a simple slider decides how many of your residents are still allowed to take a taxi.

---

## How it works

Taxi Traffic uses a switch the game already has built in, called **`IgnoreTaxi`**.

The base game already puts this flag on certain citizens during normal play, and when a citizen carries it the game simply leaves taxis out of their travel options. All this mod does is apply that same flag to more citizens, based on your settings.

That's the whole trick. Nothing is hacked and nothing is forced — the game keeps making its own travel decisions, it just stops offering taxis to the citizens you've chosen.

The mod also ties off two loose ends so nobody gets stranded:

- Citizens who were **already standing around waiting for a taxi** get released, so they set off another way instead of freezing in place.
- Taxi **call-outs still pending are cancelled**, so fewer taxis get sent out and spawned over time.

---

## Options

Found in **Options → Taxi Traffic → Actions**.

| Setting | What it does |
|---|---|
| **Residents allowed to use taxis** | The main slider — how many of your residents may still take a taxi. |
| **Commuters avoid taxis** | Also stop people travelling in from outside for work. Off by default. |
| **Tourists avoid taxis** | Also stop visiting tourists. Off by default. |
| **Game Defaults** | One click to put everything back to normal, vanilla behaviour. |

### The slider

| Setting | Meaning |
|---:|---|
| **0%** *(default)* | No residents use taxis |
| 25% | About 1 in 4 residents may use taxis |
| 50% | About half of residents may use taxis |
| 75% | About 3 in 4 residents may use taxis |
| 100% | Residents use taxis exactly like vanilla |

On a fresh install the slider starts at **0%** — the strongest setting — while commuters and tourists are left alone. Slide it up if you'd rather keep some taxi traffic around, or press **Game Defaults** to go back to 100% and fully vanilla behaviour.

---

## What to expect

- Taxi traffic **drops noticeably** once the simulation has run for a few minutes.
- Taxis already on the road will **finish what they're doing first**, so you'll still see a few for a short while. They thin out as they go.
- Citizens should **never get stuck** waiting for a taxi that isn't coming.

### Taxi stands

Taxi stands are left completely alone. If you've built them, taxis may still drive over and park there as usual — but with this mod switched on, very few citizens will actually hail one. Turn the slider up if you'd like your stands busy again.

---

## Safety

- **Save-safe.** Add or remove the mod at any time, on new or existing cities.
- **Fully reversible.** Set the slider to 100% (or press **Game Defaults**) and the game goes straight back to vanilla behaviour.
- **No Harmony patching**, so it's less likely to break when the game updates.

---

## Credits and thanks

- **River Mochi** — mod author
- **Noel / Noel2** (of MapExt) — testing and feedback
- Thumbnail: *World Class Traffic Jam 2* by joiseyshowaa, Freehold NJ — [CC BY-SA 2.0](https://commons.wikimedia.org/w/index.php?curid=63542844)

Source: <https://github.com/River-Mochi/cs2-TaxiTraffic>
