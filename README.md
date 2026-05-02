## Cim Be Smart (CS2) — stop cims calling taxis
 
**Cim Be Smart** is a Cities: Skylines II game mod that blocks taxis on the *rider (demand)* side.  
When enabled, cims will choose other travel options (walk, bike, private car, public transport if it exists) instead of taking taxis.
This helps reduce city taxi traffic as taxis can only carry 1-4 passengers and can overwhelm city roads.
 
### What this mod does
 
- **Prevents new taxi selection** by forcing `ResidentFlags.IgnoreTaxi`, which causes `RouteUtils.GetTaxiMethods(...)` to remove `PathMethod.Taxi` from trip planning.
- **Prevents “stuck waiting for taxi”** by:
  - clearing taxi-lane wait flags (`CreatureLaneFlags.Taxi` / `ParkingSpace`) and forcing a re-route (`PathFlags.Obsolete`)
  - clearing taxi-stand waiting only when the queue target is a taxi stand (`TaxiStand`), so bus/train waiting isn’t broken
- **Stops taxi dispatch demand** by cancelling `TaxiRequest` entities for `Customer / Outside / None` (stand requests are left alone to avoid fighting `TaxiStandSystem`).
 
### Vanilla taxi flow (simplified)
 
1. Cim considers taxi as a travel method.
2. Cim enters a taxi pickup/wait state.
3. A `TaxiRequest` entity gets created (e.g. by `RideNeederSystem`, plus other sources like taxi stands / outside).
4. `TaxiDispatchSystem` matches a request to a taxi (including taxis from outside connections).
 
### How Cim Be Smart changes the flow
 
When enabled:
 
1. **Taxi is removed from rider decision-making** (`ResidentFlags.IgnoreTaxi`).
2. **Any current taxi waiting is unwound** so cims re-route instead of freezing.
3. **Taxi requests get cancelled** before dispatch can satisfy them, so taxi traffic drops over time.
 
### Expected results
 
- **Taxi usage drops sharply** after a short settling period.
- Existing taxis may still be visible briefly (finishing old behavior), but fewer new taxi trips should appear.
- Cims should **not** get stuck waiting forever for a taxi.
 
### Options / Usage
 
- Install the mod.
- In **Options → Cim Be Smart → Actions**:
  - Enable **Cims Block taxi use** (default: ON)
  - Optional: include/exclude **Commuters** and **Tourists**
- Let the simulation run for a few minutes to see taxi traffic decline.
- Taxi Stand - block them from requesting taxis drive to stands and park there. This is a minor bonus feature if it can be made to work.

| Setting | Meaning |
|---:|---|
| 0% | No residents use taxis |
| 25% | About 1 in 4 residents may use taxis |
| 50% | About half of residents may use taxis |
| 75% | About 3 in 4 residents may use taxis |
| 100% | Residents use taxis like vanilla |


| Situation | Residents allowed to use taxis | Commuters avoid taxis | Tourists avoid taxis | Meaning |
|---|---:|---:|---:|---|
| First install / mod default | 0% | OFF | OFF | Residents avoid taxis; commuters/tourists are left vanilla. |
| Reset to Game Defaults button | 100% | OFF | OFF | Vanilla-style taxi behavior. |

###  Credits
- River Mochi mod author
- Noel/Noel2: testing, feedback
