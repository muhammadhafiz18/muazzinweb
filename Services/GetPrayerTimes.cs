using Newtonsoft.Json;
using Muazzinweb.Models;

namespace Muazzinweb.Services;

public class GetPrayerTimes
{
    private static readonly HttpClient client = new();

    public async Task<PrayerTimes> GetPrayerTimesAsync(string latitude, string longitude, int method = 4, int school
    = 1)
    {
        var url =
        $"https://api.aladhan.com/v1/timings?latitude={latitude}&longitude={longitude}&method={method}&school={school}";

        var response = await client.GetStringAsync(url);

        var jsonResponse = JsonConvert.DeserializeObject<ApiResponse>(response);

        if (jsonResponse != null && jsonResponse.Data != null)
        {
            var timings = jsonResponse.Data.Timings;

            return new PrayerTimes
            {
                Location = jsonResponse?.Data?.Meta?.Timezone,
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
