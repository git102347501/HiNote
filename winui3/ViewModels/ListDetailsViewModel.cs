using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using HiNote.Contracts.ViewModels;
using HiNote.Service.Contracts.Services;
using HiNote.Service.Models;
using HiNote.Service.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Windows.UI.WebUI;

namespace HiNote.ViewModels;

public class ListDetailsViewModel : ObservableRecipient
{
    private readonly INoteService _noteService;
    private readonly IOpenAIService _openAIService;
    private GetCategoryOutput? _selected;
    private NoteViewModel? _data = new NoteViewModel();
    public RichEditBox RichEditBox;
    public WebView2 MdWebView;

    /// <summary>
    /// 是否全屏模式
    /// </summary>
    private bool _full = false;

    #region Visibility 

    private GridLength _categoryWidth = new GridLength(205);
    public GridLength CategoryWidth
    {
        get => _categoryWidth;
        set => SetProperty(ref _categoryWidth, value);
    }
    private GridLength _noteWidth = new GridLength(265);
    public GridLength NoteWidth
    {
        get => _noteWidth;
        set => SetProperty(ref _noteWidth, value);
    }

    private bool isstart = false;
    public bool IsStart
    {
        get => isstart;
        set => SetProperty(ref isstart, value);
    }


    private bool isError = false;
    public bool IsError
    {
        get => isError;
        set => SetProperty(ref isError, value);
    }


    private Microsoft.UI.Xaml.Visibility noteListVisibility = Visibility.Visible;
    public Microsoft.UI.Xaml.Visibility NoteListVisibility
    {
        get => noteListVisibility;
        set => SetProperty(ref noteListVisibility, value);
    }
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
    private Microsoft.UI.Xaml.Visibility noteListNullVisibility = Visibility.Visible;
    public Microsoft.UI.Xaml.Visibility NoteListNullVisibility
    {
        get => noteListNullVisibility;
        set => SetProperty(ref noteListNullVisibility, value);
    }

    private Visibility noteSaveVisibility = Visibility.Visible;
    public Visibility NoteSaveVisibility
    {
        get => noteSaveVisibility;
        set => SetProperty(ref noteSaveVisibility, value);
    }

    private Visibility noteLoadingVisibility = Visibility.Collapsed;
    public Visibility NoteLoadingVisibility
    {
        get => noteLoadingVisibility;
        set => SetProperty(ref noteLoadingVisibility, value);
    }

    private Visibility noteContentLoadingVisibility = Visibility.Collapsed;
    public Visibility NoteContentLoadingVisibility
    {
        get => noteContentLoadingVisibility;
        set => SetProperty(ref noteContentLoadingVisibility, value);
    }

    private Microsoft.UI.Xaml.Visibility noteChoiceVisibility = Visibility.Visible;
    public Microsoft.UI.Xaml.Visibility NoteChoiceVisibility
    {
        get => noteChoiceVisibility;
        set => SetProperty(ref noteChoiceVisibility, value);
    }

    private Microsoft.UI.Xaml.Visibility noteEditVisibility = Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility NoteEditVisibility
    {
        get => noteEditVisibility;
        set => SetProperty(ref noteEditVisibility, value);
    }

    private Microsoft.UI.Xaml.Visibility noteMdEditVisibility = Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility NoteMdEditVisibility
    {
        get => noteMdEditVisibility;
        set => SetProperty(ref noteMdEditVisibility, value);
    }
    #endregion

    private bool noteListLoading = false;
    public bool NoteListLoading
    {
        get => noteListLoading;
        set => SetProperty(ref noteListLoading, value);
    }


    private bool noteCategoryLoading = false;
    public bool NoteCategoryLoading
    {
        get => noteCategoryLoading;
        set => SetProperty(ref noteCategoryLoading, value);
    }

    public int SkipCount = 0;

    public int MaxResultCount = 1000;

    public string Sorting = "id";

    public string CategorySorting = "orderno";

    private string _sortIcon = "Up";
    public string SortIcon
    {
        get => _sortIcon;
        set => SetProperty(ref _sortIcon, value);
    }

    private Microsoft.UI.Xaml.Visibility _fullBtn = Visibility.Visible;
    public Microsoft.UI.Xaml.Visibility FullBtn
    {
        get => _fullBtn;
        set => SetProperty(ref _fullBtn, value);
    }

    private Microsoft.UI.Xaml.Visibility _fullBackBtn = Visibility.Collapsed;
    public Microsoft.UI.Xaml.Visibility FullBackBtn
    {
        get => _fullBackBtn;
        set => SetProperty(ref _fullBackBtn, value);
    }

    private int _selectIndex = 0;
    public int SelectIndex
    {
        get => _selectIndex;
        set => SetProperty(ref _selectIndex, value);
    }

    public NoteViewModel Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }

    public Guid MainId;

    public Guid MainNoteId;

    public GetCategoryOutput? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ObservableCollection<NoteItem> NoteItems { get; private set; } = new ObservableCollection<NoteItem>();

    public List<GetCategoryOutput> NoteCategoryItems { get; private set; }

    public ObservableCollection<ExplorerItem> DataSource { get; private set; } = new ObservableCollection<ExplorerItem>();


    public ListDetailsViewModel(INoteService noteService, IOpenAIService openAIService)
    {
        _noteService = noteService;
        _openAIService = openAIService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            await this.LoadCategoryListAsync();
        }
    }

    public ExplorerItem? GetMainItem(Guid id)
    {
        ExplorerItem? data = null;
        foreach (var item in DataSource)
        {
            data = GetMainItem(item, id);
            if (data != null)
            {
                return data;
            }
            else
            {
                continue;
            }
        }
        return data;
    }

    private ExplorerItem? GetMainItem(ExplorerItem items, Guid id)
    {
        var data = items.Id == id ? items : null;
        if (data == null)
        {
            return items.Children.FirstOrDefault(c => c.Id == id);
        } 
        return data;
    }

    public void Clear()
    {
        this.Data.Title = "";
        this.Data.Content = "";
        this.MainId = default;
        this.MainNoteId = default;
        if (this.DataSource != null)
        {
            this.DataSource.Clear();
        }
        if (this.NoteCategoryItems != null)
        {
            this.NoteCategoryItems.Clear();
        }
        if (this.NoteItems != null)
        {
            this.NoteItems.Clear();
        }
        this.RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, "");
    }

    /// <summary>
    /// 加载列表
    /// </summary>
    /// <returns></returns>
    public async Task LoadNoteList(bool clearData = true)
    {
        this.NoteListLoading = true;
        NoteListVisibility = Visibility.Collapsed;
        NoteListNullVisibility = Visibility.Collapsed;
        this.NoteItems.Clear();
        var data = await _noteService.GetNoteListAsync(new GetNoteListInput()
        {
            Id = MainId,
            SkipCount = 0,
            Sorting = this.Sorting,
            MaxResultCount = 1000
        });

        if (data.IsSuccess && data.Data.Items.Count > 0)
        {
            foreach (var item in data.Data.Items)
            {
                NoteItems.Add(new NoteItem()
                {
                    Id = item.Id,
                    CategoryId = item.CategoryId,
                    Title = item.Title
                });
            }
            NoteListVisibility = Visibility.Visible;
            NoteListNullVisibility = Visibility.Collapsed;
            var firstdata = data.Data.Items.FirstOrDefault();
            if (firstdata != null)
            {
                this.MainNoteId = firstdata.Id;
                await LoadNoteDetail();
            }
        }
        else
        {
            NoteListVisibility = Visibility.Collapsed;
            NoteListNullVisibility = Visibility.Visible;
            this.MainNoteId = default;
            if (this.Data != null && clearData)
            {
                this.Data.Title = "";
                this.Data.Content = "";
            }

            RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, "");
        }
        this.NoteListLoading = false;
    }

    /// <summary>
    /// 加载详情
    /// </summary>
    /// <returns></returns>
    public async Task LoadNoteDetail()
    {
        var data = await _noteService.GetAsync(MainNoteId);

        if (data.IsSuccess && data.Data != null)
        {
            SelectIndex = this.NoteItems.ToList().FindIndex(c => c.Id == MainNoteId);
            var content = Encoding.UTF8.GetString(data.Data.Content);
            Data = new NoteViewModel()
            {
                Title = data.Data.Title,
                CategoryId = data.Data.CategoryId,
                Content = content,
                CreationTime = data.Data.CreationTime,
                LastUpdateTime = data.Data.LastUpdateTime,
                EditType = data.Data.EditType,
                MdUrl = "https://hinote.magicalconch.com/?id=" + MainNoteId
            };
            //Data.Title = data.Data.Title;
            //Data.CategoryId = data.Data.CategoryId;
            //Data.Content = content;
            //Data.CreationTime = data.Data.CreationTime;
            //Data.LastUpdateTime = data.Data.LastUpdateTime;
            //Data.EditType = data.Data.EditType;
            if (Data.EditType == 0)
            {
                this.SelectRichEdit();
                RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, content);
            }
            else if(Data.EditType == 1)
            {
                this.SelectMdEdit();
            } 
            else
            {
                RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, content);
                this.SelectChoice();
            }
        } 
        else
        {
            this.Data = new NoteViewModel();
            RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, "");
        }
    }

    public void SelectRichEdit()
    {
        NoteEditVisibility = Visibility.Visible;
        NoteMdEditVisibility = Visibility.Collapsed;
        NoteChoiceVisibility = Visibility.Collapsed;
        this.Data.EditType = 0;
    }

    public void SelectMdEdit()
    {
        NoteEditVisibility = Visibility.Collapsed;
        NoteMdEditVisibility = Visibility.Visible;
        NoteChoiceVisibility = Visibility.Collapsed;
        this.Data.EditType = 1;
        this.RefWebView();
    }

    public void RefWebView()
    {
        this.Data.MdUrl = "https://hinote.magicalconch.com/?id=" + MainNoteId;
        MdWebView.Source = new Uri(this.Data.MdUrl);
    }

    public void SelectChoice()
    {
        NoteEditVisibility = Visibility.Collapsed;
        NoteMdEditVisibility = Visibility.Collapsed;
        NoteChoiceVisibility = Visibility.Visible;
        this.Data.EditType = -1;
    }

    /// <summary>
    /// 加载目录
    /// </summary>
    /// <returns></returns>
    public async Task LoadCategoryListAsync()
    {
        this.NoteCategoryNullVisibility = Visibility.Collapsed;
        this.NoteCategoryVisibility = Visibility.Collapsed;
        this.NoteCategoryLoading = true;
        this.MainId = default;

        var data = await _noteService.GetCategroyListAsync(new GetNoticeCategoryListInput()
        {
            SkipCount = this.SkipCount,
            MaxResultCount = this.MaxResultCount,
            Sorting = this.CategorySorting
        });

        if (data.IsSuccess && data.Data.Items.Count > 0)
        {
            this.NoteCategoryNullVisibility = Visibility.Collapsed;
            this.NoteCategoryVisibility = Visibility.Visible;

            this.DataSource.Clear();
            this.NoteCategoryItems = data.Data.Items;
            var result = data.Data.Items.Where(c => c.ParentId == null).ToList();
            var msg = "";
            foreach (var itemData in result)
            {
                this.DataSource.Add(new ExplorerItem()
                {
                    Name = itemData.Name,
                    Id = itemData.Id
                });
                msg += itemData.Name + "," +  itemData.OrderNo;
            }
            foreach (var itemData in this.DataSource)
            {
                GetChildernList(data.Data.Items, itemData);
            }
            Debug.WriteLine(msg);
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
        data.Where(c => c.ParentId == list.Id).OrderBy(c=> c.OrderNo).Select(c => new ExplorerItem()
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

    /// <summary>
    /// 保存当前日记
    /// </summary>
    /// <returns></returns>
    public async void SaveNoteCategory(Guid categoryId)
    {
        var data = await _noteService.UpdateAsync(new UpdateNoteInput()
        {
            Id = this.MainNoteId,
            CategoryId = categoryId,
            Content = this.Data.Content,
            Title = this.Data.Title
        });
        if (data.IsSuccess)
        {
            await this.LoadNoteList();
        }
    }

    /// <summary>
    /// 保存当前日记
    /// </summary>
    /// <returns></returns>
    public async Task<ResultDto> SaveNote(string content)
    {
        if (this.Data == null || string.IsNullOrWhiteSpace(this.Data?.Title))
        {
            return new ResultDto("标题不能为空", false);
        }
        if (this.MainNoteId != default)
        {
            if (this.MainId == default && this.Data != null)
            {
                var res = await this.AddNote(Data.Title, content, "", Data.EditType);
                return new ResultDto(res.Message, res.IsSuccess);
            }
            else
            {
                this.NoteSaveVisibility = Visibility.Collapsed;
                this.NoteLoadingVisibility = Visibility.Visible;
                var data = await _noteService.UpdateAsync(new UpdateNoteInput()
                {
                    Id = this.MainNoteId,
                    CategoryId = this.Data?.CategoryId ?? this.MainId,
                    Content = content,
                    Title = this.Data.Title,
                    EditType = this.Data.EditType
                });
                if (data.IsSuccess)
                {
                    // 如果标题被修改，修改列表内标题
                    var notedata = this.NoteItems.FirstOrDefault(c => c.Id == this.MainNoteId);
                    if (notedata != null)
                    {
                        notedata.Title = this.Data.Title;
                    }
                }
                this.NoteSaveVisibility = Visibility.Visible;
                this.NoteLoadingVisibility = Visibility.Collapsed;
                return data;
            }
        } 
        else if (this.MainId != default && this.Data != null)
        {
            this.NoteSaveVisibility = Visibility.Collapsed;
            this.NoteLoadingVisibility = Visibility.Visible;
            var data = await AddNote(this.Data.Title, content, "", Data.EditType);
            this.NoteSaveVisibility = Visibility.Visible;
            this.NoteLoadingVisibility = Visibility.Collapsed;
            return new ResultDto(data.Message, data.IsSuccess);
        }
        else if (this.Data != null && !string.IsNullOrWhiteSpace(this.Data.Title))
        {
#warning 国际化
            var category = this.NoteCategoryItems?.FirstOrDefault(c => c.Name == "未归类");
            if (category != null)
            {
                this.MainId = category.Id;
            } 
            else
            {
                await SaveCategory("未归类", null, false);
            }
            this.NoteSaveVisibility = Visibility.Collapsed;
            this.NoteLoadingVisibility = Visibility.Visible;
            var data = await AddNote(this.Data.Title, content, "", Data.EditType);
            this.NoteSaveVisibility = Visibility.Visible;
            this.NoteLoadingVisibility = Visibility.Collapsed;
            return new ResultDto(data.Message, data.IsSuccess);
        }
#warning 国际化
        return new ResultDto("请输入标题和正文后保存", false);
    }

    /// <summary>
    /// 更新排序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    public async Task UpdateCategoryOrderNoAsync()
    {
        var data = new List<UpdateCategoryOrderNoDto>();
        var index = 1;
        var msg = "";
        foreach (var item in this.DataSource)
        {
            data.Add(new UpdateCategoryOrderNoDto()
            {
                Id = item.Id,
                OrderNo = index
            });
            msg += item.Name + ":" + index + ",";
            index++;
            data.AddRange(UpdateCategoryOrderNoAsync(item));
        }
        Debug.WriteLine("UpdateCategoryOrderNo:" + msg);
        await _noteService.UpdateCategoryOrderNoAsync(new UpdateCategoryOrderNoInput()
        {
            Items = data
        });
    }

    public List<UpdateCategoryOrderNoDto> UpdateCategoryOrderNoAsync(ExplorerItem data)
    {
        var res = new List<UpdateCategoryOrderNoDto>();
        var msg = "";
        if (data.Children.Count > 0)
        {
            var index = 1;
            foreach (var item in data.Children)
            {
                res.Add(new UpdateCategoryOrderNoDto()
                {
                    Id = item.Id,
                    OrderNo = index
                });
                msg += item.Name + ":" + index + ",";
                index++;
                res.AddRange(UpdateCategoryOrderNoAsync(item));
            }
        }
        Debug.WriteLine("UpdateCategoryItemOrderNo:" + msg);
        return res;
    }

    /// <summary>
    /// 更新排序
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    public List<UpdateCategoryOrderNoDto> UpdateCategoryOrderNoAsync(ObservableCollection<ExplorerItem> items)
    {
        var data = new List<UpdateCategoryOrderNoDto>();
        var index = 1;
        foreach (var item in items)
        {
            data.Add(new UpdateCategoryOrderNoDto()
            {
                Id = item.Id,
                OrderNo = index
            });
            index++;
        }
        return data;
    }

    /// <summary>
    /// 保存类别
    /// </summary>
    /// <returns></returns>
    public async Task SaveCategory(string name, Guid? parentId = null, bool clearData = true)
    {
        var res = await _noteService.AddCategoryAsync(new AddCategoryInput()
        {
            Name = name,
            ParentId = parentId
        });
        if (res.IsSuccess)
        {
            // 取消所有勾选
            var list = this.DataSource.Where(c => c.IsSelected == true).ToList();
            if (list.Count > 0)
            {
                list.ForEach(c => c.IsSelected = false);
            }
            if (parentId.HasValue)
            {
                var data = GetMainItem(parentId.Value);
                if (data != null)
                {
                    data.IsExpanded = true;
                    data.Children.Add(new ExplorerItem()
                    {
                        Id = res.Data.Id,
                        IsSelected = true,
                        Name = res.Data.Name
                    });
                    this.MainId = res.Data.Id;
                    await LoadNoteList(clearData);
                }
            } 
            else
            {
                // 选择当前节点
                if (this.DataSource.Count < 1)
                {
                    await this.LoadCategoryListAsync();
                    var data = this.DataSource.FirstOrDefault();
                    if (data != null)
                        data.IsSelected = true;
                } 
                else
                {
                    this.DataSource.Add(new ExplorerItem()
                    {
                        Id = res.Data.Id,
                        IsSelected = true,
                        Name = res.Data.Name
                    });
                }
                this.MainId = res.Data.Id;
                await LoadNoteList(clearData);
            }
        }
    }

    public async void SetMainId(Guid id)
    {
        this.MainId = id;
        this.DataSource.Where(c=> c.IsSelected == true || c.IsExpanded == true).ToList().ForEach(c =>
        {
            c.IsSelected = false;
            c.IsExpanded = false;
        });
        // 找父节点
        var parentItems = this.NoteCategoryItems.Where(c => c.Id == id).FirstOrDefault();
        if (parentItems != null && parentItems.ParentId.HasValue)
        {
            var parentdata = GetMainItem(parentItems.ParentId.Value);
            if (parentdata != null)
            {
                // 展开父节点
#warning 如果超过两层，还要往上逐级展开
                parentdata.IsExpanded = true;
            }
        }
        var data = GetMainItem(id);
        if (data != null)
        {
            data.IsSelected = true;
        }
    }

    /// <summary>
    /// 保存类别
    /// </summary>
    /// <returns></returns>
    public async Task UpdateCategory(Guid? id, string name, Guid? parentId = null)
    {
        var currentId = id ?? this.MainId;
        var res = await _noteService.UpdateCategoryAsync(new UpdateCategoryInput()
        {
            Id = id ?? this.MainId,
            Name = name,
            ParentId = parentId
        });
        if (res.IsSuccess)
        {
            var data = this.DataSource.FirstOrDefault(c => c.Id == this.MainId);
            if (data != null)
            {
                data.Name = name;
            }
        }
    }

    /// <summary>
    /// 保存类别
    /// </summary>
    /// <returns></returns>
    public async Task UpdateCategoryName(Guid? id, string name)
    {
        var currentId = id ?? this.MainId;
        var res = await _noteService.UpdateCategoryNameAsync(new UpdateCategoryInput()
        {
            Id = id ?? this.MainId,
            Name = name
        });
        if (res.IsSuccess)
        {
            var data = GetMainItem(currentId);
            if (data != null)
            {
                data.Name = name;
            }
        }
    }

    /// <summary>
    /// 删除类别
    /// </summary>
    /// <returns></returns>
    public async Task DeleteCategory(Guid? id)
    {
        var currentId = id ?? this.MainId;
        var res = await _noteService.DeleteCategoryAsync(currentId);
        if (res.IsSuccess)
        {
            await this.LoadCategoryListAsync();
        }
    }

    /// <summary>
    /// 获取AI返回文本
    /// </summary>
    /// <param name="content"></param>
    /// <param name="instruction"></param>
    /// <returns></returns>
    public async Task<ResultDto<CreateEditOuput>> GetAITextAsync(string content, string instruction)
    {
        IsError = false;
        IsStart = true;
        var res = await _openAIService.CreateEditAsync(new CreateEditInput()
        {
            Content = content,
            Instruction = instruction,
            Mode = 4
        });
        if (res.IsSuccess)
        {
            var rescontent = "";
            foreach (var item in res.Data.ChoiseList)
            {
                rescontent += item + Environment.NewLine;
            }
            if (this.Data.EditType == 1)
            {
                this.Data.Content = rescontent;
            } 
            else
            {
                this.RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, rescontent);
            }
        }
        IsError = !res.IsSuccess;
        IsStart = false;
        return res;
    }

    /// <summary>
    /// 保存文章
    /// </summary>
    /// <returns></returns>
    public async Task<ResultDto<AddNoteOutput>> AddNote(string title, string content, string tags, int edittype)
    {
        var res = await _noteService.AddAsync(new AddNoteInput()
        {
            Title = title,
            Content = "",
            Tags = tags,
            CategoryId = this.MainId,
            EditType = edittype
        });
        if (res.IsSuccess)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                await _noteService.UpdateAsync(new UpdateNoteInput()
                {
                    Id = res.Data.Id,
                    CategoryId = res.Data.CategoryId,
                    Content = content,
                    Title = res.Data.Title,
                    EditType = edittype,
                    Tags = tags
                });
            }
            if (this.NoteItems.Count < 1)
            {
                await this.LoadNoteList();
            }
            else
            {
                this.NoteItems.Add(new NoteItem()
                {
                    Id = res.Data.Id,
                    CategoryId = res.Data.CategoryId,
                    Title = res.Data.Title
                });
            }
            MainNoteId = res.Data.Id;
            await LoadNoteDetail();
            if (edittype < 0)
            {
                SelectChoice();
            }
        }
        return res;
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    /// <param name="enable"></param>
    public void FullScreen()
    {
        this._full = !this._full;
        if (this._full)
        {
            this.CategoryWidth = new GridLength(0);
            this.NoteWidth = new GridLength(0);
            this.FullBtn = Visibility.Collapsed;
            this.FullBackBtn = Visibility.Visible;
        } 
        else
        {
            this.CategoryWidth = new GridLength(205);
            this.NoteWidth = new GridLength(205);
            this.FullBtn = Visibility.Visible;
            this.FullBackBtn = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 删除文章
    /// </summary>
    /// <returns></returns>
    public async Task DeleteNote(Guid? id = null)
    {
        var currentId = id ?? this.MainNoteId;
        var res = await _noteService.DeleteAsync(currentId);
        if (res.IsSuccess)
        {
            var data = this.NoteItems.FirstOrDefault(c => c.Id == currentId);
            if (data != null)
            {
                this.NoteItems.Remove(data);
                var lastData = this.NoteItems.LastOrDefault();
                if (lastData != null)
                {
                    this.MainNoteId = lastData.Id;
                }
                else
                {
                    this.MainNoteId = default;
                    this.Data = new NoteViewModel();
                    this.RichEditBox.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, "");
                }
            }
        }
    }
}

/// <summary>
/// 目录树节点对象
/// </summary>
public class NoteItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    private string _title;

    public string Title
    {
        get { return _title; }
        set
        {
            if (_title != value)
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// 目录树节点对象
/// </summary>
public class ExplorerItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public enum ExplorerItemType { Folder, File };

    private string _name;

    public string Name
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
    }
    public Guid Id
    {
        get; set;
    }

    public int Grade
    {
        get; set;
    }

    private bool m_isSelected;
    public bool IsSelected
    {
        get { return m_isSelected; }

        set
        {
            if (m_isSelected != value)
            {
                m_isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
    }

    public ExplorerItemType Type
    {
        get; set;
    }
    private ObservableCollection<ExplorerItem> m_children;
    public ObservableCollection<ExplorerItem> Children
    {
        get
        {
            if (m_children == null)
            {
                m_children = new ObservableCollection<ExplorerItem>();
            }
            return m_children;
        }
        set
        {
            m_children = value;
        }
    }

    private bool m_isExpanded;

    /// <summary>
    /// 是否展开
    /// </summary>
    public bool IsExpanded
    {
        get
        {
            return m_isExpanded;
        }
        set
        {
            if (m_isExpanded != value)
            {
                m_isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate
    {
        get; set;
    }
    public DataTemplate FileTemplate
    {
        get; set;
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        return FolderTemplate;
        //= ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
    }
}

public class NoteViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string status)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(status));
        }
    }

    private string title = "";
    public string Title
    {
        get => title;

        set
        {
            title = value;
            OnPropertyChanged("Title");
        }
    }

    private string tags = "";
    public string Tags
    {
        get => tags;

        set
        {
            title = value;
            OnPropertyChanged("Title");
        }
    }

    private int editType = 0;
    public int EditType
    {
        get => editType;

        set
        {
            editType = value;
            OnPropertyChanged("EditType");
        }
    }

    private string mdUrl = "https://hinote.magicalconch.com/?id=";
    public string MdUrl    
    {
        get => mdUrl;

        set
        {
            mdUrl = value;
            OnPropertyChanged("MdUrl");
        }
    }

    private string content = "";
    public string Content
    {
        get => content;

        set
        {
            content = value;
            OnPropertyChanged("Content");
        }
    }

    private Guid categoryId = default;
    public Guid CategoryId
    {
        get => categoryId;

        set
        {
            categoryId = value;
            OnPropertyChanged("CategoryId");
        }
    }

    private DateTime creationTime = default;
    public DateTime CreationTime
    {
        get => creationTime;

        set
        {
            creationTime = value;
            OnPropertyChanged("CreationTime");
        }
    }

    private DateTime? lastUpdateTime = default;
    public DateTime? LastUpdateTime
    {
        get => lastUpdateTime;

        set
        {
            lastUpdateTime = value;
            OnPropertyChanged("LastUpdateTime");
        }
    }
}