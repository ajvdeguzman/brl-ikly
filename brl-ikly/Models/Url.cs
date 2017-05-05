using System.Data.Entity;

namespace brl_ikly.Models
{
    public class Url
    {
        public int UrlId { get; set; }
        public string UrlLongName { get; set; }
        public string UrlShortName { get; set; }
        public int UrlVisitCount { get; set; }
    }

    public class UrlDBContext : DbContext
    {
        public DbSet<Url> Urls { get; set; }
    }
}