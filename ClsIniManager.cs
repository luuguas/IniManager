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
        private SortedDictionary<string, SortedDictionary<string, string>> _data;

        public ClsIniManager()
        {
            //配列の初期化
            _data = new SortedDictionary<string, SortedDictionary<string, string>>();
        }

        public bool ReadFile(string path)
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
                string lastSection = "";
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
                        //セクションを変更
                        lastSection = line.Substring(1, line.Length - 2);
                        _data.Add(lastSection, new SortedDictionary<string, string>());
                    }
                    else
                    {
                        separator = line.IndexOf("=");
                        if (separator > 0)
                        {
                            //キーと値を追加
                            _data[lastSection].Add(line.Substring(0, separator), line.Substring(separator + 1));
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

        public bool WriteFile(string path)
        {
            //ini拡張子がついていなければ終了
            string extension = path.Substring(path.Length - 4);
            if (extension != ".ini" && extension != ".INI")
            {
                goto failure;
            }

            using (var writer = new StreamWriter(path, false))
            {
                foreach (var section in _data)
                {
                    //セクション名を書き込み
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var keyValue in section.Value)
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

        public bool SetValue(string section, string key, string value)
        {
            if (_data.ContainsKey(section))
            {
                if (_data[section].ContainsKey(key))
                {
                    //値を変更
                    _data[section][key] = value;
                    return true;
                }
                else
                {
                    //キーを追加
                    _data[section].Add(key, value);
                    return false;
                }
            }
            else
            {
                //セクションとキーを追加
                var keyValues = new SortedDictionary<string, string>();
                keyValues.Add(key, value);
                _data.Add(section, keyValues);
                return false;
            }
        }

        public bool GetValue(string section, string key, out string value)
        {
            var trySection = new SortedDictionary<string, string>();
            bool result = _data.TryGetValue(section, out trySection);
            if (result)
            {
                string tryKey;
                result = trySection.TryGetValue(key, out tryKey);
                if (result)
                {
                    //値を出力
                    value = tryKey;
                    return true;
                }
                else
                {
                    //キー無し
                    value = null;
                    return false;
                }
            }
            else
            {
                //セクション無し
                value = null;
                return false;
            }
        }

        public bool DeleteKeyValue(string section, string key)
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
            {
                //キーを削除
                _data[section].Remove(key);
                return true;
            }
            else
            {
                //キー無し
                return false;
            }
        }

        public bool DeleteSection(string section)
        {
            if (_data.ContainsKey(section))
            {
                //セクションを削除
                _data.Remove(section);
                return true;
            }
            else
            {
                //セクション無し
                return false;
            }
        }

        public SortedDictionary<string, SortedDictionary<string, string>> GetData()
        {
            //配列を返す
            return _data;
        }
    }
}
