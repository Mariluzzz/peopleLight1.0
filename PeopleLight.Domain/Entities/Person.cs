using PeopleLight.Domain.ValueObjects;

namespace PeopleLight.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Documents Document { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public Person(string name, Documents document, string email, string phone)
        {
            Id = Guid.NewGuid();
            Name = name;
            Document = document;
            Email = email;
            Phone = phone;
        }

        public void Update(string name, Documents document, string email, string phone)
        {
            Name = name;
            Document = document;
            Email = email;
            Phone = phone;
        }
    }
}
