using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace XRssReader.Common
{
    #region コマンド引数あり
    /// <summary>
    /// デリゲートを受け取るICommandの実装
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> execute;

        private Func<T, bool> canExecute;

        private static readonly bool IS_VALUE_TYPE;

        static DelegateCommand()
        {
            IS_VALUE_TYPE = typeof(T).IsValueType;
        }

        /// <summary>
        /// コマンドのExecuteメソッドで実行する処理を指定してDelegateCommandのインスタンスを
        /// 作成します。
        /// </summary>
        /// <param name="i_execute">Executeメソッドで実行する処理</param>
        public DelegateCommand(Action<T> i_execute) : this(i_execute, o => true)
        {
            // : this(i_execute, o => true)
            // CanExeの指定を持たないものはtrue固定で渡す
        }

        /// <summary>
        /// コマンドのExecuteメソッドで実行する処理とCanExecuteメソッドで実行する処理を指定して
        /// DelegateCommandのインスタンスを作成します。
        /// </summary>
        /// <param name="i_execute">Executeメソッドで実行する処理</param>
        /// <param name="i_canExecute">CanExecuteメソッドで実行する処理</param>
        public DelegateCommand(Action<T> i_execute, Func<T, bool> i_canExecute)
        {
            if (i_execute == null)
            {
                throw new ArgumentNullException("DelegateCommand i_execute is null.");
            }

            if (i_canExecute == null)
            {
                throw new ArgumentNullException("DelegateCommand i_canExecute is null.");
            }

            this.execute = i_execute;
            this.canExecute = i_canExecute;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute(object parameter)
        {
            this.execute((T)parameter);
        }

        /// <summary>
        /// コマンドが実行可能な状態化どうか問い合わせます。
        /// </summary>
        /// <returns>実行可能な場合はtrue</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute((T)parameter);
        }

        /// <summary>
        /// ICommand.CanExecuteの明示的な実装。CanExecuteメソッドに処理を委譲する。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(Cast(parameter));
        }

        /// <summary>
        /// CanExecuteの結果に変更があったことを通知するイベント。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// ICommand.Executeの明示的な実装。Executeメソッドに処理を委譲する。
        /// </summary>
        /// <param name="parameter"></param>
        void ICommand.Execute(object parameter)
        {
            this.Execute(Cast(parameter));
        }

        /// <summary>
        /// convert parameter value
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private T Cast(object parameter)
        {
            if (parameter == null && IS_VALUE_TYPE)
            {
                return default(T);
            }
            return (T)parameter;
        }
    }
    #endregion

    #region コマンド引数なし
    /// <summary>
    /// デリゲートを受け取るICommandの実装
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Action execute;

        private Func<bool> canExecute;

        /// <summary>
        /// コマンドのExecuteメソッドで実行する処理を指定してDelegateCommandのインスタンスを
        /// 作成します。
        /// </summary>
        /// <param name="i_execute">Executeメソッドで実行する処理</param>
        public DelegateCommand(Action i_execute) : this(i_execute, () => true)
        {
        }

        /// <summary>
        /// コマンドのExecuteメソッドで実行する処理とCanExecuteメソッドで実行する処理を指定して
        /// DelegateCommandのインスタンスを作成します。
        /// </summary>
        /// <param name="i_execute">Executeメソッドで実行する処理</param>
        /// <param name="i_canExecute">CanExecuteメソッドで実行する処理</param>
        public DelegateCommand(Action i_execute, Func<bool> i_canExecute)
        {
            if (i_execute == null)
            {
                throw new ArgumentNullException("DelegateCommand i_execute is null.");
            }

            if (i_canExecute == null)
            {
                throw new ArgumentNullException("DelegateCommand i_canExecute is null.");
            }

            this.execute = i_execute;
            this.canExecute = i_canExecute;
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute()
        {
            this.execute();
        }

        /// <summary>
        /// コマンドが実行可能な状態化どうか問い合わせます。
        /// </summary>
        /// <returns>実行可能な場合はtrue</returns>
        public bool CanExecute()
        {
            return this.canExecute();
        }

        /// <summary>
        /// ICommand.CanExecuteの明示的な実装。CanExecuteメソッドに処理を委譲する。
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        /// <summary>
        /// CanExecuteの結果に変更があったことを通知するイベント。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// ICommand.Executeの明示的な実装。Executeメソッドに処理を委譲する。
        /// </summary>
        /// <param name="parameter"></param>
        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }
    }
    #endregion
}
