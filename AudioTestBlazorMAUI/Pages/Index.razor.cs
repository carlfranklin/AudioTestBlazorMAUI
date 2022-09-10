using Microsoft.AspNetCore.Components;

namespace AudioTestBlazorMAUI.Pages
{
    public partial class Index : ComponentBase
    {
        protected IAudioPlayer player = null;
        protected FileStream stream = null;
        protected string url = "";
        protected string AudioMessage = "";
        protected string ProgressPercent = "";
        protected string PlayPosition = "";
        protected string ControlsOpacity = ".5";
        protected string PlayOpacity = "1";
        protected double Percentage = 0;
        private bool playing = false;
        private bool paused = false;

        private System.Timers.Timer timer = null;

        [Inject] private IAudioManager _audioManager { get; set; }

        protected async Task PlayAudio()
        {
            if (paused == true && player != null)
            {
                player.Play();
                playing = true;
                paused = false;
                ControlsOpacity = "1";
                PlayOpacity = ".5";
                AudioMessage = "Playing";
                return;
            }

            if (string.IsNullOrEmpty(url)) return;
            
            try
            {
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
                paused = false;
                ControlsOpacity = "1";
                PlayOpacity = ".5";
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                AudioMessage = "Please enter a valid URL to an MP3 file";
            }
        }

        protected void Forward()
        {
            if (ControlsOpacity == "1")
            {
                var pos = player.CurrentPosition + 10;
                if (pos < player.Duration)
                {
                    player.Seek(pos);
                }
            }
        }

        protected void StopAudio()
        {
            if (ControlsOpacity == "1")
            {
                player.Stop();
                playing = false;
                paused = false;
                Percentage = 0;
                ControlsOpacity = ".5";
                PlayOpacity = "1";
            }
        }

        protected void PauseAudio()
        {
            if (ControlsOpacity == "1")
            {
                player.Pause();
                paused = true;
                playing = false;
                ControlsOpacity = ".5";
                PlayOpacity = "1";
                AudioMessage = "Paused";
            }
        }

        protected void Rewind()
        {
            if (ControlsOpacity == "1")
            {
                var pos = player.CurrentPosition - 10;
                if (pos < 0)
                    pos = 0;
                player.Seek(pos);
            }
        }

        protected async void Player_PlaybackEnded(object sender, EventArgs e)
        {
            playing = false;
            paused = false;
            Percentage = 0;
            ControlsOpacity = ".5";
            PlayOpacity = "1";
            AudioMessage = "";
            ProgressPercent = "";
            PlayPosition = "";

            stream.Dispose();
            timer.Stop();
            timer.Dispose();
            player.PlaybackEnded -= Player_PlaybackEnded;
            await InvokeAsync(StateHasChanged);
        }
    }
}
