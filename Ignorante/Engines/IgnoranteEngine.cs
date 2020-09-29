using Ignorante.Clients;
using Ignorante.Clients.Interfaces;
using Ignorante.Engines.Interfaces;
using Ignorante.Pools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Ignorante.Engines
{
    public class IgnoranteEngine : IIgnoranteEngine
    {
        private const string CMD_HELP = "help";
        private const string CMD_LIST = "list";
        private const string CMD_SHOW = "show";
        private const string CMD_GITIGNORE = "gitignore";
        private const string CMD_AUTHOR = "author";
        private const string CMD_EXIT = "exit";

        private const string MEU_LINKEDIN = "https://www.linkedin.com/in/igor-christofer-eisenhut/";
        private const string MEU_GIT = "https://github.com/Aizenrut";

        private const string ERRO_NAO_IDENTIFICADO = "** Ops, ocorreu um erro não identificado.\n\n\nPor favor, me contate (informações no comando \"author\") e encaminhe toda esta mensagem:\n\n{0}\n";


        private IDictionary<string, string> gitignoreCache;

        private IDictionary<string, Func<string>> funcoesSemParametro;
        private IDictionary<string, Func<string, string>> funcoesComUmParametro;

        private IGitignoreIoClient gitignoreIoClient;
        public IGitignoreIoClient GitignoreIoClient
        {
            get
            {
                if (gitignoreIoClient == null)
                    gitignoreIoClient = new GitignoreIoClient();

                return gitignoreIoClient;
            }

            set => gitignoreIoClient = value;
        }

        public IgnoranteEngine()
        {
            gitignoreCache = new Dictionary<string, string>();

            funcoesSemParametro = new Dictionary<string, Func<string>>();
            funcoesSemParametro.Add(CMD_HELP, () => Help());
            funcoesSemParametro.Add(CMD_LIST, () => List());
            funcoesSemParametro.Add(CMD_AUTHOR, () => Author());
            funcoesSemParametro.Add(CMD_EXIT, () => { return string.Empty; });

            funcoesComUmParametro = new Dictionary<string, Func<string, string>>();
            funcoesComUmParametro.Add(CMD_LIST, param => List(param));
            funcoesComUmParametro.Add(CMD_SHOW, param => Show(param));
            funcoesComUmParametro.Add(CMD_GITIGNORE, param => Gitignore(param));
        }

        private bool ExisteGitignoreSalvo(string tipo)
        {
            return gitignoreCache.ContainsKey(tipo);
        }

        private void SalvarGitignore(string tipo, string gitignore)
        {
            if (ExisteGitignoreSalvo(tipo))
                return;

            gitignoreCache[tipo] = gitignore;
        }

        private string ObterGitignore(string tipo)
        {
            if (ExisteGitignoreSalvo(tipo))
            {
                return gitignoreCache[tipo];
            }

            var gitignore = GitignoreIoClient.GetGitignore(tipo);

            SalvarGitignore(tipo, gitignore);

            return gitignore;
        }

        private IEnumerable<string> ObterListaPeloParametro(string filtro)
        {
            if (filtro.Any())
            {
                return GitignoreIoClient.GetList(filtro);
            }
            else
            {
                return GitignoreIoClient.GetList();
            }
        }

        private string Help()
        {
            var sb = StringBuilderPool.ObterDaPool();

            sb.AppendLine("** Comandos ignorantes:");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"\t{CMD_LIST}: listar todos os tipos disponíveis de .gitignore;");
            sb.AppendLine("\t" + CMD_LIST + " {filtro}: listar os tipos de .gitignore que contenham o filtro informado;");
            sb.AppendLine("\t" + CMD_SHOW + " {tipo}: consultar o .gitignore do tipo;");
            sb.AppendLine("\t" + CMD_GITIGNORE + " {tipo}: gerar o .gitignore para o tipo informado;");
            sb.AppendLine($"\t{CMD_AUTHOR}: informações sobre o desenvolvedor;");
            sb.AppendLine($"\t{CMD_EXIT}: sair do Ignorante.");
            sb.AppendLine();
            sb.AppendLine("** O arquivo .gitignore será criado na pasta em que o executável estiver.");

            return sb.ToString();
        }

        private string List(string filtro = "")
        {
            var sb = StringBuilderPool.ObterDaPool();

            sb.AppendLine("** Tipos disponíveis:");
            sb.AppendLine();
            sb.AppendLine();

            bool teveRetorno = false;

            foreach (var tipo in ObterListaPeloParametro(filtro))
            {
                if (!teveRetorno)
                {
                    teveRetorno = true;
                }

                sb.AppendLine($"\t{tipo}");
            }

            string mensagem;

            if (teveRetorno)
            {
                mensagem = sb.ToString();
            }
            else
            {
                mensagem = "** Nenhum tipo de .gitignore encontrado" + (filtro.Any() ? $" para o filtro \"{filtro}\"" : string.Empty) + ".\n";
            }

            return mensagem;
        }

        private string Show(string tipo)
        {
            var gitignore = ObterGitignore(tipo);

            var sb = StringBuilderPool.ObterDaPool();
            sb.AppendLine($"** .gitignore para o tipo {tipo}:");
            sb.AppendLine();

            foreach (var linha in gitignore.Split('\n'))
            {
                sb.AppendLine($"\t{linha}");
            }

            return sb.ToString();
        }

        private string Gitignore(string tipo)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDir, ".gitignore");
            var gitignore = ObterGitignore(tipo);

            File.WriteAllText(path, gitignore);

            var sb = StringBuilderPool.ObterDaPool();
            sb.AppendLine(Show(tipo));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"** .gitignore criado em {currentDir}");

            return sb.ToString();
        }

        private string Author()
        {
            var sb = StringBuilderPool.ObterDaPool();
            sb.AppendLine("(Escrito em setembro de 2020)");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("** O Ignorante");
            sb.AppendLine();
            sb.AppendLine("\tDesenvolvi o Ignorante porque meus repositórios do Git sempre continham os arquivos temporários que eram");
            sb.AppendLine("\tgerados pelo Visual Studio, as DLLs compiladas dos meus programas, os pacotes de dependência do Node.js,");
            sb.AppendLine("\te por aí vai. Não tinha percebido isso até que (semestre pssado) tive que entregar um trabalho com cerca");
            sb.AppendLine("\tde estonteantes 100MB ao meu professor de web apenas por causa dos pacotes de dependências que o Node");
            sb.AppendLine("\thavia baixado. Quando ele pediu para adicionar um .gitignore para que o projeto não ficasse imenso eu");
            sb.AppendLine("\tnão soube como fazer.");
            sb.AppendLine();
            sb.AppendLine("\tNa última sexta estava conversando com uns amigos enquanto fazíamos um trabalho da faculdade, quando um deles");
            sb.AppendLine("\t(o Éliton), foi criar o .gitignore no nosso projeto. Ele magicamente abriu um site (gitignore.io) que continha");
            sb.AppendLine("\tcentenas de tipos de .gitignore para cada linguagem de programação que se pudesse imaginar, copiou o conteúdo");
            sb.AppendLine("\tdo template para a linguagem que estávamos utilizando, colou no Visual Studio Code, salvou o arquivo como");
            sb.AppendLine("\t\".gitignore\" e colou na pasta do projeto. Notei duas coisas: 1 - durante todo esse tempo em que meus");
            sb.AppendLine("\trepositórios ficaram poluídos existia uma solução ridiculamente simples para o problema; e 2 - toda vez que");
            sb.AppendLine("\teu quisesse criar um arquivo .gitignore, teria que repetir passo a passo todas as etapas que o Éliton executou,");
            sb.AppendLine("\tum processo bem chato pra falar a verdade.");
            sb.AppendLine();
            sb.AppendLine("\tComo atualmente estou estudando sobre APIs e elas geralmente seguem um padrão de rota \"{endereço do site}/api\",");
            sb.AppendLine("\tresolvi fazer o teste para o gitignore.io. Adicionei o \"/api\" à URL do site e... tcharam! O site tinha");
            sb.AppendLine("\tuma API pública. Fiquei muito feliz, porque assim eu sabia como resolver a segunda coisa que eu havia");
            sb.AppendLine("\tnotado sexta-feira: com a API, eu poderia criar um serviço que consome as informações do site e faz");
            sb.AppendLine("\ttodo o processo trabalhoso para mim.");
            sb.AppendLine();
            sb.AppendLine("\tAssim nasceu o Ignorante, um aplicativo para facilitar a vida de devs que, assim como eu, não gostam de");
            sb.AppendLine("\tprocessos manuais mas precisam manter seus projetos organizados e sem resíduos. Caso tenha alguma sugestão ");
            sb.AppendLine("\tde melhoria ouencontre algum bug, por favor me contate para que eu possa realizar os ajustes!");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("** Sobre o desenvolvedor, Igor Christofer Eisenhut:");
            sb.AppendLine();
            sb.AppendLine("\tOlá, espero que este app esteja lhe ajudando! Tenho 20 anos e estou desde 2018 atuando profissionalmente");
            sb.AppendLine("\tno setor de desenvolvimento de sistemas. Tenho enorme paixão por essa área, onde me especializo na");
            sb.AppendLine("\tlinguagem de programação C#. Busco sempre me manter atualizado com as tendências do mercado e adoro fazer");
            sb.AppendLine("\tcursos, tanto presencialmente quanto em plataformas online (aliás, atualmente estou no quarto semestre");
            sb.AppendLine("\tdo curso de Bacharelado em Ciência da Computação). Gosto muito de ler sobre ciência (principalmente física");
            sb.AppendLine("\te astronomia), finanças e boas práticas de programação");
            sb.AppendLine();
            sb.AppendLine($"\tLinkedIn: {MEU_LINKEDIN}");
            sb.AppendLine($"\tGithub: {MEU_GIT}");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private string[] ObterTokens(string comando)
        {
            return comando.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Select(token => token.ToLower())
                          .ToArray();
        }

        private string ExecutarFuncao(string[] tokens)
        {
            if (tokens == null || tokens.Length <= 0 || !(funcoesSemParametro.ContainsKey(tokens[0]) || funcoesComUmParametro.ContainsKey(tokens[0])))
            {
                throw new ArgumentException($"** Comando inválido! Digite \"{CMD_HELP}\" para obter ajuda.\n");
            }

            if (tokens.Length == 1)
            {
                return funcoesSemParametro[tokens[0]]();
            }

            if (tokens[1] == CMD_LIST)
            {
                throw new ArgumentException($"** Comando inválido! \"{CMD_LIST}\" não pode ser utilizado como parâmetro.\n");
            }

            return funcoesComUmParametro[tokens[0]](tokens[1]);
        }

        private bool EhNotFound(Exception e)
        {
            return e.GetType().IsAssignableFrom(typeof(HttpRequestException)) && e.Message.Contains(((int)HttpStatusCode.NotFound).ToString());
        }

        private bool EhErroNoHost(Exception e)
        {
            return e.GetType().IsAssignableFrom(typeof(HttpRequestException)) && e.Message.Contains("host");
        }

        public string ResolverComando(string comando)
        {
            var tokens = ObterTokens(comando);
            return ExecutarFuncao(tokens);
        }

        public string TryResolverComando(string comando)
        {
            string mensagem = string.Empty;

            try
            {
                mensagem = ResolverComando(comando);
            }
            catch (AggregateException agg)
            {
                foreach (var e in agg.InnerExceptions)
                {
                    if (EhNotFound(e))
                    {
                        mensagem = "** Tipo de .gitignore não encontrado.\n";
                    }
                    else if (EhErroNoHost(e))
                    {
                        mensagem = "** Por favor, verifique sua conexão com a internet.\n";
                    }
                    else
                    {
                        mensagem = string.Format(ERRO_NAO_IDENTIFICADO, e);
                    }
                }
            }
            catch (Exception exception)
            {
                mensagem = exception.Message;
            }

            return mensagem;
        }
    }
}
