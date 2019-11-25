using System;

namespace DevilMakujin
{
    /// <summary>
    /// The main class.
    /// </summary>
    static public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var game = new DevimakuGame())
                game.Run();
        }
    }
}
