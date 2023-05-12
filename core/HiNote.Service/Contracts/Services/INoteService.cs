using HiNote.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Contracts.Services
{
    public interface INoteService
    {
        public Task<ResultDto<AddNoteOutput>> AddAsync(AddNoteInput data);

        public Task<ResultDto> UpdateAsync(UpdateNoteInput data);

        public Task<ResultDto<GetNoteOutput>> GetAsync(Guid id);

        public Task<ResultDto<PagedResultDto<GetCategoryOutput>>> GetCategroyListAsync(GetNoticeCategoryListInput input);

        public Task<ResultDto<PagedResultDto<GetNoteListOutput>>> GetNoteListAsync(GetNoteListInput input);

        /// <summary>
        /// 添加日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<ResultDto<AddCategoryOutput>> AddCategoryAsync(AddCategoryInput data);

        /// <summary>
        /// 删除日记目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResultDto> DeleteCategoryAsync(Guid id);

        /// <summary>
        /// 删除日记目录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ResultDto> DeleteAsync(Guid id);

        /// <summary>
        /// 更新日记目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<ResultDto> UpdateCategoryAsync(UpdateCategoryInput data);

        Task<ResultDto> UpdateCategoryOrderNoAsync(UpdateCategoryOrderNoInput data);

        Task<ResultDto> UpdateCategoryColorAsync(UpdateCategoryColorInput data);

        Task<ResultDto> UpdateCategoryIconAsync(UpdateCategoryIconInput data);

        Task<ResultDto> UpdateCategoryNameAsync(UpdateCategoryInput data);
    }
}
