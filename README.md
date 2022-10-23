# hungarian_notation_hunter_PT
A prototype tool to hunt down instances of hungarian notation in your software.

Encoding the type of an object into its name is, in my opinion, meaningless noise.
I understand that having a simple "m" or "s" prefix for indicating member or static
can be genuinely useful. It's valuable knowing the scope of a variable you're looking
at. However, what's LESS useful is litering type identifiers everywhere. You get
nothing out of it and it just starts making everything blend together into a meaningless
soup of encoded meanings.

This tool is something I made to try to wrestle back control from this issue. It will
attempt to use a regex expression to find variable declarations which match this pattern.
Once you clean up its search results, it will help you find every declaration of hungarian
notation in your chosen folder and you can start cleaning it up.

Again, this is just a prototype. It works, but I may make a better refactoring tool later.
