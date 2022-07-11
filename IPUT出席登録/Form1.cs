using System;
using System.Windows.Forms;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using AngleSharp;
using System.IO;
using Microsoft.Win32;
using System.Net;
using Newtonsoft.Json.Linq;
using AngleSharp.Js.Dom;
using System.Text;
using System.Text.Json;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Text.RegularExpressions;


namespace IPUT出席登録
{
    public partial class アカウント情報入力 : Form
    {
        public string ID;
        public string PASSWORD;
        public Form form3;
        public int retry = 1;
        public bool flag = false;
        public bool debug = false;
        public int retrylimit = 3;
        public string[] option;
        public string token;

        class ORData
        {
            public string mainurl { get; set; }
            public string attendmoduleurl { get; set; }
            public string Start_time { get; set; }
        }

        public static string ReadAllLine(string filePath, string encodingName)
        {
            if (!File.Exists(filePath)) return null;
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(encodingName));
            string allLine = sr.ReadToEnd();
            sr.Close();

            return allLine;
        }

        public void Dprint(string str)
        {
            if (debug) Console.WriteLine(str);
        }
        private void Dprint(bool v)
        {
            if (debug) Console.WriteLine(v);
        }

        public アカウント情報入力(string[] args)
        {
            option = args;
            if (option.Length != 0)
            {
                if (option[0] == "--debug") debug = true;
                if (option.Length >= 1) retrylimit = Int32.Parse(option[1]);
            }

            Dprint("ログインフォーム初期化開始");
            InitializeComponent();
            Dprint("レジストリ確認");
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\IPUT\\LMS\\AUTHDATA");
            if (key == null)
            {
                Dprint("レジストリキー存在なし");
                Dprint("ログインフォーム表示");
                Visible = true;
                return;
            }

            Dprint("レジストリから保存情報を取得");

            Dprint("レジストリからIDを取得");
            ID = (string)key.GetValue("ID");
            Dprint("レジストリからPASSWORDを取得");
            PASSWORD = (string)key.GetValue("PASSWORD");
            Dprint("ID:" + ID.ToString());
            Dprint("PASSWORD:" + PASSWORD.ToString());
            key.Close();
            Dprint("レジストリから取得したデータを確認(開発者デバッグ用)");
            Dprint("IDがnullかチェック:");
            Dprint(ID != null);
            Dprint("PASSWORDがnullかチェック:");
            Dprint(PASSWORD != null);
            Dprint("IDが空文字かチェック:");
            Dprint(ID != "");
            Dprint("PASSWORDが空文字かチェック:");
            Dprint(PASSWORD != "");
            Dprint("レジストリから取得したデータを確認(プログラム上での判定)");
            if (ID != null && PASSWORD != null && ID != "" && PASSWORD != "")
            {
                Dprint("レジストリデータチェック通過");
                Dprint("自動認証フラグtrue");
                flag = true;
                Dprint("自動認証メッセージフォーム準備");
                form3 = new Form3();
                Dprint("自動認証メッセージフォーム表示");
                form3.Show();
                Dprint("ログインフォーム非表示");
                Visible = false;
                Dprint("自動認証開始");
                Login(true);
            }
            else
            {
                Dprint("レジストリデータが不正");
                Dprint("ログインフォーム表示");
                Visible = true;
                return;
            }
        }

        private void アカウント情報入力_Shown(object sender, EventArgs e)
        {
            Dprint("ログインフォーム表示イベント発火");
            Dprint("自動認証フラグ:");
            Dprint(flag);
            if (flag)
            {
                Dprint("自動認証フラグtrue");
                Dprint("ログインフォームを非表示");
                Visible = false;
            }

        }

        private void ログインボタン_Click(object sender, EventArgs e)
        {
            Dprint("ログインボタンクリックイベント発火");
            Dprint("ログイン処理開始");
            Login(false);
        }

        async private void Login(bool autologin)
        {
            Dprint("ログイン画面URL:");
            var urlstring = "";

            Dprint("override.jsonのデータ取得");
            string ORjsonStr = ReadAllLine("override.json", "utf-8");
            if (ORjsonStr == null)
            {
                MessageBox.Show("override.jsonが見つかりませんでした。\n[ERROR]OpenLMS-EasyAttend.exeと同じディレクトリにoverride.jsonが存在するか確認してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                ORData[] ORjsonData;
                ORjsonData = JsonSerializer.Deserialize<ORData[]>(ORjsonStr);
                foreach (var data in ORjsonData)
                {
                    if (data.mainurl == null && data.mainurl == "")
                    {
                        continue;
                    }
                    urlstring = data.mainurl;
                }
                if (urlstring == "")
                {
                    MessageBox.Show("override.jsonを読み込む際にエラーが発生しました。\n[ERROR]mainurlの値が不正です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (JsonException)
            {
                MessageBox.Show("override.jsonを読み込む際にエラーが発生しました。\noverride.jsonの書式や項目の数が正確か確認してから再試行してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Dprint(urlstring);
            IConfiguration config = Configuration.Default.WithDefaultLoader().WithJs().WithDefaultCookies();
            IBrowsingContext context = BrowsingContext.New(config);
            Dprint("URLアクセス開始");
            IDocument doc = await context.OpenAsync(urlstring);
            Dprint("データ取得完了");
            Dprint("デバッグのためのタイトルデータ取得");
            string title = doc.Title;
            Dprint("タイトル:");
            Dprint(title);

            string id;
            string pass;

            if (autologin)
            {
                id = ID;
                pass = PASSWORD;
            }
            else
            {
                id = userid.Text;
                pass = password.Text;
            }

            Dprint("ID:");
            Dprint(id);
            Dprint("PASSWORD:");
            Dprint(pass);
            Dprint("ログイン試行中");

            _ = await context.Active.QuerySelector<IHtmlFormElement>("form").SubmitAsync(new
            {
                username = id,
                password = pass
            });

            string AfterLoginURL = context.Active.Url;

            Dprint("ログイン結果：");
            Dprint(AfterLoginURL != urlstring);
            Dprint("ログイン後URL:");
            Dprint(AfterLoginURL);
            
            if (AfterLoginURL != urlstring)
            {
                Dprint("ログイン成功");
                Dprint("レジストリに登録されているデータに変更があったか確認");

                if (!autologin && (ID != userid.Text || PASSWORD != password.Text))
                {
                    Dprint("レジストリデータを更新中");
                    RegistryKey key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\IPUT\\LMS\\AUTHDATA");
                    key.SetValue("ID", userid.Text);
                    key.SetValue("PASSWORD", password.Text);
                    key.Close();
                }
                else
                {
                    Dprint("レジストリデータを更新する必要はありません");
                }

                Dprint("ログインフォーム非表示");
                
                Visible = false;
                
                if (autologin)
                {
                    Dprint("自動認証フォーム非表示");
                    form3.Hide();
                }

                Dprint("出席パスワード入力フォーム表示");
                
                new Form2(option,context).Show();
            }
            else
            {
                Dprint("ログイン失敗");
                Dprint("再試行回数上限:");
                Dprint(retrylimit.ToString());
                Dprint("再試行回数:");
                Dprint(retry.ToString());

                if (retry < retrylimit)
                {
                    Dprint("再試行開始");
                    Dprint("再試行回数加算");
                    retry++;
                    Dprint("ログイン再試行");
                    Login(autologin);
                    return;
                }
                Dprint("再試行回数が上限に達しました");
                Dprint("再試行回数初期化");
                retry = 1;

                if (autologin)
                {
                    Dprint("自動認証フォーム非表示");
                    form3.Hide();
                    Dprint("エラーメッセージ表示");
                    MessageBox.Show("認証に失敗しました。IDとパスワードを再入力してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Dprint("エラーメッセージ表示");
                    MessageBox.Show("認証に失敗しました。IDとパスワードを確認してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Dprint("ログインフォーム表示");
                Visible = true;
            }
        }
    }
}
