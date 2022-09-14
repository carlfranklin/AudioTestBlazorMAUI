#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace AudioTestBlazorMAUI
{
    public static class Globals
    {
        public static readonly int WindowWidth = 900;
        public static readonly int WindowHeight = 500;
#if WINDOWS
        public static Microsoft.UI.Windowing.AppWindow AppWindow {get;set;}
#endif
    }
}
