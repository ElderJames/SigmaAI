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

            var lines = Regex.Split(prompt, "\r\n|\r|\n");
            string currentRole = "system"; // 默认角色为system
            var currentMessage = new List<string>(); // 使用列表累积多行消息
            string newLine = Environment.NewLine; // 获取当前环境的换行符
            int index = 0; // 当前处理的行索引

            // 使用while循环处理每一行，直到处理完所有行
            while (index <= lines.Length)
            {
                string? line = index < lines.Length ? lines[index] : null;
                var match = line != null ? Regex.Match(line, @"^\s*(system|assistant|user):") : null;

                if (match != null && match.Success)
                {
                    // 当遇到新角色时，先保存之前的消息（如果有）
                    if (currentMessage.Count > 0)
                    {
                        messages.Add(new(currentRole, string.Join(newLine, currentMessage).Trim()));
                        currentMessage.Clear(); // 清空当前消息列表，以便开始新的消息
                    }
                    currentRole = match.Groups[1].Value; // 更新当前角色
                    currentMessage.Add(line.Substring(match.Length)); // 添加去除角色标识的当前行到消息中
                }
                else if (line != null)
                {
                    // 如果当前行不是新角色，则继续累积到当前消息
                    currentMessage.Add(line);
                }

                // 如果已处理完所有行，保存最后一条消息
                if (index == lines.Length && currentMessage.Count > 0)
                {
                    messages.Add(new(currentRole, string.Join(newLine, currentMessage).Trim()));
                }

                index++; // 移动到下一行
            }

            return messages;
        }
    }
}