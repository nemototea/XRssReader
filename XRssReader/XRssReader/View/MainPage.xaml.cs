using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;

using XRssReader.ViewModel;
using XRssReader.Common;

namespace XRssReader.View
{
    /// <summary>
    /// メイン画面のコードビハインド
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // データバインディング用にVMをBindingContextに設定
            // ※Xamarin版DataContext
            this.BindingContext = new MainPageViewModel();
        }

        /// <summary>
        /// RSSフィード取得完了通知イベント
        /// </summary>
        /// <remarks>1:VMインスタンス、2:フィード取得処理結果(Boolean)</remarks>
        private void getRSSCompleted(object sender, TaskResultEventArgs e)
        {
            // 取得失敗ならメッセージを出す。
            // ※UI側の操作のためViewで処理する
            if (e.TaskResult == false)
            {
                var result = this.DisplayAlert("更新失敗", "RSSフィード取得中にエラーが発生しました。", "うい");
            }
        }
    }
}
