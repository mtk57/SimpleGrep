using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace GrepLib
{
    public static class Utils
    {
        [DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern uint GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpszLongPath,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder lpszShortPath,
            uint cchBuffer);

        /// <summary>
        /// 探す文字列（ワイルドカード可）から正規表現を返す
        /// </summary>
        /// <param name="ptn">探す文字列（ワイルドカード可）</param>
        /// <returns>正規表現</returns>
        public static Regex GetWildCardRegEx(string ptn)
        {
            var reg = Regex.Escape(ptn);
            reg = reg.Replace(@"\*", ".*?");
            reg = reg.Replace(@"\?", ".");

            return new Regex(reg, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 短いファイルパス名を取得する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>短いパス名</returns>
        public static string GetShortPath(string path)
        {
            var sb = new StringBuilder(1024);
            var ret = GetShortPathName(path, sb, (uint)sb.Capacity);
            if(ret == 0)
            {
                throw new Exception("Failed to get short file name.");
            }
            return sb.ToString();
        }

        /// <summary>
        /// ファイル名をユニークにして返す
        /// </summary>
        /// <param name="inName">ファイル名</param>
        /// <returns>ユニークなファイル名</returns>
        public static string GetUniqueFileNmae(string inName)
        {
            const string FMT_FILENAME = "{0}_{1}{2}";
            var fileName = Path.GetFileNameWithoutExtension(inName);
            var ext = Path.GetExtension(inName);
            var uniqueStr = DateTime.Now.ToString(Const.FORMAT_YYYYMMDDHHMMSSFFF);
            return string.Format(FMT_FILENAME, fileName, uniqueStr, ext);
        }

        /// <summary>
        /// メッセージボックスの表示とログ出力を行う
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public static void ShowMessageBoxAndWriteLog(string msg)
        {
            MessageBox.Show(msg);
            Logger.Write(msg);
        }

        /// <summary>
        /// メッセージボックスの表示とログ出力を行う（例外用）
        /// </summary>
        /// <param name="ex">例外</param>
        public static void ShowMessageBoxAndWriteLogForException(Exception ex)
        {
            var msg = string.Format(Const.FORMAT_EXCEPTION_MESSAGE, ex.Message, ex.StackTrace);
            ShowMessageBoxAndWriteLog(msg);
        }

        /// <summary>
        /// バイナリーファイルを文字列に変換する
        /// ただしエンコードはShift-JIS固定
        /// </summary>
        /// <param name="fs">FileStream</param>
        /// <param name="replace">制御文字を置換する文字</param>
        /// <returns>文字列</returns>
        public static List<string> ConvertBinaryToString(FileStream fs, char replace = ' ')
        {
            Encoding enc = Encoding.GetEncoding("Shift_JIS");

            using(var br = new BinaryReader(fs))
            {
                var bytes = br.ReadBytes((int)fs.Length).Select(x =>
                {
                    if((x < 0x0A)
                        || (x == 0x0B || x == 0x0C)
                        || (x > 0x0D && x < 0x20)
                        || (x == 0x7F))
                    {
                        // 制御文字は置換(ただし0x0A, 0x0Dはそのまま使う)
                        x = (byte)replace;
                    }
                    else if(x == 0x0D)
                    {
                        x = 0x0A;
                    }
                    return x;
                });

                return enc.GetString(bytes.ToArray()).Split('\n').ToList();
            }
        }

        /// <summary>
        /// StreamReaderにReadAllLinesメソッドを追加する
        /// </summary>
        /// <param name="sr">StreamReaderにReadAllLinesメソッドを追加する</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadAllLines(this StreamReader sr)
        {
            var line = string.Empty;
            while((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// テキストファイルにデータを書き込む
        /// テキストファイルは「GUID+拡張子」で作成される
        /// </summary>
        /// <param name="dir">出力ディレクトリー</param>
        /// <param name="writeData">書き込むデータ</param>
        /// <param name="extension">拡張子(Ex.".txt")</param>
        /// <returns>作成したファイルパス</returns>
        public static string WriteTextFile(DirectoryInfo dir, string writeData, string extension)
        {
            var fileName = Guid.NewGuid().ToString() + extension;

            var filePath = Path.Combine(dir.FullName, fileName);

            using(var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.Write(writeData);
            }
            return filePath;
        }

        /// <summary>
        /// バイナリーファイルか否か
        /// </summary>
        /// <param name="file">ファイルパス</param>
        /// <returns>True:バイナリーファイル, False:バイナリーファイルではない</returns>
        public static bool IsBinaryFile(FileInfo file)
        {
            using(var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return IsBinaryFile(fs);
            } 
        }

        /// <summary>
        /// バイナリーファイルか否か
        /// </summary>
        /// <param name="fs">ファイルストリーム(呼び出し元で閉じること!)</param>
        /// <returns>True:バイナリーファイル, False:バイナリーファイルではない</returns>
        public static bool IsBinaryFile(FileStream fs)
        {
            fs.Position = 0;
            var byteData = new byte[1];
            while(fs.Read(byteData, 0, byteData.Length) > 0)
            {
                if(byteData[0] <= 0x08)
                //if(byteData[0] == 0x00)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ファイルのエンコードを返す
        /// </summary>
        /// <param name="file">ファイル</param>
        /// <returns>エンコード</returns>
        public static Encoding GetEncode(FileInfo file)
        {
            var bytes = File.ReadAllBytes(file.FullName);

            var ret = GetCode(bytes);

            if(ret == null)
            {
                ret = Encoding.Default;
            }
            return ret;
        }

        /// <summary>
        /// ファイルのエンコードを返す
        /// </summary>
        /// <param name="fs">ファイルストリーム(呼び出し元で閉じること!)</param>
        /// <returns>エンコード</returns>
        public static Encoding GetEncode(FileStream fs)
        {
            fs.Position = 0;
            var bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);

            var ret = GetCode(bytes);

            if(ret == null)
            {
                ret = Encoding.Default;
            }
            return ret;
        }

        /// <summary>
        /// 文字コードを判別する
        /// https://dobon.net/vb/dotnet/string/detectcode.html
        /// </summary>
        /// <remarks>
        /// Jcode.pmのgetcodeメソッドを移植したものです。
        /// Jcode.pm(http://openlab.ring.gr.jp/Jcode/index-j.html)
        /// Jcode.pmの著作権情報
        /// Copyright 1999-2005 Dan Kogai <dankogai@dan.co.jp>
        /// This library is free software; you can redistribute it and/or modify it
        ///  under the same terms as Perl itself.
        /// </remarks>
        /// <param name="bytes">文字コードを調べるデータ</param>
        /// <returns>適当と思われるEncodingオブジェクト。
        /// 判断できなかった時はnull。</returns>
        public static System.Text.Encoding GetCode(byte[] bytes)
        {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = bytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for(int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if(b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    //'binary'
                    isBinary = true;
                    if(b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                    {
                        //smells like raw unicode
                        return System.Text.Encoding.Unicode;
                    }
                }
            }
            if(isBinary)
            {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for(int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if(b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if(notJapanese)
            {
                return System.Text.Encoding.ASCII;
            }

            for(int i = 0; i < len - 2; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                b3 = bytes[i + 2];

                if(b1 == bEscape)
                {
                    if(b2 == bDollar && b3 == bAt)
                    {
                        //JIS_0208 1978
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if(b2 == bDollar && b3 == bB)
                    {
                        //JIS_0208 1983
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if(b2 == bOpen && (b3 == bB || b3 == bJ))
                    {
                        //JIS_ASC
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if(b2 == bOpen && b3 == bI)
                    {
                        //JIS_KANA
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    if(i < len - 3)
                    {
                        b4 = bytes[i + 3];
                        if(b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            //JIS_0212
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                        if(i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        {
                            //JIS_0208 1990
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for(int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if(((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    //SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for(int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if(((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    i++;
                }
                else if(i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if(b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for(int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    //UTF8
                    utf8 += 2;
                    i++;
                }
                else if(i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF))
                    {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            System.Diagnostics.Debug.WriteLine(
                string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if(euc > sjis && euc > utf8)
            {
                //EUC
                return System.Text.Encoding.GetEncoding(51932);
            }
            else if(sjis > euc && sjis > utf8)
            {
                //SJIS
                return System.Text.Encoding.GetEncoding(932);
            }
            else if(utf8 > euc && utf8 > sjis)
            {
                //UTF8
                return System.Text.Encoding.UTF8;
            }

            return null;
        }

        /// <summary>
        /// XMLファイルからオブジェクトにデシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">XMLファイルパス</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(string path, Type type)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentException("Parameter is null.");
            if(!System.IO.File.Exists(path)) throw new ArgumentException(string.Format("File not found.[{0}]", path));

            T obj;

            using(var reader = XmlReader.Create(path))
            {
                var serializer = new XmlSerializer(type);
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

        /// <summary>
        /// オブジェクトをXMLファイルにシリアライズする
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <param name="path">XMLファイルパス</param>
        /// <param name="obj">オブジェクト</param>
        public static void ObjectToXml<T>(string path, T obj)
        {
            if(string.IsNullOrEmpty(path) || obj == null) throw new ArgumentException("Parameter is null.");

            using(var writer = XmlWriter.Create(path))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }
        }
    }

    // ナノ秒という単位の精度で経過時間を取得する
    // http://yas-hummingbird.blogspot.com/2008/12/blog-post.html
    public class TraceTime
    {
        #region
        [DllImport("kernel32.dll")]
        extern static int QueryPerformanceCounter(ref long x);

        [DllImport("kernel32.dll")]
        extern static int QueryPerformanceFrequency(ref long x);
        #endregion

        #region フィールド
        long st;
        long end;
        long frq;
        #endregion

        public TraceTime()
        {
            st = 0;
            end = 0;
            QueryPerformanceFrequency(ref frq);
        }

        public void Start()
        {
            QueryPerformanceCounter(ref st);
        }

        public void Stop()
        {
            QueryPerformanceCounter(ref end);
        }

        public double NowResult()
        {
            long now = 0;
            QueryPerformanceCounter(ref now);
            return (double)(now - st) / frq;
        }

        public double Result()
        {
            return (double)(end - st) / frq;
        }
    }
}
