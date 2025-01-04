using System.Runtime.Serialization;

namespace Flour.Commons.Models.PagedRequest;

[DataContract]
public class SortDescriptor(string field, EnumSortDirection order = EnumSortDirection.Asc)
{
    [DataMember(Order = 1)] public string Field { get; set; } = field;
    [DataMember(Order = 2)] public EnumSortDirection Order { get; set; } = order;

    public override string ToString()
    {
        var direction = string.Empty;

        if (Order == EnumSortDirection.Desc) direction = $" {Order.ToString()}";

        return $"{Field}{direction}";
    }
}