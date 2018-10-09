using Interceptor;
using Model;
using Sample.Controller;
using Sample.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var repositoryAssemblyPath = "C:\\Diego\\Projetos Teste\\interceptor-validation\\interceptor-validation\\InterceptorValidation\\Repository\\bin\\Debug\\Repository.dll";

            InterceptorValidator.Create(repositoryAssemblyPath);
            var assembly = Assembly.LoadFile(repositoryAssemblyPath);

            var nameList = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("MC", "WM"),
                new KeyValuePair<string, string>("MC", "Kekel"),
                new KeyValuePair<string, string>("MC", "Maha"),
                new KeyValuePair<string, string>("MC", "Kevinho"),
                new KeyValuePair<string, string>("MC", "Gustta"),
                new KeyValuePair<string, string>("MC", "Pikeno"),
                new KeyValuePair<string, string>("MC", "Menor"),
                new KeyValuePair<string, string>("MC", "Brinquedo"),
                new KeyValuePair<string, string>("MC", "Bin Laden")
            };

            var clientController = new ClientController(assembly.CreateInstance("Repository.Repository") as IClientRepository);

            var random = new Random();

            for (int i = 0; i < 20; i++)
            {
                var name = nameList[random.Next(9)];
                var client = new Client()
                {
                    Id = random.Next(100),
                    Name = name.Key,
                    SurName = name.Value,
                    BirthDate = DateTime.Now.AddYears(-random.Next(30))
                };

                clientController.Add(client);
            }

            var clients = clientController.GetAll();

            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }

            clientController.Teste(clients.First());

            Console.ReadLine();
        }
    }
}
