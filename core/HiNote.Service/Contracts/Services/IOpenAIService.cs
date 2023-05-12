using HiNote.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Contracts.Services
{
    public interface IOpenAIService
    {
        public Task<ResultDto<CreateEditOuput>> CreateEditAsync(CreateEditInput input);

        public Task<ChatOutput> ChatAsync(List<ChatInput> input);
    }
}
