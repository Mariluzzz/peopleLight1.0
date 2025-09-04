using PeopleLight.Application.Interfaces;
using PeopleLight.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PeopleLight.Domain.Entities;

namespace PeopleLight.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Person person)
        {
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Person>> GetAllAsync() => await _context.Persons.ToListAsync();

        public async Task<Person> GetByIdAsync(Guid id) => await _context.Persons.FindAsync(id);

        public async Task UpdateAsync(Person person)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }
    }
}
