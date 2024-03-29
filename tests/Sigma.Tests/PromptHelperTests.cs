using Sigma.LLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Tests
{
    public class PromptHelperTests
    {
        [Fact]
        public void TestHistories()
        {
            string input = """
                system: message1,

                user: userMesssage assistant:34425
                okokok

                assistant: assistantMessage
                ```
                ddd
                ```

                """;

            var messages = PromptHelper.GetHistories(input);

            Assert.True(messages.Count == 3);
        }
    }
}