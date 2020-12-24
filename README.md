# Flour common libraries

This repo is a set of helper libraries (SDK if we can say so) that can be used independently to help developer to start building their web apps and microservices based on .NET faster. This set of libraries provides utility extensions to tackle challenges such as logging, distributed tracing and more, but in plans.

## Libraries ![Travis](https://travis-ci.com/flour/flourlibs.svg?branch=master) ![AllInOne](https://github.com/flour/flourlibs/workflows/AllInOne/badge.svg)

## Flour.Commons ![NuGet](https://img.shields.io/nuget/v/Flour.Commons.svg?style=flat)

Contains a set of helpful extensions that simplifies service configuring.

## Flour.Logging ![NuGet](https://img.shields.io/nuget/v/Flour.Logging.svg?style=flat)

Set of common loggers that can be used while creating new service based on Serilog. At the moment supports Console, File, Seq, Elastic Search, Graylog.

## Flour.BrokersContracts ![NuGet](https://img.shields.io/nuget/v/Flour.BrokersContracts.svg?style=flat)

Common contracts to work with message brokers, e.g. RabbitMQ.

## Flour.CQRS ![NuGet](https://img.shields.io/nuget/v/Flour.Logging.svg?style=flat)

Simplified implementation of CQRS.

## Flour.CQRS.Brokers ![NuGet](https://img.shields.io/nuget/v/Flour.CQRS.Brokers.svg?style=flat)

General contracts for event publishing 

## Flour.BlobStorage.Contracts ![NuGet](https://img.shields.io/nuget/v/Flour.BlobStorage.Contracts.svg?style=flat)

A blob storage interfaces to manupulate blob data.
This library exposes IBlobStorageReader, IBlobStorageWriter and IBlobStorageDeleter interfaces, which can be used with one of the implemented blob storage libraries to read, write and delete blobs to supported blob storage.

## Flour.BlobStorage.AmazonS3 ![NuGet](https://img.shields.io/nuget/v/Flour.BlobStorage.AmazonS3.svg?style=flat)

The Amazon S3 implementation uses the AWS SDK for .NET to interact with the Amazon S3 API and other similar APIs like MinIO.

## Flour.RabbitMQ ![NuGet](https://img.shields.io/nuget/v/Flour.RabbitMQ.svg?style=flat)

Wrapper around RabbitMQ to publish domain events

## Flour.IOBox ![NuGet](https://img.shields.io/nuget/v/Flour.IOBox.svg?style=flat)

Outbox pattern contracts and in-memory implementation 

## Flour.Tracing.Jaeger ![NuGet](https://img.shields.io/nuget/v/Flour.Tracing.Jaeger.svg?style=flat)

**WIP**

## Notes

### Flour.Logging

To use Graylog with normal output (using UDP) create GELF + UDP input, but not just plain text + UDP
https://github.com/Graylog2/graylog2-server/issues/1431
