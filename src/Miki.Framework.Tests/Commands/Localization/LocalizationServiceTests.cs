namespace Miki.Framework.Tests.Commands.Localization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Framework.Commands.Localization.Services;
    using Miki.Framework.Commands.Localization;
    using Miki.Framework.Commands.Localization.Models;
    using Miki.Framework.Commands.Localization.Models.Exceptions;
    using Miki.Localization;
    using Miki.Localization.Models;
    using Xunit;

    public class LocalizationServiceTests : BaseEntityTest<ChannelLanguage>  
    {
        private const long Id = 0L;
        private const long InvalidId = 1L;

        public LocalizationServiceTests()
        {
            using var context = NewDbContext();
            context.Column.AddRange(
                new ChannelLanguage
                {
                    EntityId = 0L,
                    Language = "dut"
                });
            context.SaveChanges();
        }

        [Fact]
        public async Task GetValidLocaleTestAsync()
        {
            await using var context = NewContext();
            var service = CreateService(context);

            Locale locale = await service.GetLocaleAsync(Id);

            Assert.NotNull(locale);
            Assert.Equal("dut", locale.CountryCode);
        }

        [Fact]
        public async Task GetDefaultLocaleTestAsync()
        {
            await using var context = NewContext();
            var service = CreateService(context);

            Locale locale = await service.GetLocaleAsync(InvalidId);

            Assert.NotNull(locale);
            Assert.Equal("eng", locale.CountryCode);
        }

        [Fact]
        public async Task GetInvalidLocaleTestAsync()
        {
            await using var context = NewContext();

            var serviceNoFallback = new LocalizationService(
                context, new LocaleCollection(), new LocalizationService.Config
                {
                    DefaultIso3 = "invalid"
                });

            await Assert.ThrowsAsync<LocaleNotFoundException>(
                async () => await serviceNoFallback.GetLocaleAsync(InvalidId))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ListLocalesTestAsync()
        {
            await using var context = NewContext();
            var service = CreateService(context);

            var locales = new HashSet<string>();
            await foreach(var i in service.ListLocalesAsync())
            {
                locales.Add(i);
            }

            Assert.Contains("eng", locales);
            Assert.Contains("dut", locales);
            Assert.DoesNotContain("swe", locales);
        }

        [Fact]
        public async Task SetLocaleTestAsync()
        {
            await using (var context = NewContext())
            {
                var service = CreateService(context);
                await service.SetLocaleAsync(Id, "eng");
                await context.CommitAsync();
            }

            await using(var context = NewContext())
            {
                var service = CreateService(context);
                Locale locale = await service.GetLocaleAsync(Id);

                Assert.NotNull(locale);
                Assert.Equal("eng", locale.CountryCode);
            }
        }

        private ILocalizationService CreateService(IUnitOfWork context)
        {
            var collection = new LocaleCollection();
            collection.Add(new Locale("eng", null));
            collection.Add(new Locale("dut", null));

            var service = new LocalizationService(context, collection);
            return service;
        }
    }
}
