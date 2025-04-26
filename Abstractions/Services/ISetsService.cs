using System.Threading.Tasks;

namespace Abstractions.Services;

public interface ISetsService
{
    public Task<bool> AddNewSet(string gtin);
}