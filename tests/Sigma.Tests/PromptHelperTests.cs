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
            string expectedSystemMessage = """
                message1
                message1
                message1
                """;

            string expectedUserMessage = """
                123

                213123
                12321
                """;

            string expectedAssistantMessage = """
                daf
                adfd
                adsfa
                afa
                """;

            string input = $"""
                {expectedSystemMessage}

                user:

                {expectedUserMessage}
                assistant:

                {expectedAssistantMessage}
                """;

            var messages = PromptHelper.GetHistories(input);

            Assert.True(messages.Count == 3);
            Assert.Equal("system", messages[0].Role);
            Assert.Equal("user", messages[1].Role);
            Assert.Equal("assistant", messages[2].Role);
            Assert.Equal(expectedSystemMessage, messages[0].Message);
            Assert.Equal(expectedUserMessage, messages[1].Message);
            Assert.Equal(expectedAssistantMessage, messages[2].Message);
        }
    }
}