using System.Windows;
using System.Windows.Controls;

namespace MaJiCSoft.ChatOverlay
{
    public enum TabItemType
    {
        Status,
        Chat
    }

    public sealed class TabItem
    {
        public TabItemType TabType { get; set; }
        public string ItemName { get; set; }
        public string ChannelName { get; set; }
    }


    public class TabItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusView { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            var data = (TabItem)item;
            var win = Application.Current.MainWindow;
            DataTemplate template;
            switch (data.TabType)
            {
                case TabItemType.Status:
                    template = StatusView; // win.FindResource("StatusView") as DataTemplate;
                    break;
                default:
                    template = base.SelectTemplate(item, container);
                    break;
            }
            return template;
        }
    }
}
