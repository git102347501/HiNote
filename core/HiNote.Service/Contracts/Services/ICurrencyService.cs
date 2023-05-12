using System.Threading.Tasks;
using HiNote.Service.Models;

namespace HiNote.Service.Contracts.Services;

public interface ICurrencyService
{
    Task<ResultDto<GetCurrencyOutput>> GetAsync();
}