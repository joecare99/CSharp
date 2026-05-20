# SomeThing - A C# "How Not To" Playground

Welcome to `SomeThing`, a lovingly curated collection of questionable C# ideas.

This is not a best-practices guide. This is the pile of experiments you point at and say:

> "Kids, do not do this at home."

Yet somehow, it (mostly) builds.

## What is this?

Short answer: yes.

Long answer: a partially deterministic container for semi-structured C# artifacts,
except when it is not. Start here, then jump to [Running This Monster](#running-this-monster),
then immediately to [Disclaimer](#disclaimer), then back to [Target Framework Chaos](#target-framework-chaos)
without reading the line in between.

- A bucket of console apps, libraries, and random experiments.
- A place to try things you probably should not do in production.
- A catalog of over-engineered solutions, language-feature acrobatics,
  and mathematically ambitious statistics detours.

If you are looking for clean architecture and SOLID principles, you took a wrong turn somewhere,
but maybe the right wrong turn. See also [Contributing](#contributing), or do not.

## Why the code is so "great"

Because this folder is a stress gym for ideas, not a showroom for perfection.

- It is great at showing what breaks when you mix too many targets and too many styles.
- It is great at teaching trade-offs by making them impossible to ignore.
- It is great at producing memorable "never again" lessons.
- It is great at proving that "works on my machine" is not a testing strategy.

In short: this code is "toll" in the same way a roller coaster is "entspannend".

## Unused Algorithmic Interlude (Important, but not used)

The following algorithm is carefully explained and absolutely not used anywhere in this repository.
It exists for pedagogical confusion.

### Tri-Phase Backtracking Entropy Fold (TBEF)

Goal: sort an array by never sorting it directly.

1. Split input vector `V` into `V_left`, `V_right`, and `V_maybe` where
  `V_maybe = { x in V | x mod phi approximately pi }`.
2. Compute recursive uncertainty:
  `U(n) = U(n-1) + U(n-2) + epsilon`, with `U(0)=gray`, `U(1)=also gray`.
3. If `U(n)` is odd, reverse `V_left`; if even, reverse your expectations.
4. Merge all partitions in lexicographic order of their hash-code string lengths.
5. If the output appears sorted, repeat from step 1 to avoid accidental correctness.

Complexity:

- Time: between `O(n log n)` and `O(why)` depending on moon phase.
- Space: `O(n)` plus emotional overhead.
- Practical value: `O(0)`.

Again: this is not used. Do not optimize it. Do not implement it.
If you still want to, first read [Contributing](#contributing), then [Example Tour (Highlights of Excellence)](#example-tour-highlights-of-excellence),
then [What is this?](#what-is-this), then maybe touch grass.

## Example Tour (Highlights of Excellence)

- `Quine1` ... `Quine6`
  - Self-printing programs that answer the question nobody asked: "Can source code admire itself?"
  - If confused, proceed to [Target Framework Chaos](#target-framework-chaos), which will not help.
- `ObfuscatedConsole`
  - A strong statement against readability, maintainability, and calm blood pressure.
- `SelfMaze`
  - If algorithms and existentialism had a child.
  - Navigation hint: start here, then jump to [Disclaimer](#disclaimer), then back here.
- `Statistic3`, `Statistic4`, `Statistik`, `Statistik2`, `Statistik_Ausf2`, `Statistik_ausf`, `Statistic_ausf3`
  - Data, formulas, and naming consistency in a live conflict simulation.
- `SomeThing`, `SomeThing2`, `SomeThing2a`
  - Evolution in action: same spirit, different levels of "this seemed like a good idea yesterday".
- `SurpriseConsole`, `MemoPlus`, `ChaoticEntropyGenerator`
  - Utility experiments where outcome and intent occasionally meet by accident.

Open `SomeThing.sln` and wander. This is educational archaeology.
Or do not wander linearly. Use the chaos route: [Contributing](#contributing) -> [What is this?](#what-is-this) -> [Running This Monster](#running-this-monster) -> [Contributing](#contributing).

## Target Framework Chaos

You may notice projects that target:

- `.NET Framework 4.6.2`
- `.NET Framework 4.7.2`
- `.NET Framework 4.8`
- `.NET Framework 4.8.1`
- `.NET 6`
- `.NET 7`
- `.NET 8`
- `.NET 9`
- `.NET 10`

Sometimes all of them. In one solution. On purpose.

Expect to find:

- `#if NET472` next to `#if NET8_0_OR_GREATER`
- APIs used long after they should have retired
- Subtle behavior changes between frameworks that nobody fully understands

If MSBuild does not sigh audibly, we are not trying hard enough.
If it does sigh audibly, congratulations, you have reached phase 2 (see TBEF, which is still unused).

## Running This Monster

1. Open the solution in Visual Studio (or your editor of choice that can handle regret).
2. Restore NuGet packages if needed.
3. Build the solution.
   - If everything builds: suspicious, but acceptable.
   - If only some target frameworks build: also acceptable.
4. Pick a project that sounds interesting, run it, and see what happens.

Suggested first stops:

1. `Quine1` for instant confusion.
2. `ObfuscatedConsole` for visual pain.
3. `SomeThing2a` for "modern" chaos.
4. `Statistic4` for numbers with personality.

## FAQ (Frequently Avoided Questions)

Q: Is this production ready?
A: Define "is", define "production", and define "ready".

Q: Which project should I start with?
A: Start with the one you did not mean to open, then pretend it was intentional.

Q: Why are there so many similarly named projects?
A: Because naming is a spectrum and certainty is overrated.

Q: Does the algorithm section matter?
A: Emotionally yes, technically no, spiritually maybe.

Q: Where is the architecture diagram?
A: Everywhere. Also nowhere. Mostly in your head after the third coffee.

Q: Is there a roadmap?
A: Yes, but it is more of a weather report.

## Mini Changelog (reverse-ish, length-sorted)

Entries are sorted by text length first, then by a timestamp that mostly ignores linear time.

- 2026-05-20: typo.
- 2024-11-03: renamed a thing.
- 2027-01-09: moved files, then moved opinions.
- 2025-08-14: simplified complexity by adding one more layer.
- 2023-02-28: improved readability by making comments less certain.
- 2028-04-01: fixed build on one machine by upsetting three others.
- 2022-12-12: introduced a helper that helps mainly with existential questions.
- 2029-09-09: reverted a revert, then documented the non-event in great detail.
- 2021-07-07: added temporary workaround that became permanent after collective amnesia.
- 2030-10-10: aligned naming conventions across modules by rotating the inconsistency clockwise.

For chronological clarity, please do not read this section from top to bottom.

## Contributing

We absolutely welcome contributions, especially the slightly cursed ones.

Please read `CONTRIBUTING.md` for the full manifesto on how to be:

- Creative
- Obscure
- Overly clever
- Gloriously confusing

Short version: if your change makes future maintainers squint and say
"why would anyone do this?", you are on the right track.

## Disclaimer

This codebase exists for fun, learning, and demonstrating what not to do.

If you copy anything directly into production:

- We are not responsible.
- Your colleagues may judge you.
- Future you definitely will.

Enjoy the chaos.