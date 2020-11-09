﻿using System;
using System.Text;
using Flour.BlobStorage.Contracts;
using Newtonsoft.Json;

namespace Flour.BlobStorage.AmazonS3
{
    public class AmazonS3BlobReference : IBlobReference
    {
        private readonly BucketKey _bucketKeyReference;

        public AmazonS3BlobReference(string bucket, string key)
        {
            if (string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentNullException(nameof(bucket));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _bucketKeyReference = new BucketKey(bucket, key);
        }

        public AmazonS3BlobReference(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            _bucketKeyReference = FromId(id);
        }

        public string Id => ToId();
        public string Bucket => _bucketKeyReference.Bucket;
        public string Key => _bucketKeyReference.Key;

        private string ToId()
        {
            var serializedJson = JsonConvert.SerializeObject(_bucketKeyReference);
            var bytes = Encoding.UTF8.GetBytes(serializedJson);
            return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        }

        private BucketKey FromId(string id)
        {
            var bytes = Convert.FromBase64String(id);
            var serializedJson = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<BucketKey>(serializedJson);
        }

        private class BucketKey
        {
            public BucketKey() { }

            public BucketKey(string bucket, string key)
            {
                Bucket = bucket.ToLowerInvariant();
                Key = key.ToLowerInvariant();
            }

            public string Bucket { get; set; }
            public string Key { get; set; }
        }
    }
}