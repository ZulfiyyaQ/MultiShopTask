using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.DAL;
using MULTİSHOPMVC.Models;

namespace MULTİSHOPMVC.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;
        }

        public List<Category> GetCategories()
        {
            List<Category> getCategories = _context.Categories.ToList();
            return getCategories;
        }
    }
}
