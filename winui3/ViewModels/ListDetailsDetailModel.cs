using CommunityToolkit.Mvvm.ComponentModel;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;

namespace HiNote.ViewModels
{
    public class ListDetailsDetailModel : ObservableRecipient
    {
        private readonly INoteService _noteService;

        public Guid MainId { get; set; }

        public GetNoteOutput Data { get; set; }

        public ListDetailsDetailModel(INoteService sampleDataService)
        {
            _noteService = sampleDataService;
        }

        public async Task LoadData()
        {
            var data = await _noteService.GetAsync(MainId);

            if (data.IsSuccess)
            {
                Data = data.Data;
            }
        }
    }
}
