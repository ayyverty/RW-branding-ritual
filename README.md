# Branding Ritual

A RimWorld 1.6 mod that adds an ideology-neutral Ideology ritual for burning a permanent brand into a pawn.

A moral guide gathers a piece of steel, carries the target onto a ritual spot or altar, and holds the iron beside them while you choose a mark. The target is held in place for the duration and cannot walk away. The brand is permanent.

Every ideology may perform the ritual: it requires no memes and no additional precepts.

| | |
|---|---|
| **Package ID** | `branding.ritual` |
| **Author** | toasterbath |
| **Version** | 1.6.1.0 |
| **Game version** | RimWorld 1.6 (developed against 1.6.4871) |
| **Required DLC** | Ideology (`ludeon.rimworld.ideology`) |
| **Harmony** | not used |
| **Other mods** | none required |

---

## Requirements

- RimWorld 1.6
- The **Ideology** expansion

Nothing else. The mod contains a single assembly (`SlaveBranding.dll`) and defs only — no Harmony patching, no dependency on RimJobWorld or any other mod. It will sit alongside RimJobWorld if you have it, but does not interact with it.

## Installation

Either drop the mod folder into your RimWorld `Mods` directory, or subscribe/build it and enable **Branding Ritual** in the mod list. Ideology must also be enabled. Load **after** Ideology.

```
<RimWorld>/Mods/branding-ritual/
├── About/
│   ├── About.xml
│   └── Manifest.xml
├── 1.6/
│   ├── Assemblies/SlaveBranding.dll
│   ├── Defs/
│   │   ├── DutyDefs/Duties_Branding.xml
│   │   ├── HediffDefs/Hediffs_Brands.xml
│   │   ├── JobDefs/JobDefs_Branding.xml
│   │   ├── PreceptDefs/Precepts_Branding.xml
│   │   ├── PreceptDefs/RitualPatternDefs/RitualPatterns_Branding.xml
│   │   ├── RitualDefs/Brands_Branding.xml
│   │   ├── RitualDefs/Ritual_Behaviors_Branding.xml
│   │   ├── RitualDefs/Ritual_Outcomes_Branding.xml
│   │   └── ThoughtDefs/Thoughts_Branding_Quality.xml
│   └── Languages/English/Keyed/Branding.xml
└── LoadFolders.xml
```

## Using the ritual

1. Build or obtain a **ritual spot** or **altar**. Either satisfies the ritual's target filter.
2. Open your ideology's **Rituals** tab and start **Branding**. It can be started at any time, at any spot.
3. Assign a **brander** and a **brandee**.
   - The brander prefers an actual `Moralist` ideo role, but any pawn may step in as a substitute at reduced outcome quality.
   - The brandee may be **any humanlike pawn** — colonist, guest, prisoner, or slave. The brander cannot brand themselves.
4. The ritual runs for 7500 ticks (2 min 5 s at normal speed) across three stages.

Stage timers are **cumulative** across the whole ritual, matching vanilla. Stage 1 is fixed at the first 30% (2250 ticks); stage 2 ends the moment the brandee is delivered rather than on a timer; stage 3 runs from wherever stage 2 left off to 100%.

### Stage 1 — The iron (first 30%)

The brander fetches **one steel** from the colony stockpile and carries it. If the steel is already in their inventory this resolves immediately and they hold position until the timer runs out. If the ritual is abandoned or interrupted during this stage, the steel is dropped back rather than lost.

### Stage 2 — The spot (ends on delivery)

The brander escorts the brandee **onto** the spot itself (not beside it), where they are stunned and laid down awake. They stay there and cannot walk off for the rest of the ritual. The stage fails if the brander falls asleep or the brandee becomes unreachable.

The 7000-tick stun is guaranteed to outlast the ritual, since it cannot start before 2250 ticks have elapsed and the ritual ends at 7500.

### Stage 3 — The brand (to 100%)

The brander stands beside the brandee holding the iron in both hands for the whole stage, and the brand-selection window opens. If the iron was lost along the way, the brander fetches a replacement first. If the brandee dies mid-stage, the stage ends instead of opening a dialog for a corpse.

You then pick one of four brands. The steel is consumed whether or not the brander still had it in hand.

| Action | Result |
|---|---|
| **Brand them** | Applies the chosen brand permanently, consumes 1 steel |
| **Leave unbranded** | Closes the dialog; the ritual still completes, but no brand is applied |

Closing the dialog with the X or Escape is equivalent to *Leave unbranded*.

### Witnesses

Everyone who watches is a **witness**, and witness count feeds ritual quality (see below). During the branding stage, witnesses line up along the horizontal sides of the spot so the mark is visible.

## The four brands

All four are permanent global hediffs. They never progress, never change severity, and cannot be tended or cured. They are removed only when the pawn dies. Stat effects live on the hediff's stage and apply at full strength.

### Brand of Service

*A mark of hard-won obedience.*

| Effect | Value |
|---|---|
| Work speed | ×1.15 (+15%) |
| Social impact | ×0.85 (−15%) |

The bearer works faster, but holds them back in every conversation.

### Brand of Chains

*The bearer is held fast and stays docile.*

| Effect | Value |
|---|---|
| Slave suppression offset | +0.25 (+25%) |
| Social impact | ×0.85 (−15%) |

Slave suppression lasts far longer and they rarely think of rebellion. They have little left to say for themselves.

> Implementation note: `SlaveSuppressionOffset` has a base value of 0, so this is a **flat stat offset**, not a multiplier. A multiplier would multiply zero and do nothing. It displays as a percentage in the stat readout.

### Brand of Pain

*Feeling runs shallow beneath the mark.*

| Effect | Value |
|---|---|
| Pain offset | −0.3 |
| Social impact | ×0.85 (−15%) |

The bearer shrugs off what should floor them and rarely goes down from pain. Felt pain is clamped at 0, so a healthy wearer absorbs the whole value while an already-injured wearer still feels pain. Still, they have little left to say for themselves.

### Brand of Pleasure

*The bearer draws others in and holds attention easily.*

| Effect | Value |
|---|---|
| Social impact | ×1.2 (+20%) |
| Work speed | ×0.85 (−15%) |

Their focus wanders and work suffers for it.

## Ritual quality and outcomes

Ritual quality affects the **outcome narration and mood only**. It never gates which brand is applied — the brand is always your explicit choice, and the ritual succeeds at any quality.

Starting quality is `0.5`, clamped to `0.2`–`0.9`. Quality offsets:

| Source | Offset |
|---|---|
| Real `Moralist` brander (not a substitute) | +0.2 |
| 3 witnesses | +0.2 |
| 8 witnesses | +0.5 |
| Held at a ritual spot (altars also count) | +0.05 |

Outcomes roll as:

| Chance | Outcome | Positivity | Witness mood |
|---|---|---|---|
| 25% | Cruel | −1 | −4 |
| 35% | Unsparing | 0 | 0 |
| 40% | Clean | +1 | +2 |

Each outcome hands its witnesses a `Thought_AttendedRitual` memory for 6 days, stacking up to 3 times, mirroring vanilla's scarification thoughts. **Unsparing is deliberately mood-neutral** — a competent but unremarkable result neither cheers nor appalls anyone. Only the brander, brandee, and witnesses are involved; no letter or development points are generated.

## How brands are defined

Brands are data, not code. `SlaveBranding.BrandDef` is a `Def` subclass with two fields — `label`, `description`, and a `hediffDef` reference — and the dialog builds itself from every `BrandDef` in the database, ordered by `defName`. Adding a fifth brand needs no new C#.

```xml
<SlaveBranding.BrandDef>
  <defName>toast_BrandOfExample</defName>
  <label>Brand of Example</label>
  <description>What the mark does.</description>
  <hediffDef>toast_BrandOfExampleHediff</hediffDef>
</SlaveBranding.BrandDef>
```

```xml
<HediffDef>
  <defName>toast_BrandOfExampleHediff</defName>
  <label>brand of example</label>
  <description>What the mark does.</description>
  <hediffClass>HediffWithComps</hediffClass>
  <initialSeverity>1</initialSeverity>
  <isBad>false</isBad>
  <tendable>false</tendable>
  <!-- No HediffCompProperties_Disappears comp: this is what makes it permanent. -->
  <stages>
    <li>
      <label>branded</label>
      <statFactors>
        <MoveSpeed>0.9</MoveSpeed>
      </statFactors>
    </li>
  </stages>
</HediffDef>
```

Stat effects (`statFactors`, `statOffsets`, `painOffset`, `capMods`, …) belong on the **stage**, not on `HediffDef`. A hediff is permanent simply by omitting a `HediffCompProperties_Disappears` comp and setting `tendable` to false.

## Localization

All user-facing strings are in `1.6/Languages/English/Keyed/Branding.xml` under the `SB_` prefix. Def labels and descriptions are plain English in the defs; only the dialog and eligibility-rejection messages are keyed.

## Known limitations

- **Dialog can be skipped.** `RitualStageAction_ChooseBrand` declines to open if any non-immediate dialog is already up, and does not retry. If another modal window happens to be open when the stage fires, no brand dialog appears for that ritual.
- **Restraint is a timed stun.** Immutability comes from vanilla's `RitualStageAction_StunPawns` at 7000 ticks rather than a persistent restraint hediff. If the ritual is forcibly interrupted the stun can outlive it.
- **Target role filtering is permissive.** `RitualRole_Target` overrides the default restrictions and accepts any humanlike pawn, including downed ones and children. It does not check for dead pawns beyond what the base ritual logic already filters.
- **Precept generation.** The precept sets `canGenerateAsSpecialPrecept` to false. It is visible and listed for roles, but confirm it appears without manual setup in ideologies that have no ritual precepts yet.

## Development

### Layout

```
Source/SlaveBranding/
├── BrandDef.cs                      Def subclass for a selectable brand
├── BrandingStrings.cs               Translated string accessors
├── BrandingUtility.cs               Applies the hediff, consumes the steel
├── Dialog_ChooseBrand.cs            Brand-selection window
├── JobDriver_HoldBrandingIron.cs    Pick up the iron, then hold it indefinitely
├── JobGiver_HoldBrandingIron.cs     Issues the hold job; declines when no iron
├── RitualRole_Target.cs             Any-humanlike target eligibility
└── RitualStageAction_ChooseBrand.cs Opens the dialog during the branding stage
```

### Build

Requires the .NET SDK. The project targets `net472` and references `Krafs.Rimworld.Ref`, which supplies the RimWorld reference assemblies without needing a game install to compile against.

```bash
~/.dotnet/dotnet build -c Release
```

Output goes straight to `1.6/Assemblies/SlaveBranding.dll`, and the post-build target pokes the version into `About/About.xml` and `About/Manifest.xml`.

### Deploy

```bash
MODSRC=/home/avery/Projects/rjw-slavebranding-ritual
MODDST=/path/to/RimWorld/Mods/branding-ritual
rsync -a --delete "$MODSRC/About" "$MODSRC/1.6" "$MODSRC/LoadFolders.xml" "$MODDST/"
```

## Credits

Ideology ritual framework, scarification icon, `RitualStage_InteractWithRole`, `DeliverPawnToAltar`, `RitualPosition_*`, and `JobDriver_PickupToHold` to Ludeon Studios.