using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Flour.BlobStorage.Contracts
{
    public class Blob : IDisposable
    {
        private int _disposeCounter;

        public Stream Stream { get; }
        public string ContentType { get; }
        public IDictionary<string, string> Metadata { get; }

        public Blob(Stream stream, string contentType, IDictionary<string, string> metadata)
        {
            Stream = stream;
            ContentType = contentType;
            Metadata = metadata;
        }

        public Blob() : this(Stream.Null, string.Empty, new Dictionary<string, string>())
        { }

        public virtual void Dispose()
        {
            if (Interlocked.Increment(ref _disposeCounter) != 1)
                return;
            Stream?.Dispose();
        }
    }
}
