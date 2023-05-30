# Pattern Tutorial
Heavily inspired by [42_Entwickler](https://www.youtube.com/@42Entwickler)

## Lession 1: The Singleton
Instead of global variables you can use the singleton design pattern.
This pattern gives the ability to creare one and only on Instance of a class.
So you have both, all advantages of a class, and the assurance to always work with the same Instance.
In this example: the [UserContext]()-Class implements the singleton pattern.

## Lession 2: The Observer
To get the change of a value there are two possible ways to get this done:
* The class can regularly look/ask for the value. This is called *Polling*
* The class can register itself to get notified when the value is changed. This class now is an *Observer* to that value.
The class that contains/emits the value is called *Subject*.

 