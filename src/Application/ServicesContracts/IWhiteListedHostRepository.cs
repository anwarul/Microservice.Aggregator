namespace RS.MF.Aggregator.Application.ServicesContracts
{
    public interface IWhiteListedHostRepository
    {
        bool IsWhiteListedHost(string host);
    }
}