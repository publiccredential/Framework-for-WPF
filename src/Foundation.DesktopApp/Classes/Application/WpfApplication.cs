//-----------------------------------------------------------------------
// <copyright file="WpfApplication.cs" company="Genesys Source">
//      Copyright (c) 2017 Genesys Source. All rights reserved.
//      Licensed to the Apache Software Foundation (ASF) under one or more 
//      contributor license agreements.  See the NOTICE file distributed with 
//      this work for additional information regarding copyright ownership.
//      The ASF licenses this file to You under the Apache License, Version 2.0 
//      (the 'License'); you may not use this file except in compliance with 
//      the License.  You may obtain a copy of the License at 
//       
//        http://www.apache.org/licenses/LICENSE-2.0 
//       
//       Unless required by applicable law or agreed to in writing, software  
//       distributed under the License is distributed on an 'AS IS' BASIS, 
//       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  
//       See the License for the specific language governing permissions and  
//       limitations under the License. 
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Genesys.Extensions;
using Genesys.Extras.Configuration;
using Genesys.Extras.Net;
using System.Windows;
using System.Linq;
using System.Windows.Navigation;
using Genesys.Foundation.Application;

namespace Foundation.Applications
{
    /// <summary>
    /// Global application information
    /// </summary>
    public abstract class WpfApplication : System.Windows.Application, IWpfApplication
    {
        ///// <summary>
        ///// Persistent ConfigurationManager class, automatically loaded with this project .config files
        ///// </summary>
        public IConfigurationManager ConfigurationManager { get; set; } = new ConfigurationManagerFull();

        /// <summary>
        /// MyWebService
        /// </summary>
        public Uri MyWebService { get { return new Uri(ConfigurationManager.AppSettingValue("MyWebService"), UriKind.RelativeOrAbsolute); } }

        /// <summary>
        /// Entry point Screen (Typically login screen)
        /// </summary>
        public abstract Uri LandingPage { get; }

        /// <summary>
        /// Home dashboard screen
        /// </summary>
        public abstract Uri HomePage { get; }

        /// <summary>
        /// Error screen
        /// </summary>
        public abstract Uri ErrorPage { get; }

        /// <summary>
        /// Returns currently active window
        /// </summary>
        public static new Window Current { get { return Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive); } }

        /// <summary>
        /// Error screen
        /// </summary>
        public Uri CurrentPage { get { return ((NavigationWindow)WpfApplication.Current).CurrentSource; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public WpfApplication() : base()
        {
            InitializeAsync();
            StartupUri = this.LandingPage;
        }

        /// <summary>
        /// Init all locally stored data
        /// </summary>
        public abstract Task InitializeAsync();

        /// <summary>
        /// Initializes this class with data, and allows for proper async behavior
        /// </summary>
        /// <returns></returns>
        public virtual async Task InitializeAsync(bool hasServices)
        {
            await this.LoadConfigAsync();
            if (hasServices == true) await this.WakeServicesAsync();
        }

        /// <summary>
        /// Loads config data
        /// </summary>
        /// <returns></returns>
        public async Task LoadConfigAsync()
        {
            await Task.Delay(1);
            ConfigurationManager = new ConfigurationManagerFull();
        }

        /// <summary>
        /// Wakes up any sleeping processes, and MyWebService chain
        /// </summary>
        /// <returns></returns>
        public virtual async Task WakeServicesAsync()
        {
            if (MyWebService.ToString() == TypeExtension.DefaultString)
            {
                HttpRequestGetString Request = new HttpRequestGetString(MyWebService.ToString());
                Request.ThrowExceptionWithEmptyReponse = false;
                await Request.SendAsync();
            }
        }

        /// <summary>
        /// Gets the root frame of the application
        /// </summary>
        /// <returns></returns>
        public Frame RootFrame
        {
            get
            {
                Frame returnValue = new Frame();

                if (Current.Content is Frame)
                {
                    returnValue = (Frame)Current.Content;
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Can this screen go back
        /// </summary>
        public bool CanGoBack { get { return RootFrame.CanGoBack; } }

        /// <summary>
        /// Can this screen go forward
        /// </summary>
        public bool CanGoForward { get { return RootFrame.CanGoForward; } }

        /// <summary>
        /// Navigates back to previous screen
        /// </summary>
        public void GoBack() { RootFrame.GoBack(); }

        /// <summary>
        /// Navigates forward to next screen
        /// </summary>
        public void GoForward() { RootFrame.GoForward(); }

        /// <summary>
        /// Navigates to a page via Uri.
        /// Typically in WPF apps
        /// </summary>
        /// <param name="destination">Destination page Uri</param>
        public bool Navigate(Uri destinationPageUrl) { return RootFrame.Navigate(destinationPageUrl); }

        /// <summary>
        /// Navigates to a page via Uri.
        /// Typically in WPF apps
        /// </summary>
        /// <param name="destination">Destination page Uri</param>
        /// <param name="dataToPass">Data to be passed to the destination page</param>
        public bool Navigate<TModel>(Uri destinationPageUrl, TModel dataToPass) { return RootFrame.Navigate(destinationPageUrl, dataToPass); }

    }
}
