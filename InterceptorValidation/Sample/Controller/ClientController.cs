using Model;
using Sample.Repository;
using System.Collections.Generic;

namespace Sample.Controller
{
    public class ClientController
    {
        private IClientRepository _repository;

        public ClientController(IClientRepository repository)
        {
            _repository = repository;
        }

        public void Add(Client client)
        {
            _repository.Add(client);
        }

        public Client Get(int id)
        {
            return _repository.Get(id);
        }

        public void Remove(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<Client> GetAll()
        {
            return _repository.GetAll();
        }

        public void Teste(Client client)
        {
            _repository.Teste(client);
        }
    }
}
