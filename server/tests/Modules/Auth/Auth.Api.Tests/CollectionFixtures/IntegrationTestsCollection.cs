using Auth.Infrastructure.Persistance;
using TestsCommon.Persistance.Factories;

namespace Auth.Api.Tests.CollectionFixtures;

[CollectionDefinition(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class IntegrationTestsCollection : ICollectionFixture<DefaultTestingWebApplicationFactory<AuthDbContext>>
{
}