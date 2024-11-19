# Hello Checkout!

This is my solution for your take-home exercise.

It's fairly self-explanatory, and I've added comments to explain any interfaces etc.

## Technical Considerations

### Distributed transaction considerations

I think one interesting point to discuss is the POST method on the PaymentsController method, namely what happens when we receive a POST request from the Merchant, post it to the acquiring bank, and then fail to persist it to our payment repository, meaning that the merchant would have no way to retrieve a record of the transaction for later reconciliation. If this occurs, we should alert this for L1 support as critical, as the data store may be inconsistent and require additional intervention. An option might be that we try and rollback the acquiring bank transaction if we fail to persist it, but this could potentially make things worse.

### Async programming

PaymentRepository switched to Async/Await to facilitate move to remote data store.

### Abstractions

PaymentRepository and AcquiringBankService placed behind interfaces for testability and easier change of implementation

### Third-party libraries

Flurl used for HTTP interactions - provides a nice abstraction over HttpClient. Unit testing library changed to NUnit due to personal familiarity and experience. Use of Moq to serve as AcquiringBank service in testing without docker.

### Monitoring

Added health check endpoint at /health

### API design

Returning Rejected to the Merchant without giving additional feedback might not be the best experience for the merchant. Maybe it's worth considering wrapping any Rejected transactions with some description of what is wrong, especially if it is user-error.

## Test commands for Postman

Attached in additional file PostmanTests.json for Postman with some tests for the responses for Authorized, Declined, and Rejected transactions.
