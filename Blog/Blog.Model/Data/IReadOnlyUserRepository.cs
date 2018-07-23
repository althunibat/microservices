using Framework.Model.Data;

namespace Blog.Model.Data {
    public interface IReadOnlyUserRepository : IReadOnlyRepository<User,string> {

    }
}