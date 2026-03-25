# CONTRIBUTING.md
_"Abandon all best practices, ye who enter here."_

Welcome to this glorious dumpster fire of C# experiments.
This repository is a **how-to-do-it-NOT** collection. If your code would make your future self cry: you are home.

## Prime Directive

Your mission, should you choose to accept it:

> Be as creative, obscure, confusing, over-engineered, and unnecessary as possible –
> while still compiling. Most of the time.

If your solution is clean, elegant, or easily testable… please reconsider your life choices and add at least three layers of indirection.

## Golden Rules

- **Rule 1 – Readability is optional**  
  If someone understands your code in under 5 minutes, you’re doing it wrong.

- **Rule 2 – Patterns everywhere**  
  Use design patterns even when they make no sense.  
  `FactoryOfAbstractSingletonAdapterVisitorFacade`? Perfect.

- **Rule 3 – Overuse language features**  
  LINQ on LINQ on LINQ, reflection where a simple `if` would do, async everywhere, dynamic anywhere.

- **Rule 4 – Comments are misdirection**  
  If you must comment, do it cryptically:  
  `// TODO: This shouldn’t work, but it does. Don’t touch.`

- **Rule 5 – Generic everything**  
  When in doubt, make it generic.  
  When not in doubt, make it generic anyway.

- **Rule 6 – Architecture by chaos**  
  Circular dependencies, questionable layering, cross-project references that surprise everyone – especially MSBuild.

- **Rule 7 – Multi-targeting madness**  
  The more `TargetFrameworks`, the better.  
  Prefer `net462;net472;net48;net481;net6.0;net7.0;net8.0;net9.0;net10.0` in one project, sprinkled with `#if NET472`, `#if NET8_0_OR_GREATER`, and one `#else` that does something completely different.

## How to Contribute

1. **Fork the repo**  
   This gives you a safe place to commit your crimes.

2. **Create a branch**  
   Name it something “helpful”, like `fix-stuff`, `test`, or `tmp2-final-new`.

3. **Add your masterpiece**  
   - New project that does almost the same as an existing one? Excellent.  
   - Same logic, different style, more confusion? Even better.  
   - Abuse of multi-targeting and `#if` directives? Chef’s kiss.

4. **Break expectations, not the build (too much)**  
   Ideally it still builds for at least one target framework.  
   If not, leave a note in a comment explaining that it’s “by design”.

5. **Open a Pull Request**  
   - PR title: vaguely misleading.  
   - Description: just enough info to be dangerous.  
   - Screenshots of console output no one asked for: encouraged.

## Style (or the lack thereof)

- Tabs vs spaces? **Yes.**
- Naming:  
  - Mix German, English, abbreviations, and one random word you made up.  
  - Booleans that don’t sound boolean: `numberIsChaos`, `maybeResult`, `done2`.
- Exceptions for control flow? Why not.
- “Helper” classes named `Util`, `Helper`, `Stuff`, or `BaseSomething`: approved.

## Tests

Unit tests are welcome if:

- They are as confusing as the code.  
- They only pass under specific, undocumented conditions.  
- They depend on timing, locale, or number of CPU cores.

Clean, robust tests are allowed only if they test something truly unhinged.

## Documentation

If you write documentation, please ensure:

- It is at least partially out of date.  
- It references features that no longer exist.  
- It uses diagrams that explain less than the code.

## Final Words

This is a playground, not a production system.  
Add things that make you think:

> "Wow, I hope nobody ever copies this into real code."

If you’re having fun and your code makes you slightly uncomfortable, you’re contributing correctly.
