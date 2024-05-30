namespace WeatherBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TelegramBotController telegramBotController = new TelegramBotController();
            telegramBotController.Start();
            Console.ReadKey();

        }
    }
}