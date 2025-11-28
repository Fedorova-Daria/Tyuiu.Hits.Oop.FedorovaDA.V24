using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data;
using NewsAggregator.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace NewsAggregator.Services
{ 
    public class NewsBackgroundService : BackgroundService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        // Событие, чтобы уведомить интерфейс о новых новостях
        public event Action? OnNewsUpdated;

        public NewsBackgroundService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchNewsAsync();
                // Ждем 10 минут перед следующим обновлением
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task FetchNewsAsync()
        {
            using var context = _dbFactory.CreateDbContext();
            // Загружаем источники и сразу все существующие категории в память (для кэша)
            var sources = await context.Sources.ToListAsync();
            var dbCategories = await context.Categories.ToListAsync();

            foreach (var source in sources)
            {
                try
                {
                    using var reader = XmlReader.Create(source.RssUrl);
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        if (await context.Articles.AnyAsync(a => a.Link == item.Links[0].Uri.ToString()))
                            continue;

                        // 1. Пытаемся вытащить название категории из RSS
                        // Обычно оно лежит в item.Categories[0].Name
                        string catName = "Общее"; // Дефолтная категория

                        // 1. Пробуем стандартный ategory>
                        if (item.Categories.Count > 0)
                        {
                            catName = item.Categories.First().Name;
                        }
                        // 2. Если пусто, пробуем вытащить из <dc:subject> (некоторые RSS прячут темы там)
                        else if (item.ElementExtensions.Any(e => e.OuterName == "subject"))
                        {
                            // Это сложнее парсить, пропустим для простоты, 
                            // но можно просто присвоить имя источника как категорию
                            catName = source.Name;
                        }

                        // Если категория пустая или null — ставим заглушку
                        if (string.IsNullOrWhiteSpace(catName)) catName = "Без рубрики";

                        // 2. Ищем такую категорию в базе или создаем новую
                        var category = dbCategories.FirstOrDefault(c => c.Name == catName);
                        if (category == null)
                        {
                            category = new Category { Name = catName, IsInteresting = false };
                            context.Categories.Add(category);
                            await context.SaveChangesAsync(); // Сохраняем сразу, чтобы получить ID
                            dbCategories.Add(category); // Обновляем локальный кэш
                        }
                        var pubDate = item.PublishDate.DateTime != DateTime.MinValue
    ? item.PublishDate.DateTime
    : (item.LastUpdatedTime.DateTime != DateTime.MinValue ? item.LastUpdatedTime.DateTime : DateTime.UtcNow);

                        var article = new Article
                        {
                            Title = item.Title?.Text ?? "Без заголовка",
                            Description = item.Summary?.Text ?? (item.Content as TextSyndicationContent)?.Text ?? "...",
                            Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? source.RssUrl,
                            PublishedDate = pubDate.ToUniversalTime(),
                            SourceId = source.Id,
                            CategoryId = category.Id
                        };

                        context.Articles.Add(article);
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка RSS {source.Name}: {ex.Message}");
                }
            }
            await context.SaveChangesAsync();

            // Уведомляем подписчиков (наш UI), что новости обновились
            OnNewsUpdated?.Invoke();
        }
    }
}