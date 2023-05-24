using CommunityToolkit.Mvvm.ComponentModel;
using HiNote.Service.Contracts;
using HiNote.Service.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiNote.ViewModels
{
    public class SelectTreeViewModel : ObservableRecipient
    {
        private Microsoft.UI.Xaml.Visibility noteCategoryNullVisibility = Visibility.Visible;
        public Microsoft.UI.Xaml.Visibility NoteCategoryNullVisibility
        {
            get => noteCategoryNullVisibility;
            set => SetProperty(ref noteCategoryNullVisibility, value);
        }

        private Microsoft.UI.Xaml.Visibility noteCategoryVisibility = Visibility.Collapsed;
        public Microsoft.UI.Xaml.Visibility NoteCategoryVisibility
        {
            get => noteCategoryVisibility;
            set => SetProperty(ref noteCategoryVisibility, value);
        }

        private bool noteCategoryLoading = false;
        public bool NoteCategoryLoading
        {
            get => noteCategoryLoading;
            set => SetProperty(ref noteCategoryLoading, value);
        }


        public ObservableCollection<ExplorerItem> DataSource = new ObservableCollection<ExplorerItem>();
        public ExplorerItem SelectedItem { get; set; }

        private readonly INoteService _noteService;

        public SelectTreeViewModel(INoteService noteService)
        {
            _noteService = noteService;
        }

        /// <summary>
        /// 加载目录
        /// </summary>
        /// <returns></returns>
        public async void LoadCategoryListAsync()
        {
            this.NoteCategoryNullVisibility = Visibility.Collapsed;
            this.NoteCategoryVisibility = Visibility.Collapsed;
            this.NoteCategoryLoading = true;

            var data = await _noteService.GetCategroyListAsync(new GetNoticeCategoryListInput()
            {
                SkipCount = 0,
                MaxResultCount = 1000,
                Sorting = "orderno"
            });

            if (data.IsSuccess && data.Data.Items.Count > 0)
            {
                this.NoteCategoryNullVisibility = Visibility.Collapsed;
                this.NoteCategoryVisibility = Visibility.Visible;

                this.DataSource.Clear();

                for (int i = 0; i < data.Data.Items.Count; i++)
                {
                    var itemData = data.Data.Items[i];
                    if (itemData.ParentId == null)
                    {
                        this.DataSource.Add(new ExplorerItem()
                        {
                            Name = itemData.Name,
                            Id = itemData.Id
                        });
                    }
                }
                foreach (var itemData in this.DataSource)
                {
                    GetChildernList(data.Data.Items, itemData);
                }
            }
            else
            {
                this.NoteCategoryNullVisibility = Visibility.Visible;
                this.NoteCategoryVisibility = Visibility.Collapsed;
            }
            this.NoteCategoryLoading = false;
        }

        private void GetChildernList(List<GetCategoryOutput> data, ExplorerItem list)
        {
            data.Where(c => c.ParentId == list.Id).Select(c => new ExplorerItem()
            {
                Name = c.Name,
                Id = c.Id,
            }).ToList().ForEach(c => list.Children.Add(c));
            if (list.Children.Count > 0)
            {
                foreach (var item in list.Children)
                {
                    GetChildernList(data, item);
                }
            }
        }
    }
}
