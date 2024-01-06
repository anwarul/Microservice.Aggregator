using MongoDB.Bson;
using MongoDB.Driver;
using RedlimeSolutions.Microservice.Framework;
using RS.MF.Aggregator.Application.ServicesContracts;

namespace RS.MF.Aggregator.Infrastructure.Services
{
    public class WhiteListedHostRepository : IWhiteListedHostRepository
    {
        private readonly INoSqlDbDataContextProvider _noSqlDbDataContext;

        public WhiteListedHostRepository(INoSqlDbDataContextProvider noSqlDbDataContext)
        {
            this._noSqlDbDataContext = noSqlDbDataContext;
        }

        public bool IsWhiteListedHost(string host)
        {
            var whiteListedHostsCollection = _noSqlDbDataContext.GetTenantDataContext().GetCollection<BsonDocument>("WhiteListedHosts");

            var isThereAnyWhiteListedHost = whiteListedHostsCollection.Find(_ => true).Any();

            if (isThereAnyWhiteListedHost == false)
            {
                return true;
            }

            var filter = Builders<BsonDocument>.Filter.Eq("Host", host.ToLower());

            return whiteListedHostsCollection.Find(filter).Any();
        }
    }
}