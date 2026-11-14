# AA98 Command Runner

The standalone host starts a shell process and sends complete input lines over redirected standard input. Standard output and standard error are displayed separately, including partial output from commands that have not printed a newline.

This is a **line-oriented command runner, not a terminal emulator**. It does not allocate a pseudo-terminal (PTY). Interactive shell prompts, job control, terminal modes, curses/full-screen programs, and many interactive REPLs are unsupported. Pressing Enter in the console input or choosing **Run line** submits the entire current line to the shell.

The process/session abstractions live in `AA98_AvlnCodeStudio.Base.OS`; this host supplies the redirected-process implementation. A future PTY implementation would need an explicit capability/input contract rather than assuming that this host's line-oriented behavior provides terminal semantics.
