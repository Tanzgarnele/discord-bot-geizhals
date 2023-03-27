# ManfredHorst Discord Bot

This Discord bot allows users to track product prices from Geizhals.de and manage alarms for price changes.

## Features

- Display current alarms
- Add or delete alarms
- Periodic price checks
- Notifications when prices drop below the alarm threshold

## Services

### LongRunningService

`LongRunningService` is responsible for periodically scanning the prices of products from Geizhals.de. When the price of a product drops below the user's alarm threshold, a notification is sent to the user.

### InteractionModule

`InteractionModule` handles user interactions and commands within the Discord bot. It provides the following functionality:

- `/show-alarms`: Display a list of the user's current alarms
- `/pricealarm`: Show current alarms and allow the user to add or delete alarms
- Add Alarm: Add a new alarm with a product URL, alias, and price threshold
- Delete Alarm: Delete an existing alarm by its alias

### InteractionHandler

`InteractionHandler` is a service that handles Discord interactions and commands within the Discord bot. It uses the `Discord.Addons.Hosting` and `Discord.Interactions` libraries to provide this functionality. The `InteractionHandler` class listens for interactions from users and executes the appropriate command based on the user's input. It also handles errors that may occur during the execution of a command, logging them and deleting the user's original response if necessary.

### GeizhalsScraper

`GeizhalsScraper` is a class that handles scraping product data from Geizhals.de. It uses the `AngleSharp.Html.Dom` and `AngleSharp.Html.Parser` libraries to parse the HTML data and extract relevant information such as product name, price, and thumbnail URL.

## License

This project is licensed under the terms of the MIT license. See the LICENSE.md file for details.
