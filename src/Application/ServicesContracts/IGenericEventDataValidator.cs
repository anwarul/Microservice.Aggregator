using RS.MF.Aggregator.Application.Dtos;
using System.Collections.Generic;

namespace RS.MF.Aggregator.Application.ServicesContracts
{
    public interface IGenericEventDataValidator
    {
        IEnumerable<string> Validate(GenericEventData genericEventData);
    }
}