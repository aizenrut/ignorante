namespace Ignorante.Engines.Interfaces
{
    public interface IIgnoranteEngine
    {
        string ResolverComando(string comando);
        string TryResolverComando(string comando);
    }
}
