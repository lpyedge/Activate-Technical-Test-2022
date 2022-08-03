using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bakery.Utils;

public static class JsonUtility
{
    private static readonly JsonSerializerOptions DefaultSettings;

    static JsonUtility()
    {
        DefaultSettings = Settings.JsonSerializerSettingsFunc();
    }

    public static string Serialize<T>(T model)
    {
        return JsonSerializer.Serialize(model, DefaultSettings);
    }

    public static string SerializePretty<T>(T model)
    {
        var options = Settings.JsonSerializerSettingsFunc();
        options.WriteIndented = true;

        return JsonSerializer.Serialize(model, options);
    }

    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultSettings);
    }

    public static class Settings
    {
        public static Func<JsonSerializerOptions> JsonSerializerSettingsFunc = () =>
        {
            var setting = new JsonSerializerOptions()
                {Converters = {new DynamicJsonConverter(), new DictionaryStringObjectJsonConverter(),new BoolConverter() }};
            //var setting = new JsonSerializerOptions();
            JsonSerializerSettingsAction(setting);
            return setting;
        };

        public static Action<JsonSerializerOptions> JsonSerializerSettingsAction = (setting) =>
        {
            //https://docs.microsoft.com/zh-cn/dotnet/api/system.text.json.jsonserializeroptions?view=net-5.0

            //将对象的属性名称转换为其他格式（例如 camel 大小写）的策略；若为 null，则保持属性名称不变。
            setting.PropertyNamingPolicy = null;
            //获取或设置用于将 IDictionary 密钥名称转换为其他格式（如 camel 大小写）的策略
            setting.DictionaryKeyPolicy = null;
            //定义 JSON 是否应使用整齐打印。
            setting.WriteIndented = false;
            //如何处理对象引用。
            setting.ReferenceHandler = null; //不设置null序列化dictionary会出现额外的$id属性 ReferenceHandler.IgnoreCycles
            //序列化或反序列化时应如何处理数字类型。
            setting.NumberHandling = JsonNumberHandling.AllowReadingFromString;

            //在序列化和反序列化期间处理字段。 默认值为 false。
            setting.IncludeFields = false;
            //序列化过程中是否忽略只读字段
            setting.IgnoreReadOnlyFields = true;
            //是否 null 在序列化过程中忽略值。 默认值为 false。
            //IgnoreNullValues = false;
            //序列化过程中是否忽略只读属性
            setting.IgnoreReadOnlyProperties = false; //设置true会影响序列化值
            //在序列化或反序列化过程中忽略具有默认值的属性。 默认值为 Never。
            setting.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            
            //确定在反序列化过程中属性名称是否使用不区分大小写的比较。 默认值为 false。
            setting.PropertyNameCaseInsensitive = true;
            //定义反序列化过程中如何处理注释
            setting.ReadCommentHandling = JsonCommentHandling.Skip;
            //临时缓冲区时要使用的默认缓冲区大小（以字节为单位）。
            setting.DefaultBufferSize = new JsonSerializerOptions().DefaultBufferSize;
            //反序列化的 JSON 有效负载中是否允许（和忽略）对象或数组中 JSON 值的列表末尾多余的逗号。
            setting.AllowTrailingCommas = true;
            //获取或设置要在转义字符串时使用的编码器
            //setting.Encoder = JavaScriptEncoder.Default;
            setting.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            //序列化或反序列化 JSON 时允许的最大深度，默认值 0 表示最大深度为 64。
            setting.MaxDepth = 0;
            //https://github.com/fujieda/DynaJson
            //https://github.com/SwingCosmic/LoveKicher.DynamicJson
            //JsonSerializer.Deserialize 返回dynamic类型对象 默认不支持，手动实现
            setting.Converters.Add(new DynamicJsonConverter());
            setting.Converters.Add(new DictionaryStringObjectJsonConverter());
            setting.Converters.Add(new BoolConverter());
        };

        //private static void JsonSerializerSettingsInit(ref JsonSerializerOptions setting)
        //{
        //    setting = new JsonSerializerOptions()
        //    {
        //        //将对象的属性名称转换为其他格式（例如 camel 大小写）的策略；若为 null，则保持属性名称不变。
        //        PropertyNamingPolicy = null,
        //        //获取或设置用于将 IDictionary 密钥名称转换为其他格式（如 camel 大小写）的策略
        //        DictionaryKeyPolicy = null,
        //        //定义 JSON 是否应使用整齐打印。
        //        WriteIndented = false,
        //        //如何处理对象引用。
        //        ReferenceHandler = null, //不设置null序列化dictionary会出现额外的$id属性
        //                                         //序列化或反序列化时应如何处理数字类型。
        //        NumberHandling = JsonNumberHandling.AllowReadingFromString,

        //        //在序列化和反序列化期间处理字段。 默认值为 false。
        //        IncludeFields = false,
        //        //序列化过程中是否忽略只读字段
        //        IgnoreReadOnlyFields = true,
        //        //是否 null 在序列化过程中忽略值。 默认值为 false。
        //        //IgnoreNullValues = false;
        //        //序列化过程中是否忽略只读属性
        //        IgnoreReadOnlyProperties = false,  //设置true会影响序列化值
        //                                                    //在序列化或反序列化过程中忽略具有默认值的属性。 默认值为 Never。
        //        DefaultIgnoreCondition = JsonIgnoreCondition.Never,

        //        //确定在反序列化过程中属性名称是否使用不区分大小写的比较。 默认值为 false。
        //        PropertyNameCaseInsensitive = true,
        //        //定义反序列化过程中如何处理注释
        //        ReadCommentHandling = JsonCommentHandling.Skip,
        //        //临时缓冲区时要使用的默认缓冲区大小（以字节为单位）。
        //        DefaultBufferSize = new JsonSerializerOptions().DefaultBufferSize,
        //        //反序列化的 JSON 有效负载中是否允许（和忽略）对象或数组中 JSON 值的列表末尾多余的逗号。
        //        AllowTrailingCommas = true,
        //        //获取或设置要在转义字符串时使用的编码器
        //        Encoder = JavaScriptEncoder.Default,
        //        //序列化或反序列化 JSON 时允许的最大深度，默认值 0 表示最大深度为 64。
        //        MaxDepth = 0,
        //        //https://github.com/fujieda/DynaJson
        //        //https://github.com/SwingCosmic/LoveKicher.DynamicJson
        //        //JsonSerializer.Deserialize 返回dynamic类型对象 默认不支持，手动实现
        //        //

        //        Converters =  { new DynamicJsonConverter() , new DictionaryStringObjectJsonConverter() }
        //    };
        //}

        private class DynamicJsonConverter : JsonConverter<dynamic>
        {
            public override dynamic Read(ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.True) return true;

                if (reader.TokenType == JsonTokenType.False) return false;

                if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetInt64(out var l)) return l;

                    return reader.GetDouble();
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    if (reader.TryGetDateTime(out var datetime)) return datetime;

                    return reader.GetString();
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    using var documentV = JsonDocument.ParseValue(ref reader);
                    return ReadObject(documentV.RootElement);
                }

                // Use JsonElement as fallback.
                // Newtonsoft uses JArray or JObject.
                var document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.Clone();
            }

            private object ReadObject(JsonElement jsonElement)
            {
                IDictionary<string, object> expandoObject = new ExpandoObject();
                foreach (var obj in jsonElement.EnumerateObject())
                {
                    var k = obj.Name;
                    var value = ReadValue(obj.Value);
                    expandoObject[k] = value;
                }

                return expandoObject;
            }

            private object? ReadValue(JsonElement jsonElement)
            {
                object? result = null;
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Object:
                        result = ReadObject(jsonElement);
                        break;
                    case JsonValueKind.Array:
                        result = ReadList(jsonElement);
                        break;
                    case JsonValueKind.String:
                        //TODO: Missing Datetime&Bytes Convert
                        result = jsonElement.GetString();
                        break;
                    case JsonValueKind.Number:
                        //TODO: more num type
                        result = 0;
                        if (jsonElement.TryGetInt64(out var l)) result = l;
                        break;
                    case JsonValueKind.True:
                        result = true;
                        break;
                    case JsonValueKind.False:
                        result = false;
                        break;
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        result = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return result;
            }

            private object? ReadList(JsonElement jsonElement)
            {
                IList<object?> list = new List<object?>();
                foreach (var item in jsonElement.EnumerateArray()) list.Add(ReadValue(item));
                return list.Count == 0 ? null : list;
            }

            public override void Write(Utf8JsonWriter writer,
                object value,
                JsonSerializerOptions options)
            {
                // writer.WriteStringValue(value.ToString());
            }
        }

        public class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
        {
            public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (var key in value.Keys)
                {
                    HandleValue(writer, key, value[key]);
                }

                writer.WriteEndObject();
            }

            private static void HandleValue(Utf8JsonWriter writer, string key, object objectValue)
            {
                if (key != null)
                {
                    writer.WritePropertyName(key);
                }

                switch (objectValue)
                {
                    case string stringValue:
                        writer.WriteStringValue(stringValue);
                        break;
                    case DateTime dateTime:
                        writer.WriteStringValue(dateTime);
                        break;
                    case long longValue:
                        writer.WriteNumberValue(longValue);
                        break;
                    case int intValue:
                        writer.WriteNumberValue(intValue);
                        break;
                    case float floatValue:
                        writer.WriteNumberValue(floatValue);
                        break;
                    case double doubleValue:
                        writer.WriteNumberValue(doubleValue);
                        break;
                    case decimal decimalValue:
                        writer.WriteNumberValue(decimalValue);
                        break;
                    case bool boolValue:
                        writer.WriteBooleanValue(boolValue);
                        break;
                    case Dictionary<string, object> dict:
                        writer.WriteStartObject();
                        foreach (var item in dict)
                        {
                            HandleValue(writer, item.Key, item.Value);
                        }

                        writer.WriteEndObject();
                        break;
                    case object[] array:
                        writer.WriteStartArray();
                        foreach (var item in array)
                        {
                            HandleValue(writer, item);
                        }

                        writer.WriteEndArray();
                        break;
                    default:
                        writer.WriteNullValue();
                        break;
                }
            }

            private static void HandleValue(Utf8JsonWriter writer, object value)
            {
                HandleValue(writer, null, value);
            }

            public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
        
        public class BoolConverter : JsonConverter<bool>
        {
            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
                writer.WriteBooleanValue(value);

            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    JsonTokenType.String => bool.TryParse(reader.GetString(), out var b) ? b : throw new JsonException(),
                    JsonTokenType.Number => reader.TryGetInt64(out long l) ? Convert.ToBoolean(l) : reader.TryGetDouble(out double d) ? Convert.ToBoolean(d) : false,
                    _ => throw new JsonException(),
                };
        }
    }
}