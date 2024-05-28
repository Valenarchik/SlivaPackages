﻿using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace Localization
{
    public static class CSVManager
    {
        private static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

        public static string[] ImportTransfersByKey(LocalizationSettings settings, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log("Id is empty!");
                return null;
            }
            
            key = key.Replace(",", "*");

            var data = Resources.Load(settings.CSVFileTranslate.name) as TextAsset;

            var keys = Regex.Split(CommaFormat(data.text), LINE_SPLIT_RE);
            var result = new string[settings.languages.Count];

            for (var i = 1; i < keys.Length; i++)
            {
                var translates = Regex.Split(keys[i], ",");
                if (translates[0] != key) continue;

                for (var i2 = 0; i2 < settings.languages.Count; i2++)
                {
                    if (!LangMethods.GetLangArr(settings)[i2]) continue;

                    result[i2] = translates[i2 + 1].Replace("*", ",").Replace(@"\n", "\n");
                }

                return result;
            }
            Debug.Log("Couldn't find a translation for this object!");
            return null;
        }
        
        public static void WriteCSVFile(LocalizationSettings settings, string[,] keys, string[] idArray)
        {
            string textFile = "";
            string[] oldIDs = null;

            if (!File.Exists(Patch(settings))) // если файла нет
            {
                // Записываем первую строку
                textFile = CreateFirstLine(settings);
            }
            else // Если файл есть
            {
                oldIDs = File.ReadAllLines(Patch(settings));
                oldIDs = CommaFormat(oldIDs);

                // Записываем строки старых ключ-значений
                for (int i = 0; i < oldIDs.Length; i++)
                {
                    oldIDs[i] = RedLineFormat(AsteriskFormat(oldIDs[i]));
                    textFile += oldIDs[i] + "\n";
                }

                // Создаем из поля oldIDs массив с ID старых переводов
                for (int i = 1; i < oldIDs.Length; i++)
                {
                    oldIDs[i] = oldIDs[i];
                    int remIndex = oldIDs[i].IndexOf(",") + 1;
                    oldIDs[i] = oldIDs[i].Remove(remIndex);
                }
            }

            // Запускаем цикл для записи новых ключ-значений
            for (int column = 0; column < idArray.Length; column++)
            {
                // Проверка существует ли уже такой ключ
                bool clone = false;

                if (oldIDs != null)
                {
                    for (int i = 0; i < oldIDs.Length; i++)
                    {
                        if (idArray[column] + "," == oldIDs[i])
                        {
                            clone = true;
                        }
                    }
                }

                // Запись текста
                if (!clone)
                {
                    for (int line = 0; line < LangMethods.GetLangArr(settings).Length + 1; line++)
                    {
                        keys[column, line] = RedLineFormat(AsteriskFormat(keys[column, line])).Replace(",", "*");
                        textFile += keys[column, line];

                        if (line != LangMethods.GetLangArr(settings).Length)
                            textFile += ",";
                    }

                    textFile += "\n";
                }
            }

            WriteCSV(settings, textFile);
        }
        
        public static void SetIDLineFile(LocalizationSettings settings, string key, string[] languages)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log("Id is empty!");
                return;
            }
            
            CreateCSVFileIfNotExist(settings);

            var lines = File.ReadAllLines(Patch(settings));
            lines = CommaFormat(lines);

            string replace = null;

            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(key + ","))
                {
                    replace = AsteriskFormat(key);

                    for (int i2 = 0; i2 < LangMethods.GetLangArr(settings).Length; i2++)
                    {
                        replace += "," + RedLineFormat(languages[i2]).Replace(",", "*");
                    }

                    lines[i] = replace;
                    break;
                }
            }

            var result = lines.Aggregate("", (current, t) => current + (t + "\n"));

            if (replace == null)
            {
                result += RedLineFormat(AsteriskFormat(key));

                for (int i = 0; i < LangMethods.GetLangArr(settings).Length; i++)
                {
                    result += "," + RedLineFormat(languages[i]).Replace(",", "*");
                }

                result += "\n";
            }

            WriteCSV(settings, result);
        }

        private static void CreateCSVFileIfNotExist(LocalizationSettings settings)
        {
            if (File.Exists(Patch(settings))) return;

            using var file = new FileStream(Patch(settings), FileMode.Create);
            using var stream = new StreamWriter(file);
            stream.Write(CreateFirstLine(settings), Patch(settings));
        }

        private static string Patch(LocalizationSettings settings)
        {
            var patch = Application.dataPath + "/Resources/";

            if (!File.Exists(patch))
                Directory.CreateDirectory(patch);

            return patch + settings.CSVFileTranslate.name + ".csv";
        }

        private static string CreateFirstLine(LocalizationSettings infoYG)
        {
            var firstLine = "KEYLANGUAGE";

            for (int i = 0; i < LangMethods.GetLangArr(infoYG).Length; i++)
            {
                firstLine += "," + LangMethods.FullNameLanguages()[i];
            }

            firstLine += "\n";

            return firstLine;
        }

        private static void WriteCSV(LocalizationSettings settings, string data)
        {
            if (settings.CSVFileTranslate.format == FileFormat.ExcelOffice)
                data = SemicolonFormat(data);

            data = AsteriskFormat(data);

            using (FileStream file = new FileStream(Patch(settings), FileMode.Create))
            using (StreamWriter stream = new StreamWriter(file))
                stream.Write(data, Patch(settings));
        }
        
        public static string CommaFormat(string line)
        {
            return AsteriskFormat(line.Replace(";", ","));
        }

        private static string[] CommaFormat(string[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = CommaFormat(lines[i]);
            }

            return lines;
        }

        private static string SemicolonFormat(string line)
        {
            return string.IsNullOrEmpty(line) ? string.Empty : line.Replace(",", ";");
        }

        private static string AsteriskFormat(string line)
        {
            return string.IsNullOrEmpty(line) ? string.Empty : line.Replace(", ", "* ");
        }

        private static string RedLineFormat(string line)
        {
            return string.IsNullOrEmpty(line) ? string.Empty : line.Replace("\n", @"\n");
        }
    }
}