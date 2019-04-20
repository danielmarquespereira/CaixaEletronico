using System;
using System.Collections.Generic;
using System.Linq;

namespace CaixaEletronico
{
    public class CaixaEletronico : ICaixaEletronico
    {
        private Dictionary<int, int> NotasDisponiveis = new Dictionary<int, int>();
        public CaixaEletronico()
        {
        }
        /// <summary>
        /// Carrega notas no caixa eletronico
        /// </summary>
        /// <param name="nota"></param>
        /// <param name="quantidade"></param>
        public void Carregar(int nota, int quantidade)
        {
            this.NotasDisponiveis.Add(nota, quantidade);
        }
        /// <summary>
        /// M�todo Sacar
        ///  - Tentar primeiramente distribuir de forma simples pela nota maior
        ///  - Se n�o conseguir distribuir todos o saque, tentar de forma recursiva iterando todas as possibilidades
        /// </summary>
        /// <param name="valorSaqueTotal"></param>
        /// <returns></returns>
        public SortedList<int, int> Sacar(int valorSaqueTotal)
        {

            SortedList<int, int> NotasDistribuidas = new SortedList<int, int>();

            //Reduzir quantidades ao maximo coeciente, performence racinalizada
            IDictionary<int, int> NotasCopia = new Dictionary<int, int>();
            NotasDisponiveis.ToList().ForEach(n => NotasCopia.Add(n.Key, (valorSaqueTotal / n.Key) > n.Value ? n.Value : (valorSaqueTotal / n.Key)));

            //Distribuir notas caso decrescente simples
            int valorSaque = DistribuirNotas(valorSaqueTotal, NotasDistribuidas, NotasDisponiveis);
            if (valorSaque > 0)
            {
                //Se distribui��o simples n�o funcionou, tentar recursiva
                int primeiraNota = this.NotasDisponiveis.Where(n=>n.Value>0).OrderBy(c => c.Key).Select(n => n.Key).FirstOrDefault();
                List<SortedList<int, int>> DistribuicoesCompletas = new List<SortedList<int, int>>();
                DistribuirNotasRecursiva(valorSaqueTotal, primeiraNota, NotasCopia, DistribuicoesCompletas);
                
                //Pegar Distribui��o com Menos Notas
                NotasDistribuidas = DistribuicoesCompletas.OrderBy(dc => dc.Sum(nd => nd.Value)).FirstOrDefault();
                if (NotasDistribuidas!=null) valorSaque = 0;
            }
            if (valorSaque == 0)
            {
                //Descarregar Cassetes com notas distribuidas 
                foreach (KeyValuePair<int, int> nota in NotasDistribuidas)
                {
                    KeyValuePair<int, int> notaDisponivel = NotasDisponiveis.Where(n => n.Key == nota.Key).FirstOrDefault();
                    NotasDisponiveis[notaDisponivel.Key] -= nota.Value;
                }
            }
            else
            {
                throw new Exception("Saque n�o realizado, notas insuficientes no caixa eletronico.");
            }
            return NotasDistribuidas;
        }
        /// <summary>
        /// Distribui��o de notas simples pela manior nota
        /// Atendende casos como :
        ///     Notas Dispon�veis [R$ 100; 1], [R$ 50; 2], [R$ 20; 3], [R$ 10; 0]
        ///     Saque R$ 150
        ///     Distribui��o [R$ 100; 1], [R$ 50; 1]
        /// N�o Atendem casos como :
        ///     Notas Dispon�veis [R$ 100; 1], [R$ 50; 2], [R$ 20; 3], [R$ 10; 0]
        ///     Saque R$ 110
        ///     Distribui��o [R$ 50; 1], [R$ 20; 3]
        ///     
        ///     Esse ser� resolvido pela distribui��o recursiva
        ///     
        /// </summary>
        /// <param name="valorSaqueTotal"></param>
        /// <param name="Notas"></param>
        /// <param name="NotasOriginais"></param>
        /// <returns></returns>
        private int DistribuirNotas(int valorSaqueTotal, SortedList<int, int> Notas, IDictionary<int, int> NotasOriginais)
        {
            int valorSaque = valorSaqueTotal;
            foreach (KeyValuePair<int, int> nota in NotasOriginais.OrderByDescending(c => c.Key))
            {
                int notaCassete = nota.Key;
                int quantidadeNotasCassete = nota.Value;
                int valorTotalCassete = notaCassete * quantidadeNotasCassete;
                //Nota sendo menor que o valor do saque, e havendo notas no cassete
                if ((notaCassete <= valorSaque) && quantidadeNotasCassete > 0)
                {
                    int valorSacar = valorSaque;
                    //Se o saldo do cassete for menor que o valor para sacar -> pega o resto como valor a sacar do cassete;
                    if (valorTotalCassete < valorSacar) valorSacar = valorTotalCassete % valorSacar;
                    int quantidadeNotasSacadas = (valorSacar / notaCassete);

                    if (quantidadeNotasSacadas > 0 && quantidadeNotasSacadas <= quantidadeNotasCassete)
                    {
                        valorSaque -= quantidadeNotasSacadas * notaCassete;
                        Notas.Add(notaCassete, quantidadeNotasSacadas);
                    }
                }

            }
            return valorSaque;
        }
        /// <summary>
        /// A distribui��o recursiva itera todas as possibilidades e carrega a lista de distribui��es completas, onde o saldo foi totalmente atendido.
        /// Por explorar casos descenecess�rio � aconselhavel, tentar uma primeira distribui��o simples, para s� ent�o tentar a recursiva.
        /// Atenden casos como , e todos os outros.
        ///     Notas Dispon�veis [R$ 100; 1], [R$ 50; 2], [R$ 20; 3], [R$ 10; 0]
        ///     Saque R$ 110
        ///     Distribui��o [R$ 50; 1], [R$ 20; 3]
        /// </summary>
        /// <param name="valorSaque"></param>
        /// <param name="nota"></param>
        /// <param name="NotasOriginal"></param>
        /// <param name="DistribuicoesCompletas"></param>
        private void DistribuirNotasRecursiva(int valorSaque, int nota, IDictionary<int, int> NotasOriginal, List<SortedList<int, int>> DistribuicoesCompletas)
        {
            IDictionary<int, int> NotasCopia = new Dictionary<int, int>();
            NotasOriginal.ToList().ForEach(n => NotasCopia.Add(n));
            int quantidadeOriginal = NotasOriginal.Where(n => n.Key == nota).Select(n => n.Value).FirstOrDefault();
            for (int i = 0;  i <= quantidadeOriginal; i++)
            {
                NotasCopia[nota] = quantidadeOriginal-i;

                SortedList<int, int> NotasDistribuidas = new SortedList<int, int>();
                int valorSaqueIteracao = DistribuirNotas(valorSaque, NotasDistribuidas, NotasCopia);
                if (valorSaqueIteracao==0)
                {
                    DistribuicoesCompletas.Add(NotasDistribuidas);
                }
                
                foreach (KeyValuePair<int, int> proximaNota in NotasOriginal.OrderBy(c => c.Key).Where(n => n.Key > nota))
                {
                    if (proximaNota.Key > 0) DistribuirNotasRecursiva(valorSaque, proximaNota.Key, NotasCopia, DistribuicoesCompletas);
                }
            }
            return;
        }
        /// <summary>
        /// Busca a rela��o das notas dispon�veis
        /// </summary>
        /// <returns></returns>
        public SortedList<int, int> BuscarNotasDisponiveis()
        {
            SortedList<int, int> Notas = new SortedList<int, int>();
            foreach (KeyValuePair<int, int> nota in this.NotasDisponiveis.ToList().OrderByDescending(c => c.Key))
            {
                Notas.Add(nota.Key, nota.Value);
            }
            return Notas;
        }
        /// <summary>
        /// Descarrega todas as notas do caixa
        /// </summary>
        public void Descarregar()
        {
            this.NotasDisponiveis.Clear();
        }

    }
}