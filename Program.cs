using System;
using System.Collections.Generic;

namespace CaixaEletronico
{
    class Program
    {
        static void Main(string[] args)
        {
            CaixaEletronico Caixa = new CaixaEletronico();
            try
            {
                string opcao = string.Empty;
                CarregarCaixaEletronico(Caixa);
                MotarNotasDisponiveis(Caixa);
                EscreverMenu();
                while ((opcao = Console.ReadLine()) != null)
                {
                    try
                    {
                        if (opcao.ToLower() == "s")
                            Sacar(Caixa);
                        else if (opcao.ToLower() == "n")
                            MotarNotasDisponiveis(Caixa);
                        else if (opcao.ToLower() == "r")
                            CarregarCaixaEletronico(Caixa);
                        else if (opcao.ToLower() == "f")
                            break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.ReadLine();
                    }
                    Console.Clear();
                    MotarNotasDisponiveis(Caixa);
                    EscreverMenu();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        private static void EscreverMenu()
        {
            
            Console.WriteLine("Informe a opção:");
            Console.WriteLine(" s -> Sacar");
            Console.WriteLine(" n -> Notas Disponíveis");
            Console.WriteLine(" r -> Recarregar Caixa");
            Console.WriteLine(" f -> Fechar");
            Console.Write(">>");
        }
        private static void Sacar(CaixaEletronico Caixa)
        {
            string valor;
            Console.Write("Informe o valor do saque: ");
            valor = Console.ReadLine();
            int valorSaque = 0;
            if (int.TryParse(valor, out valorSaque))
            {
                SortedList<int, int> notas = Caixa.Sacar(valorSaque);
                Console.WriteLine("Notas Sacadas: ");
                ListarNotas(notas);
            }
            else
            {
                throw new Exception("Saque não efetuado, favor informado um valor numérico inteiro.");
            }
            Console.Read();
        }

        private static void ListarNotas(SortedList<int, int> notas)
        {
            int total = 0;
            foreach (KeyValuePair<int, int> nota in notas)
            {
                Console.WriteLine(string.Format("R$ {0} -> {1} Notas ", nota.Key, nota.Value));
                total += nota.Key * nota.Value;
            }
            Console.WriteLine(string.Format("Total R$ {0}", total));
        }

        private static void MotarNotasDisponiveis(CaixaEletronico Caixa)
        {
            Console.WriteLine("Notas Disponiveis:");
            SortedList<int, int> notas = Caixa.BuscarNotasDisponiveis();
            ListarNotas(notas);
        }
        private static void CarregarCaixaEletronico(CaixaEletronico Caixa)
        {
            Caixa.Descarregar();
            Caixa.Carregar(10, 1);
            Caixa.Carregar(20, 3);
            Caixa.Carregar(50, 5);
            Caixa.Carregar(100, 7);
        }
    }
}
