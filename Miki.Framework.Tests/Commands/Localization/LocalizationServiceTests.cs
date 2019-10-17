namespace Miki.Framework.Tests.Commands.Localization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Framework.Commands.Localization.Services;
    using Microsoft.EntityFrameworkCore;
    using Miki.Framework.Commands.Localization.Models;
    using Miki.Framework.Commands.Localization.Models.Exceptions;
    using Miki.Localization;
    using Miki.Localization.Models;
    using Xunit;

    public class LocalizationServiceTests : BaseEntityTest<ChannelLanguage>  
    {
        private const long ValidId = 0L;
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
        public async Task GetValidLocaleTest()
        {
            using var context = NewContext();
            var service = CreateService(context);

            Locale locale = await service.GetLocaleAsync(ValidId);

            Assert.NotNull(locale);
            Assert.Equal("dut", locale.CountryCode);
        }

        [Fact]
        public async Task GetDefaultLocaleTest()
        {
            using var context = NewContext();
            var service = CreateService(context);

            Locale locale = await service.GetLocaleAsync(InvalidId);

            Assert.NotNull(locale);
            Assert.Equal("eng", locale.CountryCode);
        }

        [Fact]
        public async Task GetInvalidLocaleTest()
        {
            using var context = NewContext();

            var serviceNoFallback = new LocalizationService(context, new LocalizationService.Config
            {
                DefaultIso3 = "invalid"
            });

            await Assert.ThrowsAsync<LocaleNotFoundException>(
                async () => await serviceNoFallback.GetLocaleAsync(InvalidId))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ListLocalesTest()
        {
            using var context = NewContext();
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
        public async Task SetLocaleTest()
        {
            using var context = NewContext();
            var service = CreateService(context);

            await service.SetLocaleAsync(ValidId, "eng");

            Locale locale = await service.GetLocaleAsync(ValidId);

            Assert.NotNull(locale);
            Assert.Equal("eng", locale.CountryCode);
        }

        [Fact]
        public async Task AddLocaleTest()
        {
            using var context = NewContext();
            var service = CreateService(context);

            var locales = new HashSet<string>();
            await foreach(var i in service.ListLocalesAsync())
            {
                locales.Add(i);
            }
            Assert.Equal(2, locales.Count);
            
            service.AddLocale(new Locale("swe", null));

            var newLocales = new HashSet<string>();
            await foreach(var i in service.ListLocalesAsync())
            {
                newLocales.Add(i);
            }
            Assert.Equal(3, newLocales.Count);
        }

        private ILocalizationService CreateService(IUnitOfWork context)
        {
            var service = new LocalizationService(context);
            service.AddLocale(new Locale("eng", null));
            service.AddLocale(new Locale("dut", null));
            return service;
        }
    }
}
