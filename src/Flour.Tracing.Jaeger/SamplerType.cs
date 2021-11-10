using System.ComponentModel;

namespace Flour.Tracing.Jaeger
{
    public enum SamplerType
    {
        [Description("constant")] Constant,
        [Description("rate")] Rate,
        [Description("prob")] Probabilistic
    }
}