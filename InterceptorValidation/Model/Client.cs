using Interceptor.Interfaces;
using System;

namespace Model
{
    public class Client : IValidate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            var age = DateTime.Now.Year - BirthDate.Year;
            return $"Id : {Id}, Name: {Name} {SurName}, Age: {age}";
        }

        public string Validate()
        {
            if (Id != 5)
            {
                return "Id não pode ser diferente de 5";
            }

            return string.Empty;
        }
    }
}
