using System;
using Microsoft.SPOT;
using SmartSecSystem.Views;

namespace SmartSecSystem
{
    public static class ViewManager
    {
        private static StartupView _startupView;
        public static StartupView StartupView
        {
            get
            {
                if (_startupView == null)
                    _startupView = new StartupView();
                return _startupView;
            }
        }

        private static IdleTimeView _idleTimeView;
        public static IdleTimeView IdleTimeView
        {
            get
            {
                if (_idleTimeView == null)
                    _idleTimeView = new IdleTimeView();
                else
                    _idleTimeView.UpdateTime();
                return _idleTimeView;
            }
        }

        private static MainView _mainView;
        public static MainView MainView
        {
            get
            {
                if (_mainView == null)
                    _mainView = new MainView();
                else
                    _mainView.UpdateTime();
                return _mainView;
            }
        }
    }
}
