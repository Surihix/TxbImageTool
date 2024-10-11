using System.Text.Json;
using TxbImageTool.Support;

namespace TxbImageTool.Conversion
{
    internal class JSONMethods
    {
        public static void CheckTokenType(string tokenType, ref Utf8JsonReader jsonReader, string property)
        {
            _ = jsonReader.Read();

            switch (tokenType)
            {
                case "Array":
                    if (jsonReader.TokenType != JsonTokenType.StartArray)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a number");
                    }
                    break;

                case "Bool":
                    if (jsonReader.TokenType != JsonTokenType.True)
                    {
                        if (jsonReader.TokenType != JsonTokenType.False)
                        {
                            SharedMethods.ErrorExit($"Specified {property} property's value is not a boolean");
                        }
                    }
                    break;

                case "Number":
                    if (jsonReader.TokenType != JsonTokenType.Number)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a number");
                    }
                    break;

                case "PropertyName":
                    if (jsonReader.TokenType != JsonTokenType.PropertyName)
                    {
                        SharedMethods.ErrorExit($"{property} type is not a valid PropertyName");
                    }
                    break;

                case "String":
                    if (jsonReader.TokenType != JsonTokenType.String)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a string");
                    }
                    break;
            }
        }


        public static void CheckPropertyName(ref Utf8JsonReader jsonReader, string propertyName)
        {
            if (jsonReader.GetString() != propertyName)
            {
                SharedMethods.ErrorExit($"Missing {propertyName} property at expected position");
            }
        }
    }
}