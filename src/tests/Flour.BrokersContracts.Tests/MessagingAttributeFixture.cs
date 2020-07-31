using Xunit;

namespace Flour.BrokersContracts.Tests
{
    public class MessagingAttributeFixture
    {
        [Fact]
        public void AbleToCreateMemberDataAttribute()
        {
            new MemberDataAttribute(null, string.Empty, string.Empty);
            new MemberDataAttribute(string.Empty, null, string.Empty);
            new MemberDataAttribute(string.Empty, string.Empty, null);
        }
    }
}
