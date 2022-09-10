using Microsoft.AspNetCore.Components;

namespace AudioTestBlazorMAUI.Pages
{
    public partial class Index: ComponentBase
    {
        protected IAudioPlayer player = null;
        protected FileStream stream = null;
        protected string url = "";
        protected string AudioMessage = "";
        protected string ProgressPercent = "";
        protected string PlayPosition = "";
        protected double Percentage = 0;
        private bool playing = false;

        private System.Timers.Timer timer = null;

        [Inject] private IAudioManager _audioManager { get; set; }

        protected async Task IncrementCount()
        {
            if (string.IsNullOrEmpty(url)) return;

            string cacheDir = FileSystem.Current.CacheDirectory;

            var localFile = $"{cacheDir}\\{Path.GetFileName(url)}"; ;

            // download if need be
            if (!File.Exists(localFile))
            {
                AudioMessage = "Downloading...";
                await InvokeAsync(StateHasChanged);

                using (var client = new HttpClient())
                {
                    var uri = new Uri(url);
                    await client.DownloadFileTaskAsync(uri, localFile);
                }
            }
            
            AudioMessage = "Playing";
            await InvokeAsync(StateHasChanged);

            stream = File.OpenRead(localFile);
            player = _audioManager.CreatePlayer(stream);
            player.PlaybackEnded += Player_PlaybackEnded;
            timer = new System.Timers.Timer(50);
            timer.Elapsed += async (state, args) =>
            {
                Percentage = (player.CurrentPosition * 100) / player.Duration;
                ProgressPercent = Percentage.ToString("N2") + "%";
                var tsCurrent = TimeSpan.FromSeconds(player.CurrentPosition);
                var tsTotal = TimeSpan.FromSeconds(player.Duration);
                var durationString = $"{tsTotal.Minutes.ToString("D2")}:{tsTotal.Seconds.ToString("D2")}";
                var currentString = $"{tsCurrent.Minutes.ToString("D2")}:{tsCurrent.Seconds.ToString("D2")}";
                PlayPosition = $"{currentString} / {durationString}";
                await InvokeAsync(StateHasChanged);
            };
            timer.Start();
            player.Play();
            playing = true;
            await InvokeAsync(StateHasChanged);
        }

        protected bool playdisabled
        {
            get
            {
                return playing;
            }
        }

        protected async void Player_PlaybackEnded(object sender, EventArgs e)
        {
            playing = false;
            stream.Dispose();
            player.PlaybackEnded -= Player_PlaybackEnded;
            AudioMessage = "";
            ProgressPercent = "";
            PlayPosition = "";
            timer.Stop();
            timer.Dispose();
            await InvokeAsync(StateHasChanged);
        }
    }
}
