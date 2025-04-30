using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace HFantasy.Script.Common.Utils
{
    public static class CsvUtil
    {
        public static List<T> ParseCsvToList<T>(TextAsset csvAsset) where T : new()
        {
            var list = new List<T>();
            if (csvAsset == null)
            {
                Debug.LogError("CsvAsset为空");
                return list;
            }

            string[] lines = csvAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1) return list;

            string[] headers = lines[0].Trim().Split(',');
            Type type = typeof(T);
            Dictionary<string, FieldInfo> fieldMap = new();

            foreach (var header in headers)
            {
                FieldInfo field = type.GetField(header.Trim(), BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    fieldMap[header.Trim()] = field;
                }
                else
                {
                    Debug.LogWarning($"Csv字段[{header}]未在类型{type.Name}中找到对应字段");
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split(',');
                T obj = new T();

                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    string header = headers[j].Trim();
                    string valueStr = values[j].Trim();

                    if (!fieldMap.TryGetValue(header, out FieldInfo field)) continue;
                    if (string.IsNullOrEmpty(valueStr)) continue;

                    try
                    {
                        object value = null;
                        if (field.FieldType == typeof(int))
                            value = int.Parse(valueStr);
                        else if (field.FieldType == typeof(float))
                            value = float.Parse(valueStr);
                        else if (field.FieldType == typeof(string))
                            value = valueStr;
                        else if (field.FieldType.IsEnum)
                            value = Enum.Parse(field.FieldType, valueStr);

                        field.SetValue(obj, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"字段[{header}]赋值失败，值：{valueStr}，错误：{ex.Message}");
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        public static void WriteCsv<T>(List<T> dataList, string path) where T : class
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();

            // 表头
            for (int i = 0; i < fields.Length; i++)
            {
                sb.Append(fields[i].Name);
                if (i < fields.Length - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // 内容
            foreach (var item in dataList)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    object value = fields[i].GetValue(item);
                    string valueStr = value != null ? value.ToString() : "";
                    sb.Append(valueStr);

                    if (i < fields.Length - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            // 确保文件夹存在
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
