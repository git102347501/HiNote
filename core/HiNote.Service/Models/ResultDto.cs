using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.Service.Models
{

    public class ResultDto
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public ResultDto(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }

    public class ResultDto<T>
    {
        public T Data { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public ResultDto(T data)
        {
            Data = data;
            IsSuccess = true;
        }

        public ResultDto(string message)
        {
            Message = message;
            IsSuccess = false;
        }
    }

    /// <summary>
    /// 列表返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResultDto<T>
    {
        public List<T> Items { get; set; }
    }

    /// <summary>
    /// 分页返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResultDto<T> : ListResultDto<T>
    {
        public long TotalCount { get; set; }

        public PagedResultDto(List<T> data, long count)
        {
            this.Items = data;
            this.TotalCount = count;
        }
    }

    /// <summary>
    /// 分页入参
    /// </summary>
    public class PagedResultRequestDto
    {
        public virtual int SkipCount { get; set; }

        public static int DefaultMaxResultCount { get; set; } = 1000;

        public static int MaxMaxResultCount { get; set; } = 1000;

        public virtual int MaxResultCount { get; set; } = DefaultMaxResultCount;
    }

    /// <summary>
    /// 分页排序入参
    /// </summary>
    public class PagedAndSortedResultRequestDto : PagedResultRequestDto
    {
        public virtual string Sorting { get; set; }
    }
}
