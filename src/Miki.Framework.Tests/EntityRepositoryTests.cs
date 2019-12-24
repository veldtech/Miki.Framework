namespace Miki.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class Model
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    public class EntityRepositoryTests : BaseEntityTest<Model>
    {
        public EntityRepositoryTests()
        {
            // Init default value on 0
            using var context = NewDbContext();
            context.Add(new Model {Key = 0, Value = "value"});
            context.SaveChanges();
        }

        [Fact]
        public async Task AddAndUpdateTest()
        {
            var newItem = new Model
            {
                Key = new Random().Next(1, 1000),
                Value = "newValue"
            };

            await using(var unit = NewContext())
            {
                var repository = unit.GetRepository<Model>();
                await repository.AddAsync(newItem);
                await unit.CommitAsync();
            }

            await using(var unit = NewContext())
            {
                var repository = unit.GetRepository<Model>();
                var value = await repository.GetAsync(newItem.Key);

                Assert.NotNull(value);
                Assert.Equal(newItem.Value, value.Value);
            }

            newItem.Value = "updatedValue";

            await using(var unit = NewContext())
            {
                var repository = unit.GetRepository<Model>();
                await repository.EditAsync(newItem);
                await unit.CommitAsync();
            }

            await using(var unit = NewContext())
            {
                var repository = unit.GetRepository<Model>();
                var value = await repository.GetAsync(newItem.Key);

                Assert.NotNull(value);
                Assert.Equal(newItem.Value, value.Value);
            }
        }

        protected override void OnModelCreating([NotNull] ModelBuilder builder)
        {
            builder.Entity<Model>()
                .HasKey(x => x.Key);
        }
    }
}
