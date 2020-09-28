using Ignorante.Clients.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ignorante.Clients
{
    public class GitignoreIoClient : IGitignoreIoClient
    {
        private const string GITIGNORE_LIST = "https://www.toptal.com/developers/gitignore/api/list";
        private const string GITIGNORE_TYPE = "https://www.toptal.com/developers/gitignore/api/{0}";

        private List<string> list;

        private List<string> ConverterRetornoList(string csv)
        {
            return csv.Replace('\n', ',').Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private async Task<string> GetResourceAsync(string resource)
        {
            using (var cliente = new HttpClient())
            {
                var resposta = await cliente.GetAsync(resource);
                resposta.EnsureSuccessStatusCode();

                return await resposta.Content.ReadAsStringAsync();
            }
        }

        private async Task<List<string>> GetListAsync()
        {
            return ConverterRetornoList(await GetResourceAsync(GITIGNORE_LIST));
        }

        private async Task<string> GetGitignoreAsync(string type)
        {
            return await GetResourceAsync(string.Format(GITIGNORE_TYPE, type));
        }

        public IEnumerable<string> GetList()
        {
            if (list == null || !list.Any())
                list = GetListAsync().Result;

            return list;
        }

        public IEnumerable<string> GetList(string filter)
        {
            return GetList().Where(type => type.Contains(filter));
        }

        public string GetGitignore(string type)
        {
            return GetGitignoreAsync(type).Result;
        }
    }
}
