using System.Threading.Tasks;
using HiNote.Service.Models;

namespace HiNote.Service.Contracts.Services;

public interface IExchangeCodeService
{
    Task<ResultDto> ExchangeAsync(ExchangeCodeInput data);
}