using LLMJson;
using System.Text.Json;

namespace Sigma.Tests
{
    public class FunctionSchema
    {
        public string Function { get; set; }
        public string Intention { get; set; }
        public JsonElement Arguments { get; set; }
    }

    public class JsonParserTests
    {
        [Fact]
        public void JsonParse()
        {
            var json = """
                [{
                   "function": "get_order",
                   "intention": "get an order",
                   "arguments": {"id": 123},
                },{
                   "function": "get_order",
                   "intention": "get an order",
                   "arguments": {"id": 789},
                }]
                """;
            
            var results = JsonParser.FromJson<List<FunctionSchema>>(json);

            Assert.True(results.Count == 2);
        }


        [Fact]
        public void Parse_object_to_array()
        {
            var json = """
                {
                   "function": "get_order",
                   "intention": "get an order",
                   "arguments": {"id": 123},
                }
                """;

            var results = JsonParser.FromJson<List<FunctionSchema>>(json);

            Assert.NotNull(results);
            Assert.True(results.Count == 1);
            Assert.True(results[0].Arguments.ValueKind == JsonValueKind.Object);
        }

        [Fact]
        public void Parse_paramsters()
        {
            var arguments = """
                {
                    "id": 123,
                    "string":"hello",
                    "numberString":123,
                    "stringNumber":"123",
                    "stringBool":"true"
                }
                """;
            var parameters = new Dictionary<string, Type>
            {
                ["id"] = typeof(int),
                ["string"]=typeof(string),
                ["numberString"] = typeof(string),
                ["stringNumber"] = typeof(int),
                ["stringBool"] = typeof(bool),
            };
            var result = JsonParameterParser.ParseJsonToDictionary(JsonDocument.Parse(arguments).RootElement, parameters);

            Assert.Equal(123, result["id"]);
            Assert.Equal("hello", result["string"]);
            Assert.Equal("123", result["numberString"]);
            Assert.Equal(123, result["stringNumber"]);
            Assert.Equal(true, result["stringBool"]);
        }
    }
}