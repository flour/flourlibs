# Some food pet project + common libraries

Dev

## Libraries

### Flour.Commons ![Commons](https://github.com/flour/flourlibs/workflows/Commons/badge.svg)

Contains a set of helpful extensions that simplifies service configuring 

### Flour.Logging ![Logging](https://github.com/flour/flourlibs/workflows/Logging/badge.svg)

Set of common loggers that can be used while creating new service based on Serilog

### Flour.CQRS ![CQRS](https://github.com/flour/flourlibs/workflows/CQRS/badge.svg)

Simplified implementation of CQRS approach

### Flour.BrokersContracts ![BrokersContracts](https://github.com/flour/flourlibs/workflows/BrokersContracts/badge.svg)

Common contracts to work with message brokers, e.g. RabbitMQ

## Notes

### Flour.Logging

To use Graylog with normal output (using UDP) create GELF + UDP input, but not just plain text + UDP
https://github.com/Graylog2/graylog2-server/issues/1431
