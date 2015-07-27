﻿using System.Threading;
using Microsoft.Phone.Notification;
using Assisticant;

namespace Correspondence.BinaryHTTPClient.Notification
{
    public class ToastNotificationObserver
    {
        private IHTTPConfigurationProvider _configurationProvider;
        private HttpNotificationChannel _httpChannel;
        private Computed _depToastBound;

        public ToastNotificationObserver(IHTTPConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _depToastBound = new Computed(UpdateToastBound);

            _depToastBound.Invalidated += () =>
                ThreadPool.QueueUserWorkItem(o =>
                    _depToastBound.OnGet());
            _depToastBound.OnGet();
        }

        #region Observable properties
        // Generated by Update Controls --------------------------------
        private Observable _indHttpChannel = new Observable();

        public HttpNotificationChannel HttpChannel
        {
            get { _indHttpChannel.OnGet(); return _httpChannel; }
            set { _indHttpChannel.OnSet(); _httpChannel = value; }
        }
        // End generated code --------------------------------
        #endregion

        private void UpdateToastBound()
        {
            HttpNotificationChannel httpChannel = HttpChannel;

            if (httpChannel != null)
            {
                // TODO: Enable toast in a portable fashion.
                // bool isToastEnabled = _configurationProvider.IsToastEnabled;
                bool isToastEnabled = false;
                if (isToastEnabled)
                {
                    if (!httpChannel.IsShellToastBound)
                        httpChannel.BindToShellToast();
                }
                else
                {
                    if (httpChannel.IsShellToastBound)
                        httpChannel.UnbindToShellToast();
                }
            }
        }
    }
}