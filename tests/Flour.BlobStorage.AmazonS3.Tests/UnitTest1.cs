using FluentAssertions;
using Xunit;

namespace Flour.BlobStorage.AmazonS3.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("bucket", "key")]
    public void When_Then(string bucket, string key)
    {
        var id = new AmazonS3BlobReference(bucket, key).Id;
        var test = new AmazonS3BlobReference(id);
        test.Bucket.Should().Be(bucket);
        test.Key.Should().Be(key);
    }
}