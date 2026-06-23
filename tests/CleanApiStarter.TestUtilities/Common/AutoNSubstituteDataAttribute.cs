namespace CleanApiStarter.TestUtilities.Common;

public sealed class AutoNSubstituteDataAttribute() : AutoDataAttribute(CreateFixture)
{
    private static IFixture CreateFixture()
    {
        Fixture fixture = new();

        fixture.Customize(new AutoNSubstituteCustomization());

        return fixture;
    }
}
