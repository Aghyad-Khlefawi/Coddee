namespace Coddee.Services
{
    public interface IConfigurationFile
    {
        string Name { get; }
        string Path { get; }

        TValue GetValue<TValue>(string key);
        void ReadFile();
        void SetValue<TValue>(string key, TValue value);
        bool TryGetValue<TValue>(string key, out TValue value);
    }
}