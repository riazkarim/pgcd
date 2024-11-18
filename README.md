# Hello Checkout-ers!

This is my solution for your take-home exercise.

It's fairly self-explanatory from the codes perspective, and I've added XML comments to explain any interfaces etc.

I think one interesting point to discuss is the POST method on the PaymentsController method, namely what happens when we receive a POST request from the Merchant, post it to the acquiring bank, and then fail to persist it to our payment repository, meaning that the merchant would have no way to retrieve a record of the transaction for later reconciliation.
In this case, we should alert this for L1 support as critical, as the data store may be inconsistent and require manual intervention to resolve.

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
