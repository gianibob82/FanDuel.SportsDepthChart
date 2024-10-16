# FanDuel.SportsDepthChart

Run the app FanDuel.SportsDepthChart.Api and it will open a Swagger page to test the API.
The app uses InMemory db to store data. When you stop and rerun the app the memory is cleared.

The app has been structured following Onion Architecture https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture and CQRS using Mediatr
Majority of business logic inside FanDuel.SportsDepthChart\src\FanDuel.SportsDepthChart.Core\Chart\

Unit tests are in FanDuel.SportsDepthChart.UnitTests and cover all commands/queries

Regarding scaling the app, I've added tables to cover other sports and other teams.

 