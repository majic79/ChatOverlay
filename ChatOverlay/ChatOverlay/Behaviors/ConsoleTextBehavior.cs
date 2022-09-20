using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace MaJiCSoft.ChatOverlay
{

    public class ConsoleTextBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb != null)
            {
                bool scroll = true; //= tb.CaretIndex == tb.Text.Length;
                if (scroll)
                {
                    tb.CaretIndex = tb.Text.Length;
                    tb.ScrollToEnd();
                }
            }
        }
    }
}
