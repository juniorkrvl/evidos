# Assignment

1. Perform a full code review on the provided solution and determine if any changes are needed. Make sure to indicate in your comments what you think must be fixed, what you think should be fixed (e.g. could be better done differently) and what you would personally change because of personal preference.
2. Pick one piece you want to improve upon. Make the code changes and send them back to us.

# Provided solution
See the attachment for `Assignment.tar.gz` file which contains a git repo. This git repo is written by a previous candidate, who provided is as the solution to belowassignment. We use his solution with his permission. Use belowassignmentinfo we originally sent to him to perform above 2 requirements (to review his code and pick and improve one piece).

## Assignment

### Background info
A user should be able to register using an email address. In the future, the user should be able to login on the system using this email address and a password.
On the platform there is an EventStore and an SQL database, these are 2 separate products. They can both be reached over the network.
A dotnet library has to be created and the library should be usable from an ASP.net core MVC website as well as from a .NET framework 4.8 console application. The library should be built on the principle of EventSourcing. Use git for version control, the main programming language should be C#. We prefer quality over quantity.

### EventStore

Assume an EventStore is available and reachable over the network which has the following properties:
- able to retrieve all events for a given key
- able to add one or more events for a given key
Define an interface in the dotnet library with the given properties which you will be able to use later.

### User lifecycle

Add the following functionality to the dotnet library using the previous declared interface:

- create a user using an email address
- a user should be able to change its email address, we want the email address to be validated first before the change is fully propegated within the system
- delete a user, any (PII) data we have from the user should be removed within 30 days within the system.
All email address should be unique with the system.
The actual verification of the email address is out of scope of this assignment. However, the library should have the necessary entry points to mark an email address as validated. Email address validation should validate that the user really has access - or is the owner of - the email address.

### User interface
Create a UI (this can also be a console application) to enable searching a user. Input should be an email address or a part of the email address.