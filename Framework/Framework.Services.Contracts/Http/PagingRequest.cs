using Newtonsoft.Json;

namespace Framework.Services.Contracts.Http {
    public class PagingRequest : DefaultRequest {
        [JsonConstructor]
        public PagingRequest(int pageIndex, int pageSize) {
            PageIndex = pageIndex <= 0 ? 1 : pageIndex;
            PageSize = pageSize <= 0 ? 10 :
                pageSize > 100 ? 100 : pageSize;
        }

        public int PageIndex { get; }
        public int PageSize { get; }
    }
}