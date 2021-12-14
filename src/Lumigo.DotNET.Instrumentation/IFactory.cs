namespace Lumigo.DotNET.Instrumentation
{
    public interface IFactory<TKey, TValue>
    {
        TValue GetInstace(TKey key);
    }
}
