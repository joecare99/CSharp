# SomeThing – A C# "How Not To" Playground

Welcome to `SomeThing`, a lovingly curated collection of **questionable** C# ideas.

This is not a best practices guide. This is the pile of experiments you point at and say:

> "Kids, don’t do this at home."

Yet somehow, it (mostly) builds.

## What is this?

- A bucket of console apps, libraries, and random experiments.
- A place to try things you probably **shouldn’t** do in production.
- A catalog of:
  - Over-engineered solutions
  - Creative misuse of language features
  - Multi-targeting that went too far
  - Statistics projects that may or may not be mathematically sound
  - Quines, obfuscation, and general weirdness

If you’re looking for clean architecture and SOLID principles, you took a wrong turn somewhere.

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

If MSBuild doesn’t sigh audibly, we’re not trying hard enough.

## Projects You May Stumble Over

- `SomeThing` – Core chaos, in many target flavors.
- `SomeThing2` / `SomeThing2a` – Variants that may or may not be improvements.
- `Statistic*` / `Statistik*` – Math, but make it confusing.
- `Quine1`–`Quine4` – Programs that print themselves, ideally in the most roundabout way.
- `ObfuscatedConsole` – Because why should console output be readable?
- `BaseLib`, `BaseLibTests`, `BaseLib.Show` – A base for… something. Details unclear.
- `ConsoleLib`, `ConsoleDisplay` – For when writing directly to the console felt too straightforward.

Feel free to open the solution and just… wander.

## Running This Monster

1. Open the solution in Visual Studio (or your editor of choice that can handle regret).
2. Restore NuGet packages if needed.
3. Build the solution.
   - If everything builds: suspicious, but acceptable.
   - If only some target frameworks build: also acceptable.
4. Pick a project that sounds interesting, run it, and see what happens.

## Contributing

We absolutely welcome contributions – especially the slightly cursed ones.

Please read `CONTRIBUTING.md` for the **full manifesto** on how to be:

- Creative
- Obscure
- Overly clever
- Gloriously confusing

Short version: if your change makes future maintainers squint and say "why would anyone do this?", you’re on the right track.

## Disclaimer

This codebase exists for fun, learning, and demonstrating what *not* to do.

If you copy anything directly into production:

- We’re not responsible.
- Your colleagues may judge you.
- Future you definitely will.

Enjoy the chaos.