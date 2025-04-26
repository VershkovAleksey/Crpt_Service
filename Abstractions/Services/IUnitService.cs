using System.Threading.Tasks;
using Domain.Models.Entities;

namespace Abstractions.Services;

public interface IUnitService
{
    public Task<bool> AddNewUnit(string gtin);
}