# Architecture
Maria is divided into multiple projects for the purposes of segregating user interface from the working code. There are the Services project, which is the one that actually does things, the Common project, a library holding common code such as exceptions and communications interface, and various UI projects, that take and format commands from the user in one way or another and sends it to Services.  

## Common
The Common project exists to maintain the compatibility of the other projects while also possibilitating their compartimentalization. No two projects may reference each other, however, all C# projects must include Common and the non-c# must implement it in some way.  
It holds those things that must be the same across projects, most importantly the communications interfaces, templates and wrappers.  
It also holds some utilities, such as testers.

## Services
Services is Maria's beating hearth. An instance of Services is what serves the user. Within it there is the wide array of activities Maria does, each with their own namespace and treated as a Module in this documentation and in the command interface. Modules generally have Submodules, specifications under a general Module. As an example, the Tracking module has a Browser and a Process submodule.

## Tester
This project exists to automatically test Services. Its functionality is actually defined in Common, because all projects should be able to have handy means of testing, but it implements so in a standalone manner.

## CLI
A user interface in the command line. As of 0.2 it is dysfunctional, but that is due to other priorities, not abandonment. The idea is that CLI ends up not just a place for a user to manually write Command, but a parser of natural language into Command.

## Extension
The extension provides a interface for Maria right into the browser. It is the only way to keep track of the websites visited by the user.  
As of 0.2, it is the only project not written in C#. It is, in fact, being moved into Angular. This may seem like overkill, but it is planned that the extension will expose many features, somewhat like a web client of Maria. That may end up being a good idea, and some day Extension will be separated into a standalone monitoring extension and a web client installed and launched alongside Services. 