using Muazzinweb.Services;
using Muazzinweb.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Muazzinweb.Pages;

public partial class Muazzin
{
    public int Spacing { get; set; } = 6;
    public string CurrentWeekday { get; set; } = DateTime.Now.ToString("dddd");
    public string CurrentDayYear { get; set; } = DateTime.Now.ToString("MMMM dd, yyyy");
    public string CurrentTime { get; set; } = DateTime.Now.ToString("HH:mm:ss");
    public string TimeLeft { get; set; } = "";
    public string NextPrayerTitle { get; set; } = "";
    public List<CardItem> Items { get; set; } = [];
    public string Latitude { get; set; } = "";
    public string Longitude { get; set; } = "";
    public string Location { get; set; } = "";
    [Inject] public IJSRuntime? JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var userLocation = await JSRuntime!.InvokeAsync<GeolocationCoordinates>("getLocation");
        Latitude = userLocation.Latitude.ToString();
        Longitude = userLocation.Longitude.ToString();
     
        var prayerTimes = await new GetPrayerTimes().GetPrayerTimesAsync(Latitude, Longitude);

        if (prayerTimes != null)
        {
            Items =
            [
            new() { Title = "Fajr", Color = "black", ImageUrl =
            "https://static-00.iconduck.com/assets.00/cityscape-at-dusk-emoji-2048x2048-5ex19m4v.png", Time = prayerTimes.Fajr },
            new() { Title = "Sunrise", Color = "black", ImageUrl =
            "https://png.pngtree.com/element_our/sm/20180530/sm_ae642a0e81203376c4b08f38a31b40f9.jpg", Time = prayerTimes.Sunrise },
            new() { Title = "Dhuhr", Color = "black", ImageUrl =
            "https://png.pngtree.com/thumb_back/fh260/background/20241127/pngtree-beautiful-frozen-dawn-snowy-trees-and-grass-framing-a-mountain-horizon-image_16693033.jpg",
            Time = prayerTimes.Dhuhr },
            new() { Title = "Asr", Color = "black", ImageUrl =
            "https://symbl-world.akamaized.net/i/webp/fb/f6d0becf984d5eecb3a7d4df69588d.webp", Time = prayerTimes.Asr },
            new() { Title = "Maghrib", Color = "black", ImageUrl =
            "https://img.freepik.com/premium-vector/abstract-red-dark-sunset-nature-background_104785-1382.jpg?semt=ais_hybrid",
            Time = prayerTimes.Maghrib },
            new() { Title = "Isha", Color = "black", ImageUrl =
            "https://static-00.iconduck.com/assets.00/night-with-stars-emoji-2048x2048-lgxcow2a.png", Time = prayerTimes.Isha }
            ];
        }

        Location = prayerTimes!.Location!;

        DateTime currentTime = DateTime.Now;

        // Create today's prayer times list
        var todayPrayerTimesList = new List<(DateTime time, string name)>
        {
            (DateTime.Parse(prayerTimes!.Fajr!), "Fajr"),
            (DateTime.Parse(prayerTimes!.Sunrise!), "Sunrise"),
            (DateTime.Parse(prayerTimes!.Dhuhr!), "Dhuhr"),
            (DateTime.Parse(prayerTimes!.Asr!), "Asr"),
            (DateTime.Parse(prayerTimes!.Maghrib!), "Maghrib"),
            (DateTime.Parse(prayerTimes!.Isha!), "Isha")
        };

        // Generate tomorrow's prayer times
        var tomorrow = DateTime.Today.AddDays(1);
        var tomorrowPrayerTimesList = new List<(DateTime time, string name)>
        {
            (DateTime.Parse(prayerTimes!.Fajr!).AddDays(1), "Fajr")
        };

        // Combine today's and tomorrow's prayer times
        var prayerTimesList = todayPrayerTimesList.Concat(tomorrowPrayerTimesList).ToList();

        // Find the next prayer
        var (time, name) = prayerTimesList.FirstOrDefault(prayer => prayer.time > currentTime);

        UpdateDateTime(time, name);

        StateHasChanged();

        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (sender, args) =>
        {
            InvokeAsync(() =>
            {
                UpdateDateTime(time, name);
                StateHasChanged();
            });
        };
        timer.Start();}

    private void UpdateDateTime(DateTime nextPrayerTime, string nextPrayerName)
        {
            DateTime currentTime = DateTime.Now;
            CurrentWeekday = DateTime.Now.ToString("dddd");
            CurrentDayYear = DateTime.Now.ToString("MMMM dd, yyyy");
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            if (nextPrayerTime != default)
            {
                TimeLeft = (nextPrayerTime - currentTime).ToString(@"hh\:mm\:ss");
                NextPrayerTitle = $"{nextPrayerName}";
            }
        }

    private string GetCardStyle(string color) => $"background-color: {color};";
}
