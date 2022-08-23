using System.Net;
using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Flour.BlobStorage.Contracts;
using Microsoft.Extensions.Logging;

namespace Flour.BlobStorage.AmazonS3;

internal class AmazonS3BlobStorageReader : IBlobStorageReader<BucketKeyReference>
{
    private readonly IAmazonS3 _client;
    private readonly ILogger<AmazonS3BlobStorageReader> _logger;

    public AmazonS3BlobStorageReader(IAmazonS3 client, ILogger<AmazonS3BlobStorageReader> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Blob> Get(BucketKeyReference reference)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = reference.Bucket,
                Key = reference.Key
            };

            using var response = await _client.GetObjectAsync(request);
            await using var responseStream = response.ResponseStream;
            var memoryStream = new MemoryStream();

            await responseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var metadata = response.Metadata.Keys
                .Select(key => new KeyValuePair<string, string>(key, response.Metadata[key]))
                .ToDictionary(x => x.Key.Replace("x-amz-meta-", string.Empty),
                    x => x.Value);

            return new Blob(memoryStream, response.Headers.ContentType, metadata);
        }
        catch (AmazonS3Exception exception)
        {
            HandleNotFound(reference.Bucket, reference.Key, exception);
            throw new BlobStorageException(
                $"An exception occurred while getting blob with key \"{reference.Key}\" from bucket \"{reference.Bucket}\"",
                exception);
        }
    }

    public async Task<IDictionary<string, string>> GetMetadata(BucketKeyReference reference)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = reference.Bucket,
                Key = reference.Key
            };

            var response = await _client.GetObjectMetadataAsync(request).ConfigureAwait(false);
            return response.Metadata.Keys.Select(e => new { Key = e, Value = response.Metadata[e] })
                .ToDictionary(e => e.Key, e => e.Value);
        }
        catch (AmazonS3Exception exception)
        {
            HandleNotFound(reference.Bucket, reference.Key, exception);
            throw new BlobStorageException(
                $"An exception occurred while getting metadata with key \"{reference.Key}\" from bucket \"{reference.Bucket}\"",
                exception);
        }
    }

    public async Task<bool> Exists(BucketKeyReference reference)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = reference.Bucket,
                Key = reference.Key
            };

            return await _client.GetObjectMetadataAsync(request) != null;
        }
        catch (AmazonS3Exception exception)
        {
            if (exception.InnerException is HttpErrorResponseException httpError)
                return httpError.Response.StatusCode != HttpStatusCode.NotFound;

            throw new BlobStorageException(
                $"An exception occurred while checking whether blob with key \"{reference.Key}\" in bucket \"{reference.Bucket}\" exists",
                exception);
        }
    }

    private void HandleNotFound(string bucket, string key, AmazonS3Exception exception)
    {
        var httpError = exception.InnerException as HttpErrorResponseException;
        if (httpError?.Response.StatusCode == HttpStatusCode.NotFound)
        {
            var message =
                $"{key} is missing from the {bucket} bucket in Amazon 23 store. It has either expired or an invalid id has been supplied";
            _logger.LogWarning(httpError, message);
            throw new BlobNotFoundException(message, exception);
        }
    }
}