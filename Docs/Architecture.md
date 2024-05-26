# Architecture
Maria is divided into multiple projects for the purposes of segregating user interface from the working code, as well as due to its multipurpose nature. There are the Services projects, which are the ones that actually do things, and each does one single thing, the Commons project, a library holding Commons code such as exceptions and communications interface, and Implementations, where Services are put together and combined with an user interface.  

## Commons
The Commons project exists to maintain the compatibility of the other projects while also possibilitating their compartimentalization. All C# projects must include Commons and the non-C# must implement it in some way.  
It holds those things that must be the same across projects, most importantly the communications interfaces, templates and wrappers.  
It also holds some utilities.

## Services
Services are Maria's beating hearth. A Services project is responsible for each major fuature of Maria, treated as Module in documentation and in the Command interface. Services are self-contained, are all class libraries and cannot reference another project, except Commons. As such, they are only useful when implemented.

## Implementations
The Implementations are those projects that use Services in wathever shape or form. As a general rule an Implementation does not reference another, but as of 0.6 the Readers razor library is to be referenced by the NovelReader and future web ui's.

### Tester
As the name indicates, this is the one which will be run to test the Services functionalities.

### CLI
A user interface in the command line. As of 0.2 it is dysfunctional, but that is due to other priorities, not abandonment. The idea is that CLI ends up not just a place for a user to manually write Command, but a parser of natural language into Command.

### Readers and NovelReader
The first is a razor library holding the the reading components themselvels, the second is a blazor-maui hybrid that currently is the only one using it. NovelReader is intended to read and translate (primarily) japanese light novels in a way that is offline and convenient in Android. At first Kavita was used for that purpose, but the author has demand for a customized, Android-fodcused solution.
The Readers is a separate project is to allow it to be used in other possible web interfaces.

### Extension
The extension provides a interface for Maria right into the browser. It is the only way to keep track of the websites visited by the user.  
As of 0.2, it is the only project not written in C#. It is, in fact, being moved into Angular. This may seem like overkill, but it is planned that the extension will expose many features, somewhat like a web client of Maria. That may end up being a good idea, and some day Extension will be separated into a standalone monitoring extension and a web client installed and launched alongside Services. 