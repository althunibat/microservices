namespace Framework.Services.Core
{
    public interface ICache<TCacheValue>
    {
        bool Add(string key, TCacheValue value);
        bool Exists(string key);
        TCacheValue Get(string key);
    }
}