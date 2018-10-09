using Interceptor;
using Model;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class Repository : IClientRepository
    {
        private IList<Client> _clients = new List<Client>();

        public void Add(Client client)
        {
            _clients.Add(client);
        }

        public Client Get(int id)
        {
            return _clients.FirstOrDefault(s => s.Id == id);
        }

        public void Remove(int id)
        {
            _clients.Remove(Get(id));
        }

        public IEnumerable<Client> GetAll()
        {
            return _clients.AsEnumerable();
        }

        public void Teste(Client client)
        {
            Console.WriteLine("Erros");
            Console.WriteLine("    " + InterceptorValidator.Errors.First());
        }
    }
}
