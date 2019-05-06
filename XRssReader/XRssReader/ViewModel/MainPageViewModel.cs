using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;

using XRssReader.Common;
using XRssReader.Model;

namespace XRssReader.ViewModel
{
    /// <summary>
    /// RSSリーダーメイン画面ViewModel
    /// </summary>
    class MainPageViewModel : ViewModelBase
    {
        #region メンバ
        // RSSフィード取得のコマンド
        private DelegateCommand getRssCommand;

        // RSSの取得完了後に発生させるイベントハンドラ
        public event TaskResultEventHandler GetRSSCompleted;
        #endregion

        #region プロパティ
        private String m_feedTitle = String.Empty;
        /// <summary>
        /// RSSフィード配信元情報：タイトル
        /// </summary>
        public String FeedTitle
        {
            get { return m_feedTitle; }
            set
            {
                if (IsProgress)
                {
                    m_feedTitle = Define.MSG_GETTING;
                }
                else
                {
                    m_feedTitle = value;
                }
                OnPropertyChanged(nameof(FeedTitle));
            }
        }

        private String m_feedDescription = String.Empty;
        /// <summary>
        /// RSSフィード配信元情報：説明
        /// </summary>
        public String FeedDescription
        {
            get { return m_feedDescription; }
            set
            {
                if (IsProgress)
                {
                    m_feedDescription = Define.MSG_RECIVING;
                }
                else
                {
                    m_feedDescription = value;
                }
                OnPropertyChanged(nameof(FeedDescription));
            }
        }

        private DateTime m_feedLastUpdatedTime = DateTime.MinValue;
        /// <summary>
        /// RSSフィード配信元情報：最終更新日
        /// </summary>
        public DateTime FeedLastUpdatedTime
        {
            get { return m_feedLastUpdatedTime; }
            set
            {
                if (IsProgress)
                {
                    m_feedLastUpdatedTime = DateTime.MinValue;
                }
                else
                {
                    m_feedLastUpdatedTime = value;
                }
                OnPropertyChanged(nameof(FeedLastUpdatedTime));
            }
        }

        private ObservableCollection<RSSContent> m_feedItems = null;
        /// <summary>
        ///  RSSフィードのコンテンツ
        /// </summary>
        public ObservableCollection<RSSContent> FeedItems
        {
            get {
                if (this.m_feedItems == null)
                {
                    this.m_feedItems = new ObservableCollection<RSSContent>();
                }

                return this.m_feedItems;
            }
            private set
            {
                this.m_feedItems = value;
            }
        }

        private String m_feedUrl = "https://jp.techcrunch.com/feed/";
        /// <summary>
        /// RSSフィードのURL
        /// </summary>
        public String FeedUrl
        {
            get { return this.m_feedUrl; }
            set
            {
                if (this.m_feedUrl != value)
                {
                    this.m_feedUrl = value;
                    this.OnPropertyChanged(nameof(FeedUrl));
                }
            }
        }

        private Boolean m_isProgress = false;
        /// <summary>
        /// RSSフィード取得中フラグ
        /// </summary>
        public Boolean IsProgress
        {
            get { return this.m_isProgress; }
            set
            {
                this.m_isProgress = value;
                OnPropertyChanged(nameof(FeedTitle));
                OnPropertyChanged(nameof(FeedDescription));
                OnPropertyChanged(nameof(FeedLastUpdatedTime));
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainPageViewModel()
        {
        }
        #endregion

        #region RSSフィード取得Command
        /// <summary>
        /// RSSフィード取得を行うコマンド
        /// </summary>
        public DelegateCommand GetRssCommand
        {
            get
            {
                if (this.getRssCommand == null)
                {
                    this.getRssCommand = new DelegateCommand(GetRssExecute, CanGetRssExecute);
                }

                return this.getRssCommand;
            }
        }

        /// <summary>
        /// RSSフィード取得コマンドを実行する
        /// </summary>
        private void GetRssExecute()
        {
            // RSSフィード取得中のフラグをオンにします。
            IsProgress = true;

            if (FeedItems != null)
            {
                // 現在のフィード情報をクリアする
                FeedItems.Clear();
            }

            // RSSフィードを非同期で取得します。
            Task result = GetRSSAsync(FeedUrl);
        }

        /// <summary>
        /// RSSフィード取得コマンドが実行可能か判定する
        /// </summary>
        /// <remarks>
        /// フィード取得中は更新不可
        /// </remarks>
        private Boolean CanGetRssExecute() => !IsProgress;
        #endregion

        #region プライベートメソッド
        // RSSフィードを取得します。(非同期メソッド)
        private async Task GetRSSAsync(String i_feedUrl)
        {
            Boolean result = false;
            try
            {
                var client = new HttpClient();
                RssFeed latest;

                using (var reader = new StringReader(await client.GetStringAsync(i_feedUrl)))
                {
                    var desirializer = new XmlSerializer(typeof(RssFeed));
                    latest = desirializer.Deserialize(reader) as RssFeed;
                }

                IsProgress = false;

                if (latest == null)
                {
                    throw new Exception("取得したフィードがnull");
                }

                // RSSフィードの配信元情報を取得します。
                this.FeedTitle = latest.Channel.Title;
                this.FeedDescription = Regex.Replace(latest.Channel.Description, Define.PATTERN_STR, String.Empty, RegexOptions.Singleline);
                this.FeedLastUpdatedTime = latest.Channel.LastBuildDate;

                // RSSフィードのコンテンツを取得します。
                foreach (var item in latest.Channel.Items)
                {
                    // コンテンツ取得
                    RSSContent content = new RSSContent();
                    content.Title = item.Title;
                    content.Description = Regex.Replace(item.Description, Define.PATTERN_STR, String.Empty, RegexOptions.Singleline);
                    content.PubDate = item.PubDate;
                    content.Link = item.Link;

                    // サムネイル画像取得
                    var imgMatch = Regex.Match(item.Description, Define.PATTERN_IMGTAG, RegexOptions.Singleline);
                    if (imgMatch.Success)
                    {
                        content.Thumbnail = ImageSource.FromUri(new Uri(imgMatch.Groups["uri"].Value));
                    }

                    Device.BeginInvokeOnMainThread(() => FeedItems.Add(content));

                    //FeedItems.Add(new RSSContent
                    //{
                    //    Title = item.Title,
                    //    Description = Regex.Replace(item.Description, Define.PATTERN_STR, String.Empty, RegexOptions.Singleline),
                    //    PubDate = item.PubDate,
                    //    Link = item.Link
                    //});

                }

                // RSSフィードの取得が完了したことをView側に通知します。
                // ※通知の引数にRSSフィード取得の結果を渡す
                GetRSSCompleted?.Invoke(this, new TaskResultEventArgs(result));

            }
            catch(Exception ex)
            {
                // RSSフィードの取得中に例外が発生したら、失敗フラグを立てます。
                result = false;
            }
        }
        #endregion
    }

    #region データ
    /// <summary>
    /// RSSのコンテンツ
    /// </summary>
    class RSSContent
    {
        // サムネイル
        public ImageSource Thumbnail { get; set; } = null;
        // タイトル
        public String Title { get; set; } = String.Empty;
        // 内容
        public String Description { get; set; } = String.Empty;
        // 投稿日時
        public DateTime PubDate { get; set; } = DateTime.MinValue;
        // リンク
        public String Link { get; set; } = String.Empty;

        #region 記事閲覧Command

        // 記事閲覧のコマンド
        private DelegateCommand<String> launchLinkUriCommand;

        /// <summary>
        /// 記事閲覧を行うコマンド
        /// </summary>
        public DelegateCommand<String> LaunchLinkUriCommand
        {
            get
            {
                if (this.launchLinkUriCommand == null)
                {
                    this.launchLinkUriCommand = new DelegateCommand<String>(LaunchLinkUriExecute, CanLaunchLinkUriExecute);
                }

                return this.launchLinkUriCommand;
            }
        }

        /// <summary>
        /// 記事閲覧コマンドを実行する
        /// </summary>
        private void LaunchLinkUriExecute(String i_link)
        {
            // フィードの記事URLを開きます
            Device.OpenUri(new Uri(i_link));
        }

        /// <summary>
        /// 記事閲覧コマンドが実行可能か判定する
        /// </summary>
        private Boolean CanLaunchLinkUriExecute(String i_link) => true;
        #endregion
    }

    ///// <summary>
    ///// RSSフィードの配信元情報
    ///// </summary>
    //class RSSProviderInfo
    //{
    //    // タイトル
    //    public String Title { get; set; } = String.Empty;
    //    // 概要
    //    public String Description { get; set; } = String.Empty;
    //    // 最終更新日時
    //    public DateTime LastUpdatedTime { get; set; } = DateTime.MinValue;

    //    public static RSSProviderInfo m_rSSProviderInfo = new RSSProviderInfo();

    //    public static RSSProviderInfo Instance
    //    {
    //        get { return m_rSSProviderInfo; }
    //    }

    //    private RSSProviderInfo() { }
    //}
    #endregion
}
