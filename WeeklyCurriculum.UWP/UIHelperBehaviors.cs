using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace WeeklyCurriculum.UWP
{
    public class FlyoutBehaviors : Behavior<Flyout>
    {
        public bool IsFlyoutOpen
        {
            get { return (bool)GetValue(IsFlyoutOpenProperty); }
            set { SetValue(IsFlyoutOpenProperty, value); }
        }

        public static readonly DependencyProperty IsFlyoutOpenProperty =
            DependencyProperty.Register("IsFlyoutOpen", typeof(bool), typeof(FlyoutBehaviors), new PropertyMetadata(false, OnIsFlyoutOpenChanged));

        private static void OnIsFlyoutOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beh = (FlyoutBehaviors)d;
            if (e.NewValue is true)
            {
                beh.AssociatedObject.ShowAt(beh.AssociatedObject.Target);
            }
            else
            {
                beh.AssociatedObject.Hide();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Opened += this.AssociatedObject_Opened;
            this.AssociatedObject.Closed += this.AssociatedObject_Closed;
        }

        private void AssociatedObject_Closed(object sender, object e)
        {
            this.IsFlyoutOpen = false;
        }

        private void AssociatedObject_Opened(object sender, object e)
        {
            this.IsFlyoutOpen = true;
        }
    }
}
