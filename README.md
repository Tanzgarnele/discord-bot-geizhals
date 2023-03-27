# ManfredHorst Discord Bot

This Discord bot allows users to track product prices from Geizhals.de and manage alarms for price changes.

## Features

- Display current alarms
- Add or delete alarms
- Periodic price checks
- Notifications when prices drop below the alarm threshold

## Services

### LongRunningService

`LongRunningService` is responsible for periodically scanning the prices of products from Geizhals.de. When the price of a product drops below the user's alarm threshold, a notification is sent to the user. The service also manages alarms, allowing users to add, delete, and update them.

### InteractionModule

`InteractionModule` handles user interactions and commands within the Discord bot. It provides the following functionality:

- `/show-alarms`: Display a list of the user's current alarms
- `/pricealarm`: Show current alarms and allow the user to add or delete alarms
- Add Alarm: Add a new alarm with a product URL, alias, and price threshold
- Delete Alarm: Delete an existing alarm by its alias
