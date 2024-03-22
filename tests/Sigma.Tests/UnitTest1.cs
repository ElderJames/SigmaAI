using LLMJson;

namespace Sigma.Tests
{
    public class FunctionSchema
    {
        public string Function { get; set; }
        public string Intention { get; set; }
        public Dictionary<string, object> Arguments { get; set; }
    }

    public class UnitTest1
    {
        [Fact]
        public void Test1()
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
    }
}