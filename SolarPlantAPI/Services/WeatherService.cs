using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarPlantAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> GetCloudCoverAsync(double latitude, double longitude)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=cloudcover";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                var json = await JsonDocument.ParseAsync(stream);

                var cloudCoverArray = json.RootElement
                    .GetProperty("hourly")
                    .GetProperty("cloudcover")
                    .EnumerateArray();

                double firstCloudCover = cloudCoverArray.FirstOrDefault().GetDouble();

                return firstCloudCover;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Weather API call failed: {ex.Message}");
                return 50.0;
            }
        }
    }
}