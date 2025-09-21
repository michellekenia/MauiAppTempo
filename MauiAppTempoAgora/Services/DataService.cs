using System.Text.Json.Nodes;
using MauiAppTempoAgora.Models;

namespace MauiAppTempoAgora.Services
{
    public static class DataService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                throw new InvalidOperationException("Sem conexão com a internet. Verifique sua conexão e tente novamente.");

            string appId = "de4ae5df8be81fde050f65314a21b0ee"; 

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(cidade)}&appid={appId}&units=metric&lang=pt_br";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new KeyNotFoundException($"Cidade '{cidade}' não encontrada. Verifique o nome e tente novamente.");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Chave da API inválida. Verifique sua chave.");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro ao buscar dados: {response.StatusCode}");

            string json = await response.Content.ReadAsStringAsync();
            JsonNode? jsonObj = JsonNode.Parse(json);
            if (jsonObj == null) return null;

            var tempo = new Tempo
            {
                lat = (double?)jsonObj["coord"]?["lat"],
                lon = (double?)jsonObj["coord"]?["lon"],
                temp_min = (double?)jsonObj["main"]?["temp_min"],
                temp_max = (double?)jsonObj["main"]?["temp_max"],
                speed = (double?)jsonObj["wind"]?["speed"],
                visibility = (int?)jsonObj["visibility"],
                main = (string?)jsonObj["weather"]?[0]?["main"],
                description = (string?)jsonObj["weather"]?[0]?["description"]
            };

            long? sunrise = (long?)jsonObj["sys"]?["sunrise"];
            long? sunset = (long?)jsonObj["sys"]?["sunset"];
            tempo.sunrise = sunrise.HasValue ? DateTimeOffset.FromUnixTimeSeconds(sunrise.Value).ToLocalTime().ToString("HH:mm") : "";
            tempo.sunset = sunset.HasValue ? DateTimeOffset.FromUnixTimeSeconds(sunset.Value).ToLocalTime().ToString("HH:mm") : "";

            if (tempo.lat == null || tempo.lon == null || tempo.temp_min == null || tempo.temp_max == null)
                throw new Exception("Não foi possível obter dados de previsão.");

            return tempo;
        }
    }
}
