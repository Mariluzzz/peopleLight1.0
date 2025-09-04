using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleLight.Domain.Entities;
using PeopleLight.Domain.ValueObjects;

namespace PeopleLight.Infrastructure.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> entity)
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Document)
                  .HasConversion(
                      d => d.Value,
                      v => new Documents(v)
                  )
                  .HasMaxLength(14)
                  .IsRequired();

            entity.Property(p => p.Name)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(p => p.Email)
                  .HasMaxLength(100);

            entity.Property(p => p.Phone)
                  .HasMaxLength(20);
        }
    }
}
