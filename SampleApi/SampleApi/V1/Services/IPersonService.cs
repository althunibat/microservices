using System.Threading.Tasks;
using Framework.Services.Contracts.Http;

namespace SampleApi.V1.Services
{
    public interface IPersonService
    {
        Task<IResponse> GetById(ByIdRequest<long> request);
    }
}