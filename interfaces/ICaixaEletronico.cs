using System.Collections.Generic;

namespace CaixaEletronico
{
    interface ICaixaEletronico
    {
        void Carregar(int nota, int quantidade);
        void Descarregar();
        SortedList<int, int> Sacar(int valor);
        SortedList<int, int> BuscarNotasDisponiveis();
    }
}