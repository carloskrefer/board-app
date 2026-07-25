namespace TestsCommon.Persistance.Interfaces;

public interface IDatabaseSpecificCommandsService
{
    public Task TruncateAllTables();
}