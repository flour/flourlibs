﻿using System.Collections.Generic;

namespace Flour.Metrics.Prometheus
{
    public class PrometheusOptions
    {
        public bool Enabled { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public IEnumerable<string> AllowedHosts { get; set; }
    }
}