using System;

namespace Framework.Model {
    public interface IAuditable {
        DateTimeOffset CreatedOn { get; }
        string CreatedBy { get; }
        DateTimeOffset? LastModifiedOn { get; }
        string LastModifiedBy { get; }
    }
}