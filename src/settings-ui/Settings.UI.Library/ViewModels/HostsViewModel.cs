﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.PowerToys.Settings.UI.Library;
using Microsoft.PowerToys.Settings.UI.Library.Helpers;
using Microsoft.PowerToys.Settings.UI.Library.Interfaces;
using Microsoft.PowerToys.Settings.UI.Library.ViewModels.Commands;
using Settings.UI.Library.Enumerations;

namespace Settings.UI.Library.ViewModels
{
    public class HostsViewModel : Observable
    {
        private bool _isElevated;

        private ISettingsUtils SettingsUtils { get; set; }

        private GeneralSettings GeneralSettingsConfig { get; set; }

        private HostsSettings Settings { get; set; }

        private Func<string, int> SendConfigMSG { get; }

        public ButtonClickCommand LaunchEventHandler => new ButtonClickCommand(Launch);

        public bool IsEnabled
        {
            get => GeneralSettingsConfig.Enabled.Hosts;

            set
            {
                if (value != GeneralSettingsConfig.Enabled.Hosts)
                {
                    // Set the status in the general settings configuration
                    GeneralSettingsConfig.Enabled.Hosts = value;
                    OutGoingGeneralSettings snd = new OutGoingGeneralSettings(GeneralSettingsConfig);

                    SendConfigMSG(snd.ToString());
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public bool LaunchAdministratorEnabled => IsEnabled && !_isElevated;

        public bool ShowStartupWarning
        {
            get => Settings.Properties.ShowStartupWarning;
            set
            {
                if (value != Settings.Properties.ShowStartupWarning)
                {
                    Settings.Properties.ShowStartupWarning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool LaunchAdministrator
        {
            get => Settings.Properties.LaunchAdministrator;
            set
            {
                if (value != Settings.Properties.LaunchAdministrator)
                {
                    Settings.Properties.LaunchAdministrator = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int AdditionalLinesPosition
        {
            get => (int)Settings.Properties.AdditionalLinesPosition;
            set
            {
                if (value != (int)Settings.Properties.AdditionalLinesPosition)
                {
                    Settings.Properties.AdditionalLinesPosition = (AdditionalLinesPosition)value;
                    NotifyPropertyChanged();
                }
            }
        }

        public HostsViewModel(ISettingsUtils settingsUtils, ISettingsRepository<GeneralSettings> settingsRepository, ISettingsRepository<HostsSettings> moduleSettingsRepository, Func<string, int> ipcMSGCallBackFunc, bool isElevated)
        {
            SettingsUtils = settingsUtils;
            GeneralSettingsConfig = settingsRepository.SettingsConfig;
            Settings = moduleSettingsRepository.SettingsConfig;
            SendConfigMSG = ipcMSGCallBackFunc;
            _isElevated = isElevated;
        }

        public void Launch()
        {
            var actionName = "Launch";

            if (!_isElevated && LaunchAdministrator)
            {
                actionName = "LaunchAdministrator";
            }

            SendConfigMSG("{\"action\":{\"Hosts\":{\"action_name\":\"" + actionName + "\", \"value\":\"\"}}}");
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
            SettingsUtils.SaveSettings(Settings.ToJsonString(), HostsSettings.ModuleName);
        }
    }
}
