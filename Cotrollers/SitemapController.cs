using Microsoft.AspNetCore.Mvc;
using soly.Pages;
using System.ServiceModel.Syndication;
using System.Xml;

namespace soly.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IndexModel _indexModel;

        public SitemapController()
        {
            _indexModel = new IndexModel();
            _indexModel.OnGet();
        }

        [Route("sitemap.xml")]
        public IActionResult Sitemap()
        {
            var baseUrl = "https://соль39.рф";
            var items = new List<SitemapItem>
            {
                new SitemapItem
                {
                    Url = baseUrl,
                    LastModified = DateTime.Now,
                    ChangeFrequency = SitemapChangeFrequency.Weekly,
                    Priority = 1.0
                }
            };

            foreach (var category in _indexModel.Categories)
            {
                items.Add(new SitemapItem
                {
                    Url = $"{baseUrl}/category/{category.Id}",
                    LastModified = DateTime.Now,
                    ChangeFrequency = SitemapChangeFrequency.Monthly,
                    Priority = 0.8
                });

                foreach (var product in category.Products)
                {
                    items.Add(new SitemapItem
                    {
                        Url = $"{baseUrl}/product/{product.Id}",
                        LastModified = DateTime.Now,
                        ChangeFrequency = SitemapChangeFrequency.Monthly,
                        Priority = 0.6
                    });
                }
            }

            var xml = GenerateSitemap(items);
            return Content(xml, "application/xml");
        }

        private string GenerateSitemap(List<SitemapItem> items)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                {
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("url");
                        writer.WriteElementString("loc", item.Url);
                        writer.WriteElementString("lastmod", item.LastModified.ToString("yyyy-MM-dd"));
                        writer.WriteElementString("changefreq", item.ChangeFrequency.ToString().ToLower());
                        writer.WriteElementString("priority", item.Priority.ToString("0.0"));
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    public class SitemapItem
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
        public SitemapChangeFrequency ChangeFrequency { get; set; }
        public double Priority { get; set; }
    }

    public enum SitemapChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}