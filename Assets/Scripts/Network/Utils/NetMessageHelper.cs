using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace HFantasy.Network.Utils
{
    public static class NetMessageHelper
    {
        static JsonConverter converter = new UnityTypeConverter();
        public static byte[] Pack<T>(NetMessageType type, T payload)
        {
            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, converter));

            NetMessage message = new NetMessage
            {
                MsgType = type,
                Payload = payloadBytes
            };

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, converter));
        }

        public static NetMessage Unpack(byte[] raw)
        {
            string json = Encoding.UTF8.GetString(raw);
            return JsonConvert.DeserializeObject<NetMessage>(json, converter);
        }

        public static T DecodePayload<T>(NetMessage message)
        {
            string json = Encoding.UTF8.GetString(message.Payload);
            return JsonConvert.DeserializeObject<T>(json, converter);
        }

        public static string DebugDump(NetMessage msg)
        {
            return $"[NetMessage] Type={msg.MsgType}, Size={msg.Payload.Length}";
        }
    }



    /// <summary>
    /// 支持 Unity 各类 Vector 和 Color 类型的 JsonConverter
    /// </summary>
    public class UnityTypeConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2) ||
                   objectType == typeof(Vector2Int) ||
                   objectType == typeof(Vector3) ||
                   objectType == typeof(Vector3Int) ||
                   objectType == typeof(Vector4) ||
                   objectType == typeof(Color) ||
                   objectType == typeof(Color32);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(reader);

            if (objectType == typeof(Vector2))
                return new Vector2((float)jo["x"], (float)jo["y"]);
            if (objectType == typeof(Vector2Int))
                return new Vector2Int((int)jo["x"], (int)jo["y"]);
            if (objectType == typeof(Vector3))
                return new Vector3((float)jo["x"], (float)jo["y"], (float)jo["z"]);
            if (objectType == typeof(Vector3Int))
                return new Vector3Int((int)jo["x"], (int)jo["y"], (int)jo["z"]);
            if (objectType == typeof(Vector4))
                return new Vector4((float)jo["x"], (float)jo["y"], (float)jo["z"], (float)jo["w"]);
            if (objectType == typeof(Color))
                return new Color((float)jo["r"], (float)jo["g"], (float)jo["b"], (float)jo["a"]);
            if (objectType == typeof(Color32))
                return new Color32((byte)jo["r"], (byte)jo["g"], (byte)jo["b"], (byte)jo["a"]);

            throw new Exception("Unsupported type in UnityTypeConverter.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case Vector2 v:
                    writer.WritePropertyName("x"); writer.WriteValue(v.x);
                    writer.WritePropertyName("y"); writer.WriteValue(v.y);
                    break;
                case Vector2Int v:
                    writer.WritePropertyName("x"); writer.WriteValue(v.x);
                    writer.WritePropertyName("y"); writer.WriteValue(v.y);
                    break;
                case Vector3 v:
                    writer.WritePropertyName("x"); writer.WriteValue(v.x);
                    writer.WritePropertyName("y"); writer.WriteValue(v.y);
                    writer.WritePropertyName("z"); writer.WriteValue(v.z);
                    break;
                case Vector3Int v:
                    writer.WritePropertyName("x"); writer.WriteValue(v.x);
                    writer.WritePropertyName("y"); writer.WriteValue(v.y);
                    writer.WritePropertyName("z"); writer.WriteValue(v.z);
                    break;
                case Vector4 v:
                    writer.WritePropertyName("x"); writer.WriteValue(v.x);
                    writer.WritePropertyName("y"); writer.WriteValue(v.y);
                    writer.WritePropertyName("z"); writer.WriteValue(v.z);
                    writer.WritePropertyName("w"); writer.WriteValue(v.w);
                    break;
                case Color c:
                    writer.WritePropertyName("r"); writer.WriteValue(c.r);
                    writer.WritePropertyName("g"); writer.WriteValue(c.g);
                    writer.WritePropertyName("b"); writer.WriteValue(c.b);
                    writer.WritePropertyName("a"); writer.WriteValue(c.a);
                    break;
                case Color32 c32:
                    writer.WritePropertyName("r"); writer.WriteValue(c32.r);
                    writer.WritePropertyName("g"); writer.WriteValue(c32.g);
                    writer.WritePropertyName("b"); writer.WriteValue(c32.b);
                    writer.WritePropertyName("a"); writer.WriteValue(c32.a);
                    break;
                default:
                    throw new Exception("Unsupported type in UnityTypeConverter.");
            }

            writer.WriteEndObject();
        }
    }


}

