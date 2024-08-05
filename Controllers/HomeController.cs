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
        //Kdy� u�ivatel nav�t�v� ko�enovou URL aplikace, zobraz� se v�choz� str�nka

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Results(string query)
        {
            //Zavol� asynchronn� metodu GetGoogleSearchResults pro z�sk�n� v�sledk� vyhled�v�n� na Google
            var results = await GetGoogleSearchResults(query);

            // Ulo�en� v�sledk� do souboru
            await SaveResultsToFile(results);

            //Vrac� view s v�sledky vyhled�v�n�
            return View(results);
        }

        //Tato metoda provede HTTP GET po�adavek na Google vyhled�v�n� s dotazem query.
        private async Task<List<string>> GetGoogleSearchResults(string query)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://www.google.com/search?q={query}");

            // Pomoc� regul�rn�ho v�razu Regex extrahuje v�sledky vyhled�v�n� z HTML odpov�di
            var matches = Regex.Matches(response, @"<div class=""BNeawe vvjwJb AP7Wnd"">(.+?)<\/div>");

            var results = matches.Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            return results;
        }

        //Tato metoda ulo�� v�sledky vyhled�v�n� do JSON souboru
        private async Task SaveResultsToFile(List<string> results)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "results.json");
            ViewBag.FilePath = filePath;

            try
            {
                var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
                {
                    WriteIndented = true // Voliteln� form�tov�n� pro lep�� �itelnost
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

