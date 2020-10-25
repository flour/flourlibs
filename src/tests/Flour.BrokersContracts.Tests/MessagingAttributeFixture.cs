using Xunit;

namespace Flour.BrokersContracts.Tests
{
    public class MessagingAttributeFixture
    {
        [Fact]
        public void AbleToCreateMemberDataAttribute()
        {
            new MessagingAttribute(null, string.Empty, string.Empty);
            new MessagingAttribute(string.Empty, null, string.Empty);
            new MessagingAttribute(string.Empty, string.Empty, null);
        }
    }
}
