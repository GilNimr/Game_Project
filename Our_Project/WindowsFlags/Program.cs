
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using MonoGame.Shared1;
using System;

namespace WindowFlags
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MonoGame.Shared1.Game1(Platform.WINDOWS))
                game.Run();
        }
    }
}
