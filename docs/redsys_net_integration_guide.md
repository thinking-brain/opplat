# Building a Subscription Engine with Redsys in .NET

When bypassing major gateways like Stripe to save on transaction fees, you must build the subscription engine yourself. Redsys does not manage subscriptions; it only processes the charges you explicitly send it. 

To achieve recurring billing, you must implement **Credential on File (COF)** tokenization.

## 1. Prerequisites & Tooling

*   **Redsys Merchant Account:** You need a TPV Virtual from a Spanish bank configured for REST/Server-to-Server (MIT) operations and Tokenization.
*   **NuGet Package:** Do not write the 3DES/MAC256 cryptography from scratch. Use a community package like `RedsysTPV` or `Redsys.Merchant`.
*   **Background Scheduler:** You will need a library like **Hangfire** or **Quartz.NET** running in your .NET backend to trigger daily billing cycles.

---

## 2. Database Schema Requirements

To charge a user in the future, you must store specific identifiers returned by Redsys during the initial payment. Your `Subscriptions` or `PaymentMethods` table needs at least these fields:

*   `UserId` (string/guid): Your internal user ID.
*   `SubscriptionPlanId` (int): E.g., 1 for Basic, 2 for Pro.
*   `NextBillingDate` (datetime): The exact date the cron job should charge this user next.
*   `RedsysToken` (string): The `Ds_Merchant_Identifier` returned by the webhook. This is the alias for the credit card.
*   `RedsysLastTxnId` (string): The `Ds_Merchant_COF_TXNID` from the *previous* successful charge. You must pass this into the *next* charge.
*   `Status` (enum): Active, PastDue, Canceled.

---

## 3. Phase 1: The Initial Setup Payment (CIT)

The user must perform a **Customer Initiated Transaction (CIT)** to authorize the card via 3D Secure. 

When the user clicks "Subscribe" in your frontend, your .NET backend generates a payment form/redirect payload. Crucially, you must include the COF (Credential on File) parameters to request a token:

*   `Ds_Merchant_Identifier`: `"REQUIRED"` (Tells Redsys to generate a token).
*   `Ds_Merchant_COF_INI`: `"S"` (Indicates this is the initial setup).
*   `Ds_Merchant_COF_TYPE`: `"R"` (Recurring type).

The user is redirected to the Redsys hosted page, completes the 3D Secure challenge with their bank app, and is redirected back to your "Success" page.

---

## 4. Phase 2: The Webhook Receiver (Notification)

Redsys will send an asynchronous HTTP POST request to your server (the `Ds_Merchant_MerchantURL`) when the payment succeeds or fails. 

**Your .NET API Endpoint (`[HttpPost] /api/webhooks/redsys`) must:**
1.  **Validate the Signature:** Use your NuGet package and your Redsys Secret Key to verify `Ds_Signature`.
2.  **Check the Status:** Ensure the response code (`Ds_Response`) falls in the success range (0000 - 0099).
3.  **Extract COF Data:** Read the `Ds_Merchant_Identifier` (The Token) and `Ds_Merchant_COF_TXNID` (The Transaction ID).
4.  **Save to Database:** Update the user's subscription record with the Token, TXNID, and set the `NextBillingDate` to 30 days from now (or 1 year, etc.).

---

## 5. Phase 3: The Recurring Background Job (MIT)

This is the engine you must build to replace Stripe Billing. 

Every night, your Hangfire/Quartz background job will:
1.  Query the database for all `Subscriptions` where `Status == Active` AND `NextBillingDate <= DateTime.UtcNow`.
2.  Loop through each due subscription.
3.  Make a **REST API Call (Server-to-Server)** directly to the Redsys endpoint. You do not redirect the user; this happens silently in the background.

**The MIT Payload must include:**
*   `Ds_Merchant_Identifier`: *The saved RedsysToken from your DB.*
*   `Ds_Merchant_COF_TXNID`: *The saved RedsysLastTxnId from your DB.*
*   `Ds_Merchant_COF_INI`: `"N"` (Not the initial setup).
*   `Ds_Merchant_COF_TYPE`: `"R"` (Recurring).

If the REST call returns a success code (0000-0099), you update the `NextBillingDate` in your database to the next cycle and update the `RedsysLastTxnId` with the new transaction ID returned from this current charge.

---

## 6. Handling Edge Cases

Because you own the billing engine, you must write logic for:
*   **Failed Payments (Dunning):** If the background MIT charge fails (e.g., insufficient funds, card expired), you must set the subscription to `PastDue`, revoke the user's access, and send them an email to update their card.
*   **Updating Cards:** If a user wants to change their card, you must run them through Phase 1 (CIT) again to generate a *new* token, and replace the old token in your database.
*   **Proration:** If a user upgrades mid-month, your backend logic must calculate the math of what they owe, charge them immediately via REST (MIT), and adjust their `NextBillingDate`.