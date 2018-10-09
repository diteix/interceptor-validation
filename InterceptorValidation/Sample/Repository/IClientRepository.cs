using Model;
using System.Collections.Generic;

namespace Sample.Repository
{
    public interface IClientRepository
    {
        void Add(Client client);

        Client Get(int id);

        void Remove(int id);

        IEnumerable<Client> GetAll();

        void Teste(Client client);
    }
}
