﻿using Microsoft.EntityFrameworkCore;
using Sigma.Core.Repositories.Base;
using Sigma.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.Domain.Chat
{
    public interface IChatRepository : IRepository<Chat>
    {
        public Task<List<ChatHistory>> GetChatHistories(string chatId, int offset, int take);

        public Task<bool> SaveHistory(ChatHistory history);

        public Task<bool> ClearHistory(string chatId);
    }

    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        private DbContext _db;

        public ChatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> SaveHistory(ChatHistory history)
        {
            if (history.Id == null)
            {
                history.Id = Guid.NewGuid().ToString();
                _db.Add(history);
            }
            else
            {
                _db.Update(history);
            }
          
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatHistory>> GetChatHistories(string chatId, int offset, int take)
        {
            return await _db.Set<ChatHistory>().AsNoTracking()
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(offset)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> ClearHistory(string chatId)
        {
           await _db.Set<ChatHistory>().Where(x => x.ChatId == chatId).ExecuteDeleteAsync();
            return true;
        }
    }
}