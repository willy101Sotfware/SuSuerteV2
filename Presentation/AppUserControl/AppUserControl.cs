using SuSuerteV2.Domain.UIServices;
using System.Windows.Controls;

namespace SuSuerteV2.UserControls
{
    public class AppUserControl : UserControl
    {
        protected readonly Navigator _nav = Navigator.Instance;

        protected void NavigateTo(UserControl view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            _nav.NavigateTo(view);
        }

        protected void EnableView()
        {
            this.Dispatcher.Invoke(() =>
            {
                IsEnabled = true;
                Opacity = 1;
            });
        }

        protected void DisableView()
        {
            this.Dispatcher.Invoke(() =>
            {
                IsEnabled = false;
                Opacity = 0.3;
            });
        }
    }
}