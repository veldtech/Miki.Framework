namespace Miki.Framework.Tests.Commands.Localization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Miki.Framework.Commands.Localization.Models;
    using Miki.Framework.Commands.Localization.Models.Exceptions;
    using Miki.Localization;
    using Miki.Localization.Models;
    using Miki.Services.Localization;
    using Xunit;

    public class LocalizationServiceTests : BaseEntityTest<ChannelLanguage>  
    {
        private readonly ILocalizationService service;
        private readonly TestContext<ChannelLanguage> context;

        private const long ValidId = 0L;
        private const long InvalidId = 1L;

        public LocalizationServiceTests()
        {
            context = NewDbContext();
            context.Column.AddRange(
                new ChannelLanguage
                {
                    EntityId = 0L,
                    Language = "dut"
                });
            context.SaveChanges();
            
            service = new LocalizationService(context);
            service.AddLocale(new Locale("eng", null));
            service.AddLocale(new Locale("dut", null));
        }

        [Fact]
        public async Task GetValidLocaleTest()
        {
            Locale locale = await service.GetLocaleAsync(ValidId);

            Assert.NotNull(locale);
            Assert.Equal("dut", locale.CountryCode);
        }

        [Fact]
        public async Task GetDefaultLocaleTest()
        {
            Locale locale = await service.GetLocaleAsync(InvalidId);

            Assert.NotNull(locale);
            Assert.Equal("eng", locale.CountryCode);
        }

        [Fact]
        public async Task GetInvalidLocaleTest()
        {
            var serviceNoFallback = new LocalizationService(context, null, new LocalizationService.Config
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
            await service.SetLocaleAsync(ValidId, "eng");

            Locale locale = await service.GetLocaleAsync(ValidId);

            Assert.NotNull(locale);
            Assert.Equal("eng", locale.CountryCode);
        }

        [Fact]
        public async Task AddLocaleTest()
        {
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
    }
}
