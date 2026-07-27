# Architecture roadmap

The main goal of the app is bring all business features at the lower possible cost.

## Changes to do

The main idea is to have a monolith but different API projects in a way that we can host the monolith in the begining and in future (when user base allow it) run the APIs be theyself.
For local testing and development and in order to make sure each API can run by theyself we can have a configuration that will deside if we run each API or just the main one with all include in it.

- Split the monolith in different api for each domain.
- Create common API project to have all needed configuration to share across APIs.
- Create configuration to determine monolith/microservice
- Make API endpoints auto-discoverables????