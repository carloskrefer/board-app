using Auth.Infrastructure.Persistance;
using TestsCommon.Persistance.Factories;

namespace Auth.Infrastructure.Tests.CollectionFixtures;

[CollectionDefinition(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class IntegrationTestsCollection : ICollectionFixture<DefaultTestingWebApplicationFactory<AuthDbContext>>
{
}