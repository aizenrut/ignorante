using Ignorante.Engines;
using Ignorante.Engines.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Ignorante
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IIgnoranteEngine ignoranteEngine = new IgnoranteEngine();

            Console.WriteLine("IGNORANTE v1.0");
            Console.WriteLine();
            Console.WriteLine(ignoranteEngine.ResolverComando("help"));

            string currentDirectory = Directory.GetCurrentDirectory();
            int ultimaBarra = currentDirectory.LastIndexOf('\\');
            string pastaAtual = currentDirectory.Substring(ultimaBarra, (currentDirectory.Length - ultimaBarra));

            bool continuar = true;

            do
            {
                Console.Write($"...{pastaAtual}>");
                
                string comando = Console.ReadLine();

                string resultado = ignoranteEngine.TryResolverComando(comando);

                continuar = resultado.Any();

                if (continuar)
                {
                    Console.Clear();
                    Console.WriteLine("IGNORANTE v1.0");
                    Console.WriteLine();
                    Console.WriteLine(resultado);
                }
                
            } while (continuar);
        }
    }
}
