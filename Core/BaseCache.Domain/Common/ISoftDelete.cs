namespace BaseCache.Domain.Common;


public interface ISoftDelete
{
    DateTime? DeletedOn { get; set; }
    int? DeletedBy { get; set; }
}
