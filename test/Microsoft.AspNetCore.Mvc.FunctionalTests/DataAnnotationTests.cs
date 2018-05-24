using System.Net;
using System.Threading.Tasks;
using RazorWebSite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class DataAnnotationTests : IClassFixture<MvcTestFixture<StartupDataAnnotations>>
    {
        private MvcTestFixture<StartupDataAnnotations> _fixture;

        public DataAnnotationTests(MvcTestFixture<StartupDataAnnotations> fixture)
        {
            _fixture = fixture;
        }

        private const string EnumUrl = "http://localhost/Enum/Enum";

        private void ConfigureServices(IWebHostBuilder builder, CompatibilityVersion version)
        {
            builder.UseStartup<StartupDataAnnotations>();
            builder.ConfigureServices(
                services =>
                {
                    services.AddMvc()
                        .AddViewLocalization()
                        .AddDataAnnotationsLocalization((options) => {
                            options.DataAnnotationLocalizerProvider =
                                (modelType, stringLocalizerFactory) => stringLocalizerFactory.Create(typeof(SingleType));
                        });
                    services.Configure<MvcCompatibilityOptions>(options => options.CompatibilityVersion = version);
                });
        }

        [Fact]
        public async Task DataAnnotationLocalizionOfEnums_FromDataAnnotationLocalizerProvider()
        {
            // Arrange
            var factory = _fixture.WithWebHostBuilder(builder => ConfigureServices(builder, CompatibilityVersion.Latest));
            var client = factory.CreateDefaultClient();

            // Act
            var response = await client.GetAsync(EnumUrl);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("FirstOptionDisplay from singletype", content);
        }
    }
}
