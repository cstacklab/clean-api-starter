namespace CleanApiStarter.AspNetCore;

public interface IEndpointGroup
{
    static virtual int MajorVersion => 1;

    static virtual string? RoutePrefix => null;

    static abstract void Map(RouteGroupBuilder groupBuilder);
}
