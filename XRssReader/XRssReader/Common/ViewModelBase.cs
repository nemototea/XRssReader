using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using XRssReader.Model;

namespace XRssReader.Common
{
    /// <summary>
    /// ViewModel基本クラス
    /// INotifyPropertyChangedの実装を提供する
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 任意のタスクの成功・失敗のフラグを扱うイベント用デリゲートです。
        /// </summary>
        public delegate void TaskResultEventHandler(object sender, TaskResultEventArgs e);

        /// <summary>
        /// プロパティ変更時に発行されるイベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChangedイベントを発行する
        /// </summary>
        /// <param name="propertyName">変更プロパティ名</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 任意のタスクの成功・失敗のフラグを扱うイベント引数です。
    public class TaskResultEventArgs : EventArgs
    {
        public Boolean TaskResult { get; set; }
        public TaskResultEventArgs(Boolean i_result)
        {
            TaskResult = i_result;
        }
    }
}
