using HiNote.Common;
using HiNote.Helpers;
using HiNote.Service.Models;
using HiNote.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.ApplicationModel.Payments;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using mux = Microsoft.UI.Xaml.Controls;

namespace HiNote.Views;

public sealed partial class ListDetailsPage : Page
{
    private Windows.UI.Color currentColor = Microsoft.UI.Colors.Green;
    public ListDetailsViewModel ViewModel
    {
        get;
    }

    public ListDetailsPage()
    {
        ViewModel = App.GetService<ListDetailsViewModel>();
        InitializeComponent();
        ViewModel.RichEditBox = editor;
        ViewModel.MdWebView = mdWebView;
        categoryTreeView.DragItemsCompleted += TreeItemDropping;
    }

    private async void categoryTreeView_ItemInvoked(mux.TreeView sender, mux.TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem != null)
        {
            if (args.InvokedItem is ExplorerItem item)
            {
                ViewModel.MainId = item.Id;
                await ViewModel.LoadNoteList();
            }
        }
    }

    private void FindBoxHighlightMatches()
    {
        FindBoxRemoveHighlights();

        Windows.UI.Color highlightBackgroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightColor"];
        Windows.UI.Color highlightForegroundColor = (Windows.UI.Color)App.Current.Resources["SystemColorHighlightTextColor"];

        string textToFind = findBox.Text;
        if (textToFind != null)
        {
            ITextRange searchRange = editor.Document.GetRange(0, 0);
            while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
            }
        }
    }
    private void FindBoxRemoveHighlights()
    {
        ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush defaultBackground = editor.Background as SolidColorBrush;
        SolidColorBrush defaultForeground = editor.Foreground as SolidColorBrush;

        documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }
    private void Editor_GotFocus(object sender, RoutedEventArgs e)
    {
        editor.Document.GetText(TextGetOptions.UseCrlf, out string currentRawText);

        // reset colors to correct defaults for Focused state
        ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

        if (background != null)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
        }
    }

    private string GetLocalString(string key)
    {
        return new ResourceLoader().GetString(key);
    }

    private void Editor_TextChanged(object sender, RoutedEventArgs e)
    {
        //if (string.IsNullOrWhiteSpace(findBox.Text) && editor.Document.Selection.CharacterFormat.ForegroundColor != currentColor)
        //{
        //    editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        //}
    }

    private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        NoteItem? note = e.AddedItems.FirstOrDefault() as NoteItem;
        if (note != null)
        {
            ViewModel.MainNoteId = note.Id;
            LoadNoteDetail();
        }
    }

    /// <summary>
    /// 加载当前文章详情
    /// </summary>
    private async void LoadNoteDetail()
    {
        if (ViewModel.MainNoteId != default)
        {
            await ViewModel.LoadNoteDetail();
        }
    }

    /// <summary>
    /// 保存文章
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            string content;
            if (ViewModel.Data.EditType == 1)
            {
                content = ViewModel.Data.Content;
            } 
            else
            {
                editor.Document.GetText(TextGetOptions.FormatRtf, out content);
            }

            var data = await ViewModel.SaveNote(content);
            if (!data.IsSuccess)
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = data.Message ?? GetLocalString("NoteListPageNoteSaveFailMsg"),
                    PrimaryButtonText = GetLocalString("NoteListPageNoteSaveFailBtn"),
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
            }
            mdWebView.Reload();
        }
        else
        {
            this.Login();
        }
    }
    private async void MoveNote()
    {
        var dialog = new ContentDialog
        {
            Content = new SelectTreeViewPage(),
            Title = GetLocalString("NoteListPageMoveCategory"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Save"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            var selectTreeViewModel = App.GetService<SelectTreeViewModel>();
            ViewModel.SaveNoteCategory(selectTreeViewModel.SelectedItem.Id);
            ViewModel.SetMainId(selectTreeViewModel.SelectedItem.Id);
            await ViewModel.LoadNoteList();
        }
    }
    private async Task AddNote()
    {
        var inputTextBox = new TextBox
        {
            AcceptsReturn = false,
            Height = 32,
            Text = "",
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = GetLocalString("NoteListPageAddNote"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Save"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            await ViewModel.AddNote(inputTextBox.Text, "", "", -1);
        }
    }

    private async void AddNote_Button_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            await this.AddNote();
        }
        else
        {
            this.Login();
        }
    }

    #region 目录操作
    /// <summary>
    /// 刷新目录事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void RefCategory_Button_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            await ViewModel.LoadCategoryListAsync();
        }
        else
        {
            this.Login();
        }
    }
    /// <summary>
    /// 添加目录事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddCategory_Button_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            await this.AddCategory();
        }
        else
        {
            this.Login();
        }
    }
    /// <summary>
    /// 添加目录
    /// </summary>
    /// <returns></returns>
    private async Task AddCategory(Guid? parentId = null)
    {

        var inputTextBox = new TextBox
        {
            AcceptsReturn = false,
            Height = 32,
            Text = "",
            PlaceholderText = parentId.HasValue ? GetLocalString("NoteListPageAddCategoryChild") : GetLocalString("NoteListPageAddCategory")
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = GetLocalString("NoteListPageAddCategoryMsg"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Save"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            await ViewModel.SaveCategory(inputTextBox.Text, parentId);
        }
    }

    /// <summary>
    /// 目录右击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeViewItem_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint((UIElement)sender).Properties;
        if (properties.IsRightButtonPressed)
        {
            MenuFlyout mf = new MenuFlyout();
            var item = new MenuFlyoutItem()
            {
                Text = GetLocalString("NoteListPageAddCategoryChildMsg"),
            };
            item.Click += new RoutedEventHandler(AddCategory);
            mf.Items.Add(item);
            var item1 = new MenuFlyoutItem()
            {
                Text = GetLocalString("Edit"),
            };
            item1.Click += new RoutedEventHandler(EditCategory);
            mf.Items.Add(item1);
            var item2 = new MenuFlyoutItem()
            {
                Text = GetLocalString("Delete"),
            };
            item2.Click += new RoutedEventHandler(DeleteCategory);
            mf.Items.Add(item2);
            Microsoft.UI.Input.PointerPoint pp = e.GetCurrentPoint((UIElement)sender);
            var ptElement = new Point(pp.Position.X, pp.Position.Y);
            mf.ShowAt((FrameworkElement)sender, ptElement);
        }
    }

    private async void AddCategory(object sender, RoutedEventArgs e)
    {
        var dc = ((FrameworkElement)e.OriginalSource).DataContext;
        var itemData = dc as ExplorerItem;
        if (itemData == null)
        {
            return;
        }
        await this.AddCategory(itemData.Id);
    }

    private async void EditCategory(object sender, RoutedEventArgs e)
    {
        var dc = ((FrameworkElement)e.OriginalSource).DataContext;
        var itemData = dc as ExplorerItem;
        if (itemData == null)
        {
            return;
        }
        ViewModel.MainId = itemData.Id;
        var inputTextBox = new TextBox
        {
            AcceptsReturn = false,
            Height = 32,
            Text = itemData.Name,
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = GetLocalString("NoteListPageEditCategoryMsg"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Save"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            await ViewModel.UpdateCategoryName(null, inputTextBox.Text);
        }
    }
    private async void DeleteCategory(object sender, RoutedEventArgs e)
    {
        var dc = ((FrameworkElement)e.OriginalSource).DataContext;
        var itemData = dc as ExplorerItem;
        if (itemData == null)
        {
            return;
        }
        ContentDialog dialog = new ContentDialog();
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = GetLocalString("NoteListPageDeleteCategoryMsg") + itemData.Name + "?";
        dialog.PrimaryButtonText = GetLocalString("Confirm");
        dialog.CloseButtonText = GetLocalString("Cancel");
        dialog.DefaultButton = ContentDialogButton.Primary;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.DeleteCategory(itemData.Id);
            LoadNoteDetail();
        }
    }
    #endregion

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            DeleteNoteAsync();
        }
        else
        {
            this.Login();
        }
    }
    private async void DeleteNoteAsync()
    {
        ContentDialog dialog = new ContentDialog();
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = GetLocalString("NoteListPageDeleteCategoryMsg") + ViewModel.Data.Title + "?";
        dialog.PrimaryButtonText = GetLocalString("Confirm");
        dialog.CloseButtonText = GetLocalString("Cancel");
        dialog.DefaultButton = ContentDialogButton.Primary;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.DeleteNote(ViewModel.MainNoteId);
            LoadNoteDetail();
        }
    }
    private void MoveNote(object sender, RoutedEventArgs e)
    {
        MoveNote();
    }
    private void DeleteNote(object sender, RoutedEventArgs e)
    {
        DeleteNoteAsync();
    }
    private void NoteList_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint((UIElement)sender).Properties;
        if (properties.IsRightButtonPressed)
        {
            MenuFlyout mf = new MenuFlyout();
            var item1 = new MenuFlyoutItem()
            {
                Text = GetLocalString("NoteListMoveCategoryText"),
            };
            item1.Click += new RoutedEventHandler(MoveNote);
            mf.Items.Add(item1);
            var item2 = new MenuFlyoutItem()
            {
                Text = GetLocalString("Delete"),
            };
            item2.Click += new RoutedEventHandler(DeleteNote);
            mf.Items.Add(item2);
            Microsoft.UI.Input.PointerPoint pp = e.GetCurrentPoint((UIElement)sender);
            var ptElement = new Point(pp.Position.X, pp.Position.Y);
            mf.ShowAt((FrameworkElement)sender, ptElement);
        }
    }

    private async void SortButton_Click(object sender, RoutedEventArgs e)
    {
        var shellViewModel = App.GetService<ShellViewModel>();
        if (shellViewModel.IsLogin)
        {
            if (this.ViewModel.SortIcon == "Up")
            {
                this.ViewModel.SortIcon = "Download";
                this.ViewModel.Sorting = "id Desc";
            }
            else
            {
                this.ViewModel.SortIcon = "Up";
                this.ViewModel.Sorting = "id";
            }
            await ViewModel.LoadNoteList();
        }
        else
        {
            this.Login();
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    private void Login()
    {
        var newWindow = WindowHelper.CreateWindow();
        var rootPage = new LoginPage();
        rootPage.RequestedTheme = ThemeHelper.RootTheme;
        newWindow.Content = rootPage;
        newWindow.SetWindowSize(430, 440);
        newWindow.SetIsAlwaysOnTop(true);
        newWindow.SetWindowPresenter(AppWindowPresenterKind.Overlapped);
        newWindow.ExtendsContentIntoTitleBar = true;
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(newWindow.GetWindowHandle());
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
        if (appWindow is not null)
        {
            DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                appWindow.Move(CenteredPosition);
            }
        }
        newWindow.Activate();

    }

    private void BoldButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
    }

    private void ItalicButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
    }

    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        // Extract the color of the button that was clicked.
        Button clickedColor = (Button)sender;
        var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
        // 黑色主题白色，白色主题黑色
        var color = ((Microsoft.UI.Xaml.Media.SolidColorBrush)rectangle.Fill).Color;

        editor.Document.Selection.CharacterFormat.ForegroundColor = color;

        fontColorButton.Flyout.Hide();
        editor.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
        currentColor = color;
    }

    private void FontIncreaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (editor.Document.Selection.CharacterFormat.Size > 0)
        {
            editor.Document.Selection.CharacterFormat.Size += 1;
        }

        fontColorButton.Flyout.Hide();
        editor.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
    }

    private void FontDecreaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (editor.Document.Selection.CharacterFormat.Size > 0)
        {
            editor.Document.Selection.CharacterFormat.Size -= 1;
        } 

        fontColorButton.Flyout.Hide();
        editor.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
    }

    /// <summary>
    /// 切换全屏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FullScreenButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.FullScreen();
    }

    private async void TreeItemDropping(TreeView sender, TreeViewDragItemsCompletedEventArgs e)
    {
        if (e.Items.Count > 0)
        {
            Debug.WriteLine("触发拖拽事件:" + JsonConvert.SerializeObject(e));
            var data = e.Items.FirstOrDefault() as ExplorerItem;

            if (data != null)
            {
                if (e.NewParentItem != null)
                {
                    // 被拖动到子级
                    var parentItem = e.NewParentItem as ExplorerItem;
                    if (parentItem != null)
                    {
                        var categoryData = ViewModel.NoteCategoryItems.FirstOrDefault(c => c.Id == data.Id);
                        if (categoryData != null && categoryData.ParentId != parentItem.Id)
                        {
                            Debug.WriteLine("更改" + data.Name + "父级ID：" + parentItem.Name);
                            await ViewModel.UpdateCategory(data.Id, data.Name, parentItem.Id);
                        } 
                    }
                }
                else
                {
                    // 被拖动到同级
                    var categoryData = ViewModel.NoteCategoryItems.FirstOrDefault(c=> c.Id == data.Id);
                    // 被拖动到上级
                    var newParent = ViewModel.GetMainItem(data.Id);
                    if ((newParent == null || newParent.Id == data.Id) && categoryData != null && categoryData.ParentId.HasValue)
                    {
                        Debug.WriteLine("更改" + data.Name + "迁移到顶层");
                        await ViewModel.UpdateCategory(data.Id, data.Name, null);
                    }
                    else if (newParent != null && categoryData != null && newParent.Id != categoryData.ParentId && newParent.Id != categoryData.Id)
                    {
                        Debug.WriteLine("更改" + data.Name + "父级ID：" + newParent.Name);
                        await ViewModel.UpdateCategory(data.Id, data.Name, newParent.Id);
                    }
                }
                await ViewModel.UpdateCategoryOrderNoAsync();
            }
        }
    }

    private void RichButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SelectRichEdit();
    }

    private void MdButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SelectMdEdit();
    }

    private async void ChoiceButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Data.EditType == 1 && string.IsNullOrWhiteSpace(ViewModel.Data.Content))
        {
            ViewModel.SelectChoice();
            return;
        }
        var dialog = new ContentDialog
        {
            Title = GetLocalString("SelectEditDialogTitle"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Confirm"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            if (ViewModel.Data.EditType == 0)
            {
                editor.Document.SetText(TextSetOptions.FormatRtf, "");
            }
            else if (ViewModel.Data.EditType == 1)
            {
                ViewModel.Data.Content = "";
            }
            ViewModel.SelectChoice();
        }
    }

    private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            await ViewModel.SaveNote(ViewModel.Data.Content);
            ViewModel.RefWebView();
        }
    }

    private async void AiButton_Click(object sender, RoutedEventArgs e)
    {
        var content = "";
        if (this.ViewModel.Data.EditType == 1)
        {
            content = mdTextBox.SelectedText;
            if (string.IsNullOrWhiteSpace(content))
            {
                content = ViewModel.Data.Content;
            }
        } 
        else
        {
            content = editor.Document.Selection.Text;
            if (string.IsNullOrWhiteSpace(content))
            {
                editor.Document.GetText(TextGetOptions.UseCrlf, out content);
            }
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            await new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "询问内容不能为空",
                Content = "选择内容或正文不能为空",
                PrimaryButtonText = "我知道了",
                DefaultButton = ContentDialogButton.Primary
            }.ShowAsync();
            return;
        }
        var inputTextBox = new TextBox
        {
            AcceptsReturn = false,
            Height = 32,
            MaxLength = 100,
            Text = "",
            PlaceholderText = GetLocalString("NoteAIDialogPlaceholderText")
        };
        var dialog = new ContentDialog
        {
            Content = inputTextBox,
            Title = GetLocalString("NoteAIDialogTitle"),
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = GetLocalString("Confirm"),
            SecondaryButtonText = GetLocalString("Cancel"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        var confirmres = await dialog.ShowAsync() == ContentDialogResult.Primary;
        if (confirmres)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "指令不能为空",
                    Content = "AI需要您的具体指令才能明白任务方向",
                    PrimaryButtonText = "我知道了",
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                return;
            }
            var res = await ViewModel.GetAITextAsync(content, inputTextBox.Text);
            if (!res.IsSuccess)
            {
                var confirmres1 = await new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "额度不足",
                    Content = "您的AI询问额度已用完，点击下方充值后即可继续使用",
                    SecondaryButtonText = "确认",
                    PrimaryButtonText = "充值",
                    DefaultButton = ContentDialogButton.Primary
                }.ShowAsync();
                if (confirmres1 == ContentDialogResult.Primary)
                {
                    ShowUserDetail();
                }
            }
        }
    }
    /// <summary>
    /// 展示用户详情
    /// </summary>
    private async void ShowUserDetail()
    {
        var dialog = new ContentDialog
        {
            Content = new UserInfoPage(),
            Title = GetLocalString("UserInfoDialogTitle"),
            IsSecondaryButtonEnabled = true,
            Width = 420,
            PrimaryButtonText = GetLocalString("Logout"),
            SecondaryButtonText = GetLocalString("Close"),
            XamlRoot = this.XamlRoot,
            RequestedTheme = this.ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            UserHepler.LogOut();
            Login();
        }
    }
}
