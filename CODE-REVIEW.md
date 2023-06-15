# Code Review

I've had a chance to review the code for the assignment and I wanted to share some observations. Although it might seem simple at first glance, there are elements that make this a challenging problem to solve.

- The user data removal requirement presents a significant challenge. Since events serve as the source of truth, events cannot be simply deleted. Moreover, it's not feasible to have all events carry user private data indefinitely.
- The requirement for email uniqueness also poses a challenge. When creating a new user, it's not as simple as scanning all the `UserCreated` events to check for a specific email or an email update.

Given these complexities, I would suggest a different approach to the code.

## Essential Improvements

- The requirement to delete user private data, like an email, is not clearly addressed in the code. There are numerous techniques that could be applied to address this issue within the event sourcing approach.
- The `CreateUserCommandHandler` currently does not check for email uniqueness. This is a crucial check that needs to be implemented.
- There seem to be missing use cases that were described in the assignment, such as a command to delete user data.
- There is no evident implementation of the use case where the user receives an email notification to confirm an email change.
- The test coverage could be expanded to include integration tests for all use cases. At present, only `UserAggregateTest.cs` is available.
- The assignment suggests a UI to interact with the use cases, which seems to be missing in the current implementation.
- The queries defined in the assignment, such as `GetUserByEmail` and `SearchUserByEmail`, also seem to be missing. These could be useful to retrieve user data from the projections to the UI.

## Recommended Improvements

- It would be beneficial to see interfaces for the query bus. This would enhance the code's modularity and testability.
- The command `UpdateUserEmailAddressCommand` could be confusing as it's a request to update the email, not the actual update. Consider renaming it for clarity.
- The use of async/await could be leveraged to run parts of the code asynchronously. Implementing Task-based methods with async and await would be an improvement.
- The inclusion of business exceptions to describe business validations could greatly assist in debugging, especially when differentiating between business errors and runtime errors.

## Personal Preferences

- My approach for the private data storage and email uniqueness check would be different.
- I would organize the code into features, so it is easier to navigate and maintain the code
- I would implement more models to represent each aspect of the system.
- I would implement the usecases such as `CreateUser`, `DeleteUser`, `RequestEmailChange`, `ConfirmEmailChange`, `GetUserByEmail`, `SearchUserByEmail`, `GetEmailPendingChanges`
- I would implement business validation specific exceptions such as `CannotChangeEmail` or `CannotCreateUser` to differentiate between business logic validation and other system errors
- I would implement InMemory repositories for testing
- Full integration testing testing all the possible flows for each use case
- Implement a console application that interfaces with the Commands and Queries I expose.

I have implemented my own version of the assignment, please check the README.md file on this repository for more information.
