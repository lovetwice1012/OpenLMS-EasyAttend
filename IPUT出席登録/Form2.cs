using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using AngleSharp;
using AngleSharp.Scripting;
using System.Text.RegularExpressions;

namespace IPUT出席登録
{
    public partial class Form2 : Form
    {
        public bool debug = false;
        public string[] option;
        public string token = "";
        public IBrowsingContext context;
        public bool debuguseronly = false;


        public static string StringBetween(string Source, string Start, string End)
        {
            string result = "";
            if (Source == null) return null;
            if (Source.Contains(Start) && Source.Contains(End))
            {
                int StartIndex = Source.IndexOf(Start, 0) + Start.Length;
                int EndIndex = Source.IndexOf(End, StartIndex);
                result = Source.Substring(StartIndex, EndIndex - StartIndex);
                return result;
            }

            return result;
        }

        public int GetDoWint(string str)
        {
            switch (str)
            {
                case "日":
                    return 0;
                case "月":
                    return 1;
                case "火":
                    return 2;
                case "水":
                    return 3;
                case "木":
                    return 4;
                case "金":
                    return 5;
                case "土":
                    return 6;
            }
            return 0;
        }

        public int GetTimefromPeriod(int num)
        {
            Dprint("*********");
            Dprint(num);
            Dprint("*********");
            switch (num)
            {
                case 1:
                    return 0;
                case 2:
                    return 105;
                case 3:
                    return 210;
                case 4:
                    return 310;
                case 5:
                    return 410;
                case 6:
                    return 510;
                case 7:
                    return 615;
            }
            return 0;
        }

        class Data
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Period { get; set; }
            public int DoW { get; set; }
        }

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
        public void Dprint(double str)
        {
            if (debug) Console.WriteLine(str);
        }
        private void Dprint(bool v)
        {
            if (debug) Console.WriteLine(v);
        }

        public Form2(string[] args, IBrowsingContext context)
        {
            InitializeComponent();
            option = args;
            
            if (option.Length != 0)
            {
                if (option[0] == "--debug") debug = true;
            }

            this.context = context;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            string attendmoduleurl = "";
            bool ORflag = false;
            bool ORChangeSTflag = false;
            string ORChangeST = "9:30";

            if (debuguseronly && !debug)
            {
                MessageBox.Show("この機能はまだ利用できません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                    if (data.attendmoduleurl == null && data.attendmoduleurl == "")
                    {
                        continue;
                    }


                    attendmoduleurl = data.attendmoduleurl;
                    ORflag = true;

                    if (data.Start_time != "9:30")
                    {
                        
                        if (data.Start_time.Split(':').Length == 1 || Int32.Parse(data.Start_time.Split(':')[0]) > 23 || Int32.Parse(data.Start_time.Split(':')[0]) < 0 || data.Start_time.Split(':')[1] == null || Int32.Parse(data.Start_time.Split(':')[1]) > 59 || Int32.Parse(data.Start_time.Split(':')[1]) < 0)
                        {
                            MessageBox.Show("override.jsonを読み込む際にエラーが発生しました。\n[ERROR]Start_timeの値が不正です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        ORChangeST = data.Start_time;
                        ORChangeSTflag = true;
                        ORflag = true;
                    }

                }

                if (attendmoduleurl == "")
                {
                    MessageBox.Show("override.jsonを読み込む際にエラーが発生しました。\n[ERROR]attendmoduleurlの値が不正です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            catch (JsonException)
            {
                MessageBox.Show("override.jsonを読み込む際にエラーが発生しました。\noverride.jsonの書式や項目の数が正確か確認してから再試行してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }




            Dprint("本日の予定の取得処理開始");
            string jsonStr = ReadAllLine("attendance.json", "utf-8");

            if (jsonStr == null)
            {
                MessageBox.Show("attendance.jsonが見つかりませんでした。\nOpenLMS-EasyAttend.exeと同じディレクトリにattendance.jsonが存在するか確認してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Dprint(jsonStr);
            
            Data[] jsonData;
            
            try
            {
                jsonData = JsonSerializer.Deserialize<Data[]>(jsonStr);
            }
            catch (JsonException)
            {
                MessageBox.Show("attendance.jsonを読み込む際にエラーが発生しました。\nattendance.jsonの書式や項目の数が正確か確認してから再試行してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Dprint("JSONから必要なデータの抽出開始");
            DateTime dt = DateTime.Now;
            int ddd = GetDoWint(dt.ToString("ddd"));
            string result = dt.ToString("HH:mm");
            TimeSpan ts1 = TimeSpan.Parse("9:30");

            if (ORflag && ORChangeSTflag)
            {
                ts1 = TimeSpan.Parse(ORChangeST);
            }
            
            TimeSpan ts2 = TimeSpan.Parse(result);
            TimeSpan ts3 = ts2 - ts1;

            Dprint(ts3.TotalMinutes);

            int id = 0;
            string name = "";

            foreach (var data in jsonData)
            {
                if (data.Id == 0) continue;
                if (ddd != data.DoW || GetTimefromPeriod(data.Period) > ts3.TotalMinutes || (GetTimefromPeriod(data.Period) + 15) < ts3.TotalMinutes) continue;
                id = data.Id;
                name = data.Name;
            }

            if (id == 0)
            {
                MessageBox.Show("出席可能な講義が見つかりませんでした。\n出席登録可能な時間を過ぎてしまった可能性があります。\nもしこれが間違いだと思われる場合はattendance.jsonの設定を見直してください。", "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            attendmoduleurl += id;
            IBrowsingContext context = this.context;
            Dprint("URLアクセス開始");
            IDocument doc = await context.OpenAsync(attendmoduleurl);
            Dprint("データ取得完了");
            Dprint("デバッグのためのタイトルデータ取得");
            string title = doc.Title;
            Dprint("タイトル:");
            Dprint(title);
            string url;


            Match match = Regex.Match(doc.ToHtml(), @"https://lms-tokyo.iput.ac.jp/mod/attendance/attendance\.php\?sessid=\d{5}&amp;sesskey=\w{10}");

            if (match == null)
            {
                MessageBox.Show("出席可能な講義が見つかりませんでした。\n出席登録可能な時間を過ぎてしまった可能性があります。\nもしこれが間違いだと思われる場合はattendance.jsonの設定を見直してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            url = match.Value;

            Dprint("attendurl");
            Dprint(url);

            doc = await context.OpenAsync(url);
            string pass = attendpassword.Text;

            Dprint("PASSWORD:");
            Dprint(pass);

            Dprint("出席登録試行中");

            _ = await doc.QuerySelector<IHtmlFormElement>("form").SubmitAsync(new
            {
                studentpassword = pass
            });

            Dprint("ログイン結果：");
            bool Authresult = context.Active.ToHtml().Contains("このセッションのあなたの出欠が記録されました。");

            Dprint(Authresult);

            if (Authresult)
            {
                Dprint("出席登録成功");
                MessageBox.Show("出席登録に成功しました。\n出席した講義:" + name, "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                Dprint("出席登録失敗");
                Dprint("エラーメッセージ表示");
                MessageBox.Show("認証に失敗しました。パスワードを確認してください。\n３回間違えるとロックがかかります。\n出席を試みた講義:" + name, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
