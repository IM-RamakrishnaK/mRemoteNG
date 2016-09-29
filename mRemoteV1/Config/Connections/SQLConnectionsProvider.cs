﻿using mRemoteNG.App;
using mRemoteNG.Messages;
using System;

namespace mRemoteNG.Config.Connections
{
    public class SqlConnectionsProvider : IDisposable
    {
        SqlUpdateTimer _updateTimer;
        SqlConnectionsUpdateChecker _sqlUpdateChecker;


        public SqlConnectionsProvider()
        {
            _updateTimer = new SqlUpdateTimer();
            _sqlUpdateChecker = new SqlConnectionsUpdateChecker();
            SqlUpdateTimer.SqlUpdateTimerElapsed += SqlUpdateTimer_SqlUpdateTimerElapsed;
            SqlConnectionsUpdateChecker.SqlUpdateCheckFinished += SQLUpdateCheckFinished;
        }

        ~SqlConnectionsProvider()
        {
            Dispose(false);
        }

        public void Enable()
        {
            _updateTimer.Enable();
        }

        public void Disable()
        {
            _updateTimer.Disable();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool itIsSafeToAlsoFreeManagedObjects)
        {
            if (itIsSafeToAlsoFreeManagedObjects)
            {
                DestroySQLUpdateHandlers();
                _updateTimer.Dispose();
                _sqlUpdateChecker.Dispose();
            }
        }

        private void DestroySQLUpdateHandlers()
        {
            SqlUpdateTimer.SqlUpdateTimerElapsed -= SqlUpdateTimer_SqlUpdateTimerElapsed;
            SqlConnectionsUpdateChecker.SqlUpdateCheckFinished -= SQLUpdateCheckFinished;
        }

        private void SqlUpdateTimer_SqlUpdateTimerElapsed()
        {
            _sqlUpdateChecker.IsDatabaseUpdateAvailableAsync();
        }

        private void SQLUpdateCheckFinished(bool UpdateIsAvailable)
        {
            if (UpdateIsAvailable)
            {
                Runtime.MessageCollector.AddMessage(MessageClass.InformationMsg, Language.strSqlUpdateCheckUpdateAvailable, true);
                Runtime.LoadConnectionsBG();
            }
        }
    }
}