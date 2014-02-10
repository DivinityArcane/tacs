using System;

namespace tacs
{
    class ConIO
    {
        private static readonly object ColorLock = new object();
        private const ConsoleColor TextColor = ConsoleColor.White;
        private const ConsoleColor TimeColor = ConsoleColor.Green;

        public static void Write (string msg)
        {
            lock (ColorLock)
            {
                var dt = DateTime.Now;
                Console.ForegroundColor = TimeColor;
                Console.Write("[{0}:{1}:{2}] ", 
                    dt.Hour.ToString().PadLeft(2, '0'),
                    dt.Minute.ToString().PadLeft(2, '0'),
                    dt.Second.ToString().PadLeft(2, '0'));
                Console.ForegroundColor = TextColor;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}
