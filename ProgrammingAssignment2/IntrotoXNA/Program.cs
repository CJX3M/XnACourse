using System;

namespace IntrotoXNA
{
#if WINDOWS || LINUX
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
            using (var game = new ProgrammingAssignment2.Game1())
                game.Run();
        }
    }
#endif
}
