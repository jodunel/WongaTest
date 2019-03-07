Specification
=============

Create 2 dotnet core console applications
• They will need to use a library to communicate with a RabbitMQ instance (you can use any nuget
package)
• 1st console application needs to:
o Read an input string via Console.ReadLine() – “Name”
o Send a string message – “Hello my name is, {Name}” to the 2nd console application
• 2nd console application needs to:
o Listen for a string message
o Upon receiving a message, validate it to ensure the application can respond to the ‘Name’
received.
o Output to the console a response – “Hello {ReceivedName}, I am your father!”


Source code
===========


Build instructions
==================
Built using visual studio 2017

Open project WongaTest.sln
Build solution.

Alternatively run build.bat, MSBuild.exe must be available and in the path.
