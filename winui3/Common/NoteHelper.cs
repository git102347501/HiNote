using HiNote.ViewModels;

namespace HiNote.Common
{
    public static class NoteHelper
    {
        public static void Clear()
        {
            var noteListData = App.GetService<ListDetailsViewModel>();
            noteListData.NoteItems.Clear();
            noteListData.Data = new NoteViewModel();
            noteListData.DataSource.Clear();
        }
    }
}
