﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WinIRC.Ui
{
    public sealed partial class ChannelListItem : UserControl
    {
        internal static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ChannelListItem), new PropertyMetadata(null));

        internal static readonly DependencyProperty IsServerProperty =
            DependencyProperty.Register("IsServer", typeof(bool), typeof(ChannelListItem), new PropertyMetadata(null));

        internal static readonly DependencyProperty ServerProperty =
            DependencyProperty.Register("Server", typeof(string), typeof(ChannelListItem), new PropertyMetadata(null));

        internal static readonly DependencyProperty HideChromeProperty =
            DependencyProperty.Register("HideChrome", typeof(bool), typeof(ChannelListItem), new PropertyMetadata(null));


        public event EventHandler ChannelCloseClicked;
        public event EventHandler ChannelJoinClicked;

        public event EventHandler ServerRightClickEvent;
        public event EventHandler ServerClickEvent;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public bool IsServer
        {
            get { return (bool)GetValue(IsServerProperty); }
            set { SetValue(IsServerProperty, value); }
        }

        public string Server
        {
            get { return (string)GetValue(ServerProperty); }
            set { SetValue(ServerProperty, value); }
        }

        public bool HideChrome
        {
            get { return (bool)GetValue(HideChromeProperty); }
            set { SetValue(HideChromeProperty, value); }
        }

        public ChannelListItem()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            Loaded += (sender, args) =>
            {
                if (IsServer)
                {
                    CloseButton.Visibility = Visibility.Collapsed;
                    AddButton.Visibility = Visibility.Visible;
                    MenuButton.Visibility = Visibility.Visible;
                }
                else if (HideChrome)
                {
                    CloseButton.Visibility = Visibility.Collapsed;
                }
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsServer)
                ChannelCloseClicked?.Invoke(sender, new ChannelEventArgs(Title, Server));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ChannelJoinClicked?.Invoke(sender, new ChannelEventArgs(channel.Text, Server));
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowRightClickMenu(sender, e);
        }

        private void ShowRightClickMenu(object sender, RoutedEventArgs e)
        {
            string menu = IsServer ? "ServerContextMenu" : "ChannelContextMenu";

            var RightClick = this.Resources[menu] as MenuFlyout;

            if (e is RightTappedRoutedEventArgs)
                RightClick.ShowAt(null, (e as RightTappedRoutedEventArgs).GetPosition(null));
            else
                RightClick.ShowAt(sender as FrameworkElement);

            Style s = new Windows.UI.Xaml.Style { TargetType = typeof(MenuFlyoutPresenter) };
            s.Setters.Add(new Setter(RequestedThemeProperty, Config.GetBoolean(Config.DarkTheme) ? ElementTheme.Dark : ElementTheme.Light));
            RightClick.MenuFlyoutPresenterStyle = s;

            if (!IsServer)
            {
                var key = Config.PerChannelSetting(Server, Title, Config.AlwaysNotify);
                AlwaysPing.IsChecked = Config.GetBoolean(key, false);
            }

        }

        private void CloseItem_Click(object sender, RoutedEventArgs e)
        {
            ServerRightClickEvent?.Invoke(sender, new ServerRightClickArgs(Title, ServerRightClickType.CLOSE));
        }

        private void ReconnectItem_Click(object sender, RoutedEventArgs e)
        {
            ServerRightClickEvent?.Invoke(sender, new ServerRightClickArgs(Title, ServerRightClickType.RECONNECT));
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRightClickMenu(sender, e);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsServer)
            {
                ServerClickEvent?.Invoke(this, new EventArgs());
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (IsServer)
            {
                this.CapturePointer(e.Pointer);
                VisualStateManager.GoToState(this, "PointerDown", true);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (IsServer)
            {
                VisualStateManager.GoToState(this, "PointerUp", true);
                this.ReleasePointerCapture(e.Pointer);
            }
        }

        private void AlwaysPing_Click(object sender, RoutedEventArgs e)
        {
            if (!IsServer)
            {
                var key = Config.PerChannelSetting(Server, Title, Config.AlwaysNotify);
                Config.SetBoolean(key, AlwaysPing.IsChecked);
            }
        }
    }

    public enum ServerRightClickType
    {
        CLOSE, RECONNECT
    }

    public class ServerRightClickArgs : EventArgs
    {
        public string server { get; private set; }
        public ServerRightClickType type { get; private set; }

        public ServerRightClickArgs(string title, ServerRightClickType type)
        {
            this.type = type;
            this.server = title;
        }
    }

    public class ChannelEventArgs : EventArgs
    {
        public ChannelEventArgs(string channel, string server)
        {
            this.Channel = channel;
            this.Server = server;
        }

        public string Channel { get; private set; }
        public string Server { get; private set; }
    }
}
