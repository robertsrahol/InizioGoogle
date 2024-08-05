using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections;


namespace InizioGoogle.Controllers
{
    public class HomeController : Controller
    {
        //Když uživatel navštíví koøenovou URL aplikace, zobrazí se výchozí stránka

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Results(string query)
        {
            //Zavolá asynchronní metodu GetGoogleSearchResults pro získání výsledkù vyhledávání na Google
            var results = await GetGoogleSearchResults(query);

            // Uložení výsledkù do souboru
            await SaveResultsToFile(results);

            //Vrací view s výsledky vyhledávání
            return View(results);
        }

        //Tato metoda provede HTTP GET požadavek na Google vyhledávání s dotazem query.
        private async Task<List<string>> GetGoogleSearchResults(string query)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://www.google.com/search?q={query}");

            // Pomocí regulárního výrazu Regex extrahuje výsledky vyhledávání z HTML odpovìdi
            var matches = Regex.Matches(response, @"<div class=""BNeawe vvjwJb AP7Wnd"">(.+?)<\/div>");

            var results = matches.Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            return results;
        }

        //Tato metoda uloží výsledky vyhledávání do JSON souboru
        private async Task SaveResultsToFile(List<string> results)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "results.json");
            ViewBag.FilePath = filePath;

            try
            {
                var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
                {
                    WriteIndented = true // Volitelnì formátování pro lepší èitelnost
                });

                await System.IO.File.WriteAllTextAsync(filePath, json);
                Console.WriteLine($"Results saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
            }
        }
    }
}

