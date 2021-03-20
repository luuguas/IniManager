using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoClip2
{
    class ClsIniManager
    {
        SortedDictionary<string, SortedDictionary<string, string>> _data;

        ClsIniManager()
        {
            //配列の初期化
            _data = new SortedDictionary<string, SortedDictionary<string, string>>();
        }

        bool ReadFile(string path)
        {
            //ini拡張子がついていなければ終了
            string extension = path.Substring(path.Length - 4);
            if (extension != ".ini" && extension != ".INI")
            {
                goto failure;
            }

            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                string line = "";
                string lastSession = "";
                int separator = -1;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    else if (line.Substring(0, 1) == "[" && line.Substring(line.Length - 1) == "]")
                    {
                        //セッションを変更
                        lastSession = line.Substring(1, line.Length - 2);
                        _data.Add(lastSession, new SortedDictionary<string, string>());
                    }
                    else
                    {
                        separator = line.IndexOf("=");
                        if (separator > 0)
                        {
                            //キーと値を追加
                            _data[lastSession].Add(line.Substring(0, separator), line.Substring(separator + 1));
                        }
                        else
                        {
                            //失敗♡
                            goto failure;
                        }
                    }
                }
            }
            //読み込み成功
            goto success;

        success:
            return true;
        failure:
            return false;
        }

        bool WriteFile(string path)
        {
            //ini拡張子がついていなければ終了
            string extension = path.Substring(path.Length - 4);
            if (extension != ".ini" && extension != ".INI")
            {
                goto failure;
            }

            using (var writer = new StreamWriter(path, false))
            {
                foreach (var session in _data)
                {
                    //セッション名を書き込み
                    writer.WriteLine($"[{session.Key}]");
                    foreach (var keyValue in session.Value)
                    {
                        //キーと値を書き込み
                        writer.WriteLine($"{keyValue.Key}={keyValue.Value}");
                    }
                    writer.WriteLine("");
                }
            }
            //書き込み成功
            goto success;

        success:
            return true;
        failure:
            return false;
        }
    }
}
