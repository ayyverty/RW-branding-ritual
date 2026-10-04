# Branding Ritual

A RimWorld 1.6 mod that adds an Ideology ritual for burning a permanent brand into a pawn.

A moral guide gathers a piece of steel, carries the target onto a ritual spot or altar, and holds the iron beside them while you choose a mark. The target is held in place for the duration and cannot walk away.

Every ideology may perform the ritual: it requires no memes and no additional precepts.

| | |
|---|---|
| **Package ID** | `branding.ritual` |
| **Author** | toasterbath |
| **Version** | 1.6.1.0 |
| **Game version** | RimWorld 1.6
| **Required DLC** | Ideology
| **Harmony** | not used |
| **Other mods** | none required |

---

## Requirements

- RimWorld 1.6
- The **Ideology** expansion

## Installation

Either drop the mod folder into your RimWorld `Mods` directory, or subscribe and enable **Branding Ritual** in the mod list. Ideology must also be enabled. Load **after** Ideology.

## Using the ritual

1. Build or obtain a **ritual spot** or **altar**. Either satisfies the ritual's target filter.
2. Open your ideology's **Rituals** tab and start **Branding**. It can be started at any time, at any spot.
3. Assign a **brander** and a **brandee**.
   - The brander prefers an actual `Moralist` ideo role, but any pawn may step in as a substitute at reduced outcome quality.
   - The brandee may be **any humanlike pawn** — colonist, guest, prisoner, or slave. The brander cannot brand themselves.
4. The ritual runs for 7500 ticks (2 min 5 s at normal speed) across three stages.

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

All four are permanent global hediffs. They never progress, never change severity, and cannot be tended or cured. They are removed only when the pawn dies.

### Brand of Service

*A mark of hard-won obedience.*

| Effect | Value |
|---|---|
| Work speed | ×1.15 (+15%) |
| Social impact | ×0.85 (−15%) |

The bearer works faster, but the brand holds them back in every conversation.

### Brand of Chains

*The bearer is held fast and stays docile.*

| Effect | Value |
|---|---|
| Slave suppression offset | +0.25 (+25%) |
| Social impact | ×0.85 (−15%) |

Slave suppression lasts far longer and they rarely think of rebellion. They have little left to say for themselves.

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

Each outcome hands its witnesses a memory for 6 days, stacking up to 3 times. **Unsparing is deliberately mood-neutral** — a competent but unremarkable result neither cheers nor appalls anyone. Only the brander, brandee, and witnesses are involved; no letter or development points are generated.

## Mood effects

Branding also produces **continuous** mood effects.

The first two are measured against a **pool**: the colony's **slaves**, the colony's **prisoners**, and the colony's **free colonists whose gender is not the supreme gender of their ideology**.

| Thought | Mood | Applies when |
|---|---:|---|
| **All accounted for** | +6 | The pool is non-empty and every member of it is branded |
| **Unbranded** | −6 | The pool is non-empty and at least one member is not branded |
| **Branded** | −8 | The pawn carrying the thought is themselves branded |

**All accounted for** and **Unbranded** are mutually exclusive. They apply only to pawns you control who are not themselves enslaved or imprisoned, so a branded pawn never gets the colony-wide thought on top of the personal one.

**Branded** applies to any branded pawn regardless of faction or standing, including guests.

If the pool is **empty** — no slaves, no prisoners, and no free colonists of a non-supremacist gender — neither colony thought applies. There is nothing to be satisfied or dissatisfied about.

**Unbranded** is the state a colony starts in and the state a newly taken prisoner returns it to, so expect it to be the thought you see most often.

## Known limitations

- **Dialog can be skipped.** `RitualStageAction_ChooseBrand` declines to open if any non-immediate dialog is already up, and does not retry. If another modal window happens to be open when the stage fires, no brand dialog appears for that ritual.
- **Restraint is a timed stun.** Immutability comes from vanilla's `RitualStageAction_StunPawns` at 7000 ticks rather than a persistent restraint hediff. If the ritual is forcibly interrupted the stun can outlive it.
- **Target role filtering is permissive.** `RitualRole_Target` overrides the default restrictions and accepts any humanlike pawn, including downed ones and children. It does not check for dead pawns beyond what the base ritual logic already filters.
- **Colony mood thoughts are cached for 250 ticks.** `BrandingMoodCache` rescans at most once every 250 ticks (~4 s at normal speed) so that a mood recalculation does not rescan the colony once per pawn. Applying a brand invalidates the cache immediately, so the player never waits on it; a pawn gaining or losing slave or prisoner status can take up to 250 ticks to be reflected.
- **Non-supremacist colonists count toward the pool.** A free colonist of a non-supremacist gender is part of the measured set, so one unbranded colonist will keep the colony on **Unbranded**.

## Credits

Ideology ritual framework, scarification icon, `RitualStage_InteractWithRole`, `DeliverPawnToAltar`, `RitualPosition_*`, and `JobDriver_PickupToHold` to Ludeon Studios.
