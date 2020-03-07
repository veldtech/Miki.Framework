namespace Miki.Framework.Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class TestContext<T> : DbContext
        where T : class
    {
        private readonly Action<ModelBuilder> onModelCreating;

        public DbSet<T> Column { get; set; }

        public TestContext(DbContextOptions options, Action<ModelBuilder> onModelCreating = null)
            : base(options)
        {
            this.onModelCreating = onModelCreating;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            onModelCreating?.Invoke(modelBuilder);
        }
    }
}
