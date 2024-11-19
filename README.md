# Hello Checkout-ers!

This is my solution for your take-home exercise.

It's fairly self-explanatory from the codes perspective, and I've added comments to explain any interfaces etc.

I think one interesting point to discuss is the POST method on the PaymentsController method, namely what happens when we receive a POST request from the Merchant, post it to the acquiring bank, and then fail to persist it to our payment repository, meaning that the merchant would have no way to retrieve a record of the transaction for later reconciliation. If this occurs, we should alert this for L1 support as critical, as the data store may be inconsistent and require additional intervention.

## Technical Considerations

### Async programming

PaymentRepository switched to Async/Await to facilitate move to remote data store.

### Abstractions

PaymentRepository and AcquiringBankService placed behind interfaces for testability and easier change of implementation

### Third-party libraries

Flurl used for HTTP interactions - provides a nice abstraction over HttpClient. Unit testing library changed to NUnit due to personal familiarity and experience.

### Monitoring

Added health check endpoint at /health

### API Design

Returning Rejected to the Merchant without giving additional feedback might not be the best experience for the merchant. Maybe it's worth considering wrapping any Rejected transactions with some description of what is wrong, especially if it is user-error.

## Test Commands

### Authorized Transaction

curl -X 'POST' \
  'https://localhost:7092/api/Payments' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "cardNumber": "2222405343248877",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "cvv": "123"
}'

### Declined Transaction

curl -X 'POST' \
  'https://localhost:7092/api/Payments' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "cardNumber": "2222405343248112",
  "expiryMonth": 1,
  "expiryYear": 2026,
  "currency": "USD",
  "amount": 60000,
  "cvv": "456"
}'
