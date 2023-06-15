# Event Sourced User Management

## Introduction

In response to the requirements presented in the assignment, I've primarily focused on addressing the issues related to user private data and email uniqueness.

To manage the concerns around user private data, I propose the use of data tokenization. This is a technique commonly used in applications that need to comply with regulations like PCI-DSS, where sensitive information like credit card numbers can't be freely shared across services. 

## Proposed Solution For GDPR/Data Privacy

I have created a representation of this as a repository (`IUserPrivateDataRepository`), and I use this interface to save the user private data. This repository could also be used to store the user login credentials in the future.

In my implementation the `IUserPrivateDataRepository` is a InMemory representation but it could well be another service completely, the only caveat in it being a separate service is that it could bring an eventual consistency, which I didn't want in this scenario. 

I wanted the user data to be saved before dispatching any event saying that something changed in the user.

The advantage with this approach is that we can easily remove all the private data from the repository without having to touch the events.

In this scenario, the events only carry simple information such as the `UserId` and the `DateTime`.

The `Token` for the data I used is the same `UserId`, I did this to simplify but it could well be also a different token for every data change.

## Email Uniqueness

Since we delegated the privacy of the user to this repository, we can rely on it to also validate that the user with a specific email is already in the system or not. In a real-world scenario I would create a relational table to store this information, and apply constraints in the email column to guarantee uniqueness for the `Email` and primary key for the `UserId`.

## Aggregate

Even though I make use of the event store and save all the events, I don't think in this scenario the UserAggregate is very useful. There is no specific rule that the UserAggregate guards. In my opinion, the user aggregate needed more business logic and validation to justify its existence, however I still kept it in the code and the only event that is interesting for the UserAggregate is the `UserCreated` event.

## Use Cases (Commands and Queries)

Based on the requirements I have mapped the following use cases:

### **CreateUserCommand**
The use case that creates a new user, makes use of the `IUserPrivateDataRepository` to check email uniqueness. This command is the only one I use the UserAggregate.

### **DeleteUserCommand**  
The delete user usercase is used to remove the private data from the system, this command does not delete any events.

### **RequestEmailChangeCommand**  
When a user wants to update the email, we need first to validate if the user is owner of the email. For this I implemented a repository for pending email change requests (`IPendingEmailChangeRepository`). 
The idea with this implementation is to create a token for the change request, send the token as a link to the user new email and use the token as a confirmation that the user is the one that requested the email change in the first place. 

### **ConfirmEmailChangeCommand**  
This use cases is where the user email gets changed, it receives as parameter the `EmailChangeToken`. If the token is valid, the user email is updated in the `IUserPrivateDataRepository` and an `UserEmailUpdated` is dispatched with the `UserId` and the `DateTime` of the event.

### **GetUserByEmailQuery**
This query makes use of the `IUserProjectionRepository` to retrieve the data from the user. I will explain later in this document how the projection is built.

### **SearchUserByEmailQuery**  
This usecase is similar with the `GetUserByEmail` the only difference is that it returns a list of all the users which the email contains the partial email query.

### **GetPendingEmailChangeRequestsQuery**  
I implemented this usecase only for demo purposes, to show the pending email changes list in the CLI application

## Events

Even though I transfered all the private data to a repository it does not mean I am not using events, I dispatch and save all the events of the system. The events are:

- UserCreated
- UserDeleted
- EmailChangeRequested
- UserEmailUpdated

None of the events transfer user private data, they only transfer the token that identifies the data, which in this case is the `UserId`.

I implemented a class called `UserEventLifecycle` where I handle all the events for a User. In this class I create the projections based on the events, so in my implementation it is important that the data is saved in the `IUserPrivateRepository` before the event arrives.

Whenever a `UserCreated` or `UserEmailUpdated` event is dispatched, the `UserEventLifecycle` takes care of creating or updating a user projection that is represented by the model `UserViewModel` object. 

The data is then stored in the `IUserProjectionRepository` and used by the queries to retrieve the user current state.

## Tests

I have created integration tests for all use cases described here as well as the event handlers. I have implemented InMemory concrete classes for all the repositories to make the testing easier. 

Given more time, I would have liked to expand the unit test coverage for all classes.

## Console Application

I have created a CLI application to demonstrate how to interact with the commands and queries. A Web API could easily be added in the solution. By importing the ICommandBus and IQueryBus, we can easily dispatch the commands and queries directly from a controller.

## Demo

![](./evidos-assignment.gif)

