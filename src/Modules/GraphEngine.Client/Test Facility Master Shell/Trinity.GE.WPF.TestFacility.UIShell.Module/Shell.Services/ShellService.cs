﻿/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UIShell.Module                     *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;
using System.Windows;
using Castle.Windsor;
using IKW.Contropolus.VKMC.UIShell.Module.Shell;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.UI.RegionNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using UI.CastleWindsorAdapter.ServiceLocator.Extensions;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module.Shell.Services
{
    public class ShellService : IUIShellService
    {
        private static IWindsorContainer GlobalContainer { get; set; }
        private static IRegionManager ShellInstanceGlobalRegionManager { get; set; }
        private static IRegionManager ScopedRegionInActiveShell { get; set; }

        public ShellService(IWindsorContainer theGlobalContainer, 
                            IRegionManager    theShellInstanceGlobalRegionManager)
        {
            GlobalContainer                  = theGlobalContainer;
            ShellInstanceGlobalRegionManager = theShellInstanceGlobalRegionManager;
        }

        public Window ShellWindow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>

        public void ShowShell(string uri)
        {
            var s = CastleWindsorContainerExtensions.Resolve(GlobalContainer, typeof(MainShellWindow));

            var shell = GlobalContainer.Resolve<MainShellWindow>();

            var scopedRegion = ShellInstanceGlobalRegionManager.CreateRegionManager();
            RegionManager.SetRegionManager(shell, scopedRegion);

            RegionManagerAware.SetRegionManagerAware(shell, scopedRegion);

            scopedRegion.RequestNavigate(UIRegionNames.MainMenuControlRegion, uri);

            shell.Show();
        }


        /// <summary>
        /// 
        /// </summary>

        private Func<Type, Type, string, IRegionManager> DefaultViewViewModelViewNameFactory = (Func<Type, Type, string, IRegionManager>) ((theUIView, theUIViewModel, theUIViewName) =>
            {
                var shell = GlobalContainer.Resolve<MainShellWindow>();

                var scopedRegion = ShellInstanceGlobalRegionManager.CreateRegionManager();
                RegionManager.SetRegionManager(shell, scopedRegion);

                RegionManagerAware.SetRegionManagerAware(shell, scopedRegion);

                // Here is the Dynamic View Inject Code against the new Scoped Region Instance!

                var uiViewInstance = GlobalContainer.Resolve(theUIView);

                // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

                var uiViewModel = GlobalContainer.Resolve(theUIViewModel, theUIViewName);

                // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel
                // Use a bit of Reflection to set the "ActiveView" property on the ViewModel

                var viewModelActiveViewProperty = uiViewModel.GetType().GetProperty("ActiveView");
                viewModelActiveViewProperty?.SetValue(uiViewModel, uiViewInstance);

                // Call the "ApplyViewModel" method on the View to set the DataContext

                var uiViewApplyViewModelMethod = uiViewInstance.GetType().GetMethod("ApplyViewModel");
                uiViewApplyViewModelMethod.Invoke(uiViewInstance, new[] { uiViewModel });

                // Do the UIViewInstance Injection!

                PrismRegionServices.InjectUIViewIntoRegion(
                    scopedRegion,
                    UIRegionNames.MainMenuControlRegion,
                    theUIViewName,
                    uiViewInstance);

                scopedRegion.RequestNavigate(UIRegionNames.MainMenuControlRegion, theUIViewName);

                ScopedRegionInActiveShell = scopedRegion;

                shell.Show();

                return scopedRegion;
            });

        private IUIShellService _iuiShellServiceImplementation;

        public void ShowShell<TViewModel, TView>(Func<Type, Type, string, object> viewViewModelViewNameFactory)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <param name="theUIViewModel"></param>
        /// <param name="theUIView"></param>
        /// <param name="theUIViewName"></param>

        public IRegionManager ShowShell<TViewModel, TView>(Type theUIViewModel, Type theUIView, string theUIViewName)
        {
            //DefaultViewViewModelViewNameFactory(theUIViewModel, theUIViewModel, theUIViewName);

            //var newShell = CastleWindsorContainerExtensions.Resolve(GlobalContainer, typeof(MainShellWindow));

            var shell = GlobalContainer.Resolve<MainShellWindow>();

            var scopedRegion = ShellInstanceGlobalRegionManager.CreateRegionManager();
            RegionManager.SetRegionManager(shell, scopedRegion);

            RegionManagerAware.SetRegionManagerAware(shell, scopedRegion);

            // Here is the Dynamic View Inject Code against the new Scoped Region Instance!

            var uiViewInstance = GlobalContainer.Resolve(theUIView);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            var uiViewModel = GlobalContainer.Resolve(theUIViewModel, theUIViewName);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel
            // Use a bit of Reflection to set the "ActiveView" property on the ViewModel

            var viewModelActiveViewProperty = uiViewModel.GetType().GetProperty("ActiveView");
            viewModelActiveViewProperty?.SetValue(uiViewModel, uiViewInstance);

            // Call the "ApplyViewModel" method on the View to set the DataContext

            var uiViewApplyViewModelMethod = uiViewInstance.GetType().GetMethod("ApplyViewModel");
            uiViewApplyViewModelMethod?.Invoke(uiViewInstance, new[] {uiViewModel});

            //// Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion(
                scopedRegion,
                UIRegionNames.MainMenuControlRegion,
                theUIViewName,
                uiViewInstance);

            scopedRegion.RequestNavigate(UIRegionNames.MainMenuControlRegion, theUIViewName);

            ScopedRegionInActiveShell = scopedRegion;

            shell.Show();

            return ScopedRegionInActiveShell;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <param name="theUIViewModel"></param>
        /// <param name="theUIView"></param>
        /// <param name="theUIViewName"></param>

        public void SetupandLoadViewViewModelInRegion<TViewModel, TView>(Type theUIViewModel, 
                                                                         Type theUIView, 
                                                                         string theUIViewName)
        {
            var shell = GlobalContainer.Resolve<MainShellWindow>();

            var scopedRegion = ShellInstanceGlobalRegionManager.CreateRegionManager();
            RegionManager.SetRegionManager(shell, scopedRegion);

            RegionManagerAware.SetRegionManagerAware(shell, scopedRegion);

            // Here is the Dynamic View Inject Code against the new Scoped Region Instance!

            var uiViewInstance = GlobalContainer.Resolve(theUIView);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            var uiViewModel = GlobalContainer.Resolve(theUIViewModel, theUIViewName);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel
            // Use a bit of Reflection to set the "ActiveView" property on the ViewModel

            var viewModelActiveViewProperty = uiViewModel.GetType().GetProperty("ActiveView");
            viewModelActiveViewProperty?.SetValue(uiViewModel, uiViewInstance);

            // Call the "ApplyViewModel" method on the View to set the DataContext

            var uiViewApplyViewModelMethod = uiViewInstance.GetType().GetMethod("ApplyViewModel");
            uiViewApplyViewModelMethod?.Invoke(uiViewInstance, new[] { uiViewModel });

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion(
                scopedRegion,
                UIRegionNames.MainMenuControlRegion,
                theUIViewName,
                uiViewInstance);

            scopedRegion.RequestNavigate(UIRegionNames.MainMenuControlRegion, theUIViewName);

            ScopedRegionInActiveShell = scopedRegion;

            shell.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theRegionName"></param>
        /// <param name="theUIView"></param>
        /// <param name="theUIViewName"></param>

        public void UnloadRegion(string theRegionName, Type theUIView, string theUIViewName)
        {
            var uiViewInstance = GlobalContainer.Resolve(theUIView);

            // Do the UIViewInstance Injection!

            PrismRegionServices.RemoveUIViewFromRegion(
                ScopedRegionInActiveShell,
                theRegionName,
                theUIViewName,
                uiViewInstance);
        }

        public bool TeardownRegionSetup()
        {
            throw new NotImplementedException();
        }

        public async Task<(int ReturnCode, int ReasonCode)> RegisterShellAsync<TShellType>(
            ContropolusShellType theShellRegistrationType, TShellType theShellViewType,
            DependencyObject theShellInstance)
        {
            return await _iuiShellServiceImplementation.RegisterShellAsync(theShellRegistrationType, theShellViewType,
                theShellInstance);
        }

        public async Task<(int ReturnCode, int ReasonCode)> RegisterShellAsync<TShellType>(
            ContropolusShellType theShellRegistrationType, Type theShellViewType,
            DependencyObject theShellInstance)
        {
            return await _iuiShellServiceImplementation.RegisterShellAsync<TShellType>(theShellRegistrationType,
                theShellViewType, theShellInstance);
        }

        public async Task<(DependencyObject ShellWindowInstance, Type ShellType)> RegisterShellInstanceAsync(
            ContropolusShellType theShellRegistrationType, DependencyObject theShellInstance,
            Type theShellInstanceType)
        {
            return await _iuiShellServiceImplementation.RegisterShellInstanceAsync(theShellRegistrationType,
                theShellInstance, theShellInstanceType);
        }

        public async Task<(DependencyObject ShellWindowInstance, Type ShellType)>
            RegisterShellInstanceAsync<TShellViewType, TShellViewModelType>(
                ContropolusShellType theShellRegistrationType,
                DependencyObject theShellInstance, Type theShellInstanceType)
        {
            return await _iuiShellServiceImplementation.RegisterShellInstanceAsync<TShellViewType, TShellViewModelType>(
                theShellRegistrationType, theShellInstance, theShellInstanceType);
        }

        /// <summary>
        /// 
        /// </summary>

        public IRegionCollection Regions { get; }
        public IRegionManager ScopedRegionManager { get; set; }
        public IRegionManager CreateRegionManager()
        {
            throw new NotImplementedException();
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri source)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string source)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback,
            NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            throw new NotImplementedException();
        }

        Task<(int ReturnCode, int ReasonCode)> IUIShellService.RegisterShellAsync<TShellType>(
            ContropolusShellType theShellRegistrationType, TShellType theShellViewType,
            DependencyObject theShellInstance)
        {
            throw new NotImplementedException();
        }

        Task<(int ReturnCode, int ReasonCode)> IUIShellService.RegisterShellAsync<TShellType>(
            ContropolusShellType theShellRegistrationType, Type theShellViewType, DependencyObject theShellInstance)
        {
            throw new NotImplementedException();
        }

        Task<(DependencyObject ShellWindowInstance, Type ShellType)> IUIShellService.RegisterShellInstanceAsync(
            ContropolusShellType theShellRegistrationType, DependencyObject theShellInstance, Type theShellInstanceType)
        {
            throw new NotImplementedException();
        }

        Task<(DependencyObject ShellWindowInstance, Type ShellType)> IUIShellService.
            RegisterShellInstanceAsync<TShellViewType, TShellViewModelType>(
                ContropolusShellType theShellRegistrationType, DependencyObject theShellInstance,
                Type theShellInstanceType)
        {
            throw new NotImplementedException();
        }
    }
}
