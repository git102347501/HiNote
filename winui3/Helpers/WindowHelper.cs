using Microsoft.UI.Xaml;

namespace HiNote.Helpers
{
    public class WindowHelper
    {
        static public Window CreateWindow()
        {
            Window newWindow = new Window();
            TrackWindow(newWindow);
            return newWindow;
        }

        static public void TrackWindow(Window window)
        {
            window.Closed += (sender, args) =>
            {
                _activeWindows.Remove(window);
            };
            _activeWindows.Add(window);
        }

        static public Window GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in _activeWindows)
                {
                    if (window.Content != null && element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        static public UIElement FindElementByName(UIElement element, string name)
        {
            if (element.XamlRoot != null && element.XamlRoot.Content != null)
            {
                var ele = (element.XamlRoot.Content as FrameworkElement).FindName(name);
                if (ele != null)
                {
                    return ele as UIElement;
                }
            }
            return null;
        }

        static public List<Window> ActiveWindows { get { return _activeWindows; } }

        static private List<Window> _activeWindows = new List<Window>();
    }
}
