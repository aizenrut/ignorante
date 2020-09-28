using System.Collections.Generic;

namespace Ignorante.Clients.Interfaces
{
    public interface IGitignoreIoClient
    {
        IEnumerable<string> GetList();
        IEnumerable<string> GetList(string filter);
        string GetGitignore(string type);
    }
}
