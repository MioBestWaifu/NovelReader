# Commands
Commands are the main way for Maria's interfaces to request her Services. The primary purpose of the various user interfaces is to generate commands, a machine-oriented scheme, in a human-friendly fashion. Maria's Services always has a CommandServer, a wrapper HTTP server, listening on port 47100 when running and all commands are handled by it.

## SYNTAX

Commands are structured as follows: action (required) module (required) submodule (may be optional or required), options (dictionary);
An example would be: start (action) tracking (module) browser (submodule), options: [{end,1600},{ignore,youtube.com}];

Module and submodule are explained (or will be at a later date) in the Services doc. Action is what to do and options are various modfiers, conditions and payloads. Some options are not so optional, however, and the Command API should be consulted on what "options" a given CommandHandler expects.

## Multiple Commands

As of 0.2, Maria's components send and receive only one Command at a time, and Commands are  treated in isolation. However, it is planned that Services receives a batch of Commands at once, and that relations between those Commands be established.