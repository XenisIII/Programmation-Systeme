using System.ComponentModel;
using EasySaveWPF.Models;
using EasySaveWPF.Services;
using System;
using System.IO;

namespace EasySaveWPF.ViewModels
{
    /// <summary>
    /// ViewModel responsible for handling real-time logging and state changes of backup operations.
    /// </summary>
    public class LogStatsRTViewModel
    {
        private WriteStatsRT _WriteStatsRT;
        public WriteStatsRT WriteStatsRT
        {
            get => _WriteStatsRT;
            set => _WriteStatsRT = value;
        }
        private string _type = "xml";
        public string Type
        {
            get => _type;
            set => _type = value;
        }
        // Directory path where logs are stored.
        private static readonly string LogDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PS-Logs");

        // Current backup model being monitored.
        private StatsRTModel _currentStatsRTModel;

        public LogStatsRTViewModel()
        {
            _WriteStatsRT = new WriteStatsRT();

        }

        /// <summary>
        /// Sets a new backup model for monitoring and logs state changes.
        /// </summary>
        /// <param name="statsRTModel">The backup model to monitor.</param>
        public void NewWork(StatsRTModel statsRTModel)
        {
            // Detach event handler from the current model if it exists.
            if (this._currentStatsRTModel is not null)
                this._currentStatsRTModel.PropertyChanged -= this.OnStatsRTModelPropertyChanged;

            // Attach the new backup model and set up property change notification.
            this._currentStatsRTModel = statsRTModel ?? throw new ArgumentNullException(nameof(statsRTModel));
            this._currentStatsRTModel.PropertyChanged += this.OnStatsRTModelPropertyChanged;
        }

        /// <summary>
        /// Responds to property changes on the current backup model.
        /// </summary>
        private void OnStatsRTModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle state changes specifically.
            if (e.PropertyName == nameof(StatsRTModel.State))
                this.HandleStateChange(this._currentStatsRTModel);
        }

        /// <summary>
        /// Handles state changes by writing real-time stats.
        /// </summary>
        /// <param name="state">The backup model with updated state.</param>
        private void HandleStateChange(StatsRTModel state) =>
            _WriteStatsRT.WriteRealTimeStatsAsync(state, LogDirPath, _type);

        /// <summary>
        /// Writes log information synchronously.
        /// </summary>
        /// <param name="log">The log information to write.</param>
        public void WriteLog(LogVarModel log) =>
            _WriteStatsRT.WriteLogsSync(log, LogDirPath, _type);
    }
}