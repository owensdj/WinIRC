﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinIRC.Ui;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinIRC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BehaviourSettingsView : BaseSettingsPage
    {
        private bool SettingsLoaded;

        List<String> UserListClickSettings = new List<string> { "Mention user in channel", "PM the user", "Show the context menu" };


        public BehaviourSettingsView()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            UserListClick.ItemsSource = UserListClickSettings;

            if (Config.Contains(Config.UserListClick))
            {
                this.UserListClick.SelectedIndex = Config.GetInt(Config.UserListClick);
            }
            else
            {
                Config.SetInt(Config.UserListClick, 0);
                this.UserListClick.SelectedIndex = 0;
            }

            if (Config.Contains(Config.SwitchOnJoin))
            {
                this.AutoChannelSwitch.IsOn = Config.GetBoolean(Config.SwitchOnJoin);
            } 
            else
            {
                Config.SetBoolean(Config.SwitchOnJoin, false);
                this.AutoChannelSwitch.IsOn = false;
            }

            this.SettingsLoaded = true;
        }

        private void UserListClick_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SettingsLoaded)
                return;

            Config.SetInt(Config.UserListClick, UserListClick.SelectedIndex);
        }

        private void AutoChannelSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!SettingsLoaded)
                return;

            Config.SetBoolean(Config.SwitchOnJoin, AutoChannelSwitch.IsOn);

        }
    }


}
