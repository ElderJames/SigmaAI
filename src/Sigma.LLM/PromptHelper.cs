using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sigma.LLM
{
    public static class PromptHelper
    {
        public static List<(string Role, string Message)> GetHistories(string prompt)
        {
            List<(string, string)> messages = [];

            // 修改正则表达式以确保role一定是一行的开头
            var matches = Regex.Matches(prompt, @"^(system|assistant|user):(.*?)(?=(\n(system|assistant|user):)|$)", RegexOptions.Multiline | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    string role = match.Groups[1].Value.Trim();
                    string message = match.Groups[2].Value.Trim();

                    messages.Add((role, message));
                }
            }

            return messages;
        }
    }
}