using HiNote.Core.Services;
using HiNote.Service.Contracts;
using HiNote.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HiNote.Service.Services
{
    public class NoteService : BasicService, INoteService
    {
        public Task<ResultDto<AddNoteOutput>> AddAsync(AddNoteInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto<AddCategoryOutput>> AddCategoryAsync(AddCategoryInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> DeleteCategoryAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto<GetNoteOutput>> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto<PagedResultDto<GetCategoryOutput>>> GetCategroyListAsync(GetNoticeCategoryListInput input)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto<PagedResultDto<GetNoteListOutput>>> GetNoteListAsync(GetNoteListInput input)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateAsync(UpdateNoteInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateCategoryAsync(UpdateCategoryInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateCategoryColorAsync(UpdateCategoryColorInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateCategoryIconAsync(UpdateCategoryIconInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateCategoryNameAsync(UpdateCategoryInput data)
        {
            throw new NotImplementedException();
        }

        public Task<ResultDto> UpdateCategoryOrderNoAsync(UpdateCategoryOrderNoInput data)
        {
            throw new NotImplementedException();
        }
    }
}
