namespace UnityFigmaMCP.Server.Unity
{
    public interface IUnityConnectionRegistry
    {
        string? Current { get; }

        void Add(string connectionId);
        void Remove(string connectionId);
    }
}
