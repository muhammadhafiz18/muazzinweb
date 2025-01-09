using Newtonsoft.Json;
using Muazzinweb.Models;

namespace Muazzinweb.Services;

public class GetPrayerTimes
{
    private static readonly HttpClient client = new();

    public async Task<PrayerTimes> GetPrayerTimesAsync(string date, string city, string country, int method = 4, int school
    = 1)
    {
        var url =
        $"https://api.aladhan.com/v1/timingsByCity/{date}?city={city}&country={country}&method={method}&school={school}";

        var response = await client.GetStringAsync(url);

        var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(response);

        if (jsonResponse != null && jsonResponse.Data != null)
        {
            var timings = jsonResponse.Data.Timings;

            return new PrayerTimes
            {
                Fajr = timings!.Fajr,
                Sunrise = timings.Sunrise,
                Dhuhr = timings.Dhuhr,
                Asr = timings.Asr,
                Maghrib = timings.Maghrib,
                Isha = timings.Isha
            };
        }

        return null!;
    }
}
