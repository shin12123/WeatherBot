using RestSharp;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class TelegramBotController
{


    private readonly string _botToken = "7177886294:AAHclXFJNqF68c-BRmp4xmQsK6vRN1SPwBU";
    private readonly TelegramBotClient _botClient = new TelegramBotClient("7177886294:AAHclXFJNqF68c-BRmp4xmQsK6vRN1SPwBU");
    private CancellationToken CancellationToken = new CancellationToken();
    private ReceiverOptions ReceiverOptions = new ReceiverOptions { AllowedUpdates = { } };
    private readonly RestClient Client = new RestClient("https://localhost:7249");



    private static readonly List<string> Jokes = new List<string>
    {
       "Почему программисты не любят природу? Слишком много багов.",
        "Почему программисты предпочитают темные темы? Потому что свет притягивает баги!",
        "Почему программисты всегда путают Рождество и Хэллоуин? Потому что OCT 31 == DEC 25.",
        "Сколько программистов нужно, чтобы поменять лампочку? Ни одного, это аппаратная проблема.",
        "Почему программисты боятся ездить на природе? Потому что они боятся 'пикников' (в программировании, 'picnic' - проблема между компьютером и стулом).",
        "Программистам сложно завести друзей: недостаточно памяти для новых подключений.",
        "Почему программисты не могут удержать секрет? Потому что они всегда оставляют следы (лог-файлы).",
        "Почему программисты не могут лазить по горам? Потому что они постоянно 'заблудились' (exception handling).",
        "Почему программисты не ходят в кино? Потому что они уже видели трейлер (preview).",
        "Что делает программист, когда он замерзает? Он просто закручивает 'винты' (loops).",
        "Программисту на свадьбу подарили книгу, но он не мог её открыть, потому что не знал пароля.",
        "Программисты не лгут, они просто создают альтернативные факты (if-else statements).",
        "Почему программисты не любят кошек? Потому что они не могут отменить изменения.",
        "Почему программистам не нравится готовить? Потому что они не могут обработать рецепт (recipe handler).",
        "Почему программисты не могут отличить левую от правой стороны? Потому что они используют 'битовые' операции.",
        "Почему программисты всегда одиноки? Потому что у них 'исключительные' случаи.",
        "Почему программисты не играют в футбол? Потому что они боятся попасть в 'тупик' (deadlock).",
        "Почему программисты любят осень? Потому что она 'отладочная' (debugging season).",
        "Почему программисты не читают книги? Потому что они не могут найти 'конечную строку' (end of file).",
        "Почему программисты не пьют кофе? Потому что это вызывает 'переполнение стека' (stack overflow).",
        "Почему программисты не ездят на велосипедах? Потому что они боятся 'сегментных ошибок' (segmentation fault).",
        "Почему программисты не играют на пианино? Потому что у них проблемы с 'клавишами' (keys).",
        "Почему программисты не могут танцевать? Потому что они не могут синхронизировать 'потоки' (threads).",
        "Почему программисты не любят лето? Потому что это время 'сезонных багов'.",
        "Почему программисты не могут найти любовь? Потому что они ищут 'идеальный код'.",
        "Почему программисты всегда носят очки? Потому что они не могут найти 'окно' (window).",
        "Почему программисты не летают на самолетах? Потому что они боятся 'разрыва соединения' (disconnect).",
        "Почему программисты не играют в шахматы? Потому что они боятся 'мат' (checkmate).",
        "Почему программисты не любят плавать? Потому что они боятся 'плавучих точек' (floating points).",
        "Почему программисты не ездят на машине? Потому что они боятся 'аварийных ситуаций' (crash)."
        //мб еще..
    };
    public async Task Start()
    {
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, ReceiverOptions, CancellationToken);
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Bot {me.Username} started.");

        //bot st
        await Task.Delay(-1, CancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update?.Message?.Text != null)
        {
            await HandleMessageAsync(client, update.Message);
        }
    }

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        var messageText = message.Text.ToLower();
        var chatId = message.Chat.Id;

        try
        {
            if (messageText.StartsWith("/w"))
            {
                var city = messageText.Substring("/w".Length).Trim();
                if (!string.IsNullOrEmpty(city))
                {
                    var weatherData = await GetWeatherForCity(city);
                    await _botClient.SendTextMessageAsync(chatId, weatherData, parseMode: ParseMode.Html);
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "Будь ласка, вкажіть назву міста після команди /w 🌆.");
                }
            }
            else if (messageText.StartsWith("/forecast"))
            {
                var city = messageText.Substring("/forecast".Length).Trim();
                if (!string.IsNullOrEmpty(city))
                {
                    var forecastData = await GetWeatherForecast(city);
                    await _botClient.SendTextMessageAsync(chatId, forecastData, parseMode: ParseMode.Html);
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "Будь ласка, вкажіть назву міста після команди /forecast 🌆.");
                }
            }
            else if (messageText.Equals("/start"))
            {
                await _botClient.SendTextMessageAsync(chatId, "<b>🌥️ Ласкаво просимо до Weather Bot! 🌦️</b>\nВикористовуйте ➡️ <i>/w [місто]</i> ⬅️, щоб отримувати прогнози погоди, або команду ➡️ <i>/help</i> ⬅️, щоб дізнатися про наші команди! Завжди готовий допомогти 🏝️⛱️.", parseMode: ParseMode.Html);
            }
            else if (messageText.Equals("/help"))
            {
                await _botClient.SendTextMessageAsync(chatId, "<b>📋 Список наших команд:</b>\n/start - Почати спілкування з ботом.▶️\n\n/help - Дізнатися про наші команди.❔\n\n/w [місто] - Отримати прогноз погоди.🌄🌁\n\n/forecast [місто] - Отримати прогноз погоди на кілька днів.🖼️📡\n\n/addfavorite [місто] - Додати місто до списку обраних.➕📝\n\n/removefavorite [місто] - Видалити місто зі списку обраних.❌📝\n\n/listfavorites - Переглянути список обраних міст.🌟📝", parseMode: ParseMode.Html);
            }
            else if (messageText.StartsWith("/addfavorite"))
            {
                var city = messageText.Substring("/addfavorite".Length).Trim();
                if (!string.IsNullOrEmpty(city))
                {
                    AddFavorite(chatId, city);
                    await _botClient.SendTextMessageAsync(chatId, $"Місто {city} додано до списку обраних 🌟.");
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "Будь ласка, вкажіть назву міста після команди /addfavorite 🌆.");
                }
            }
            else if (messageText.StartsWith("/removefavorite"))
            {
                var city = messageText.Substring("/removefavorite".Length).Trim();
                if (!string.IsNullOrEmpty(city))
                {
                 RemoveFavorite(chatId, city);
                    await _botClient.SendTextMessageAsync(chatId, $"Місто {city} видалено зі списку обраних ❌.");
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "Будь ласка, вкажіть назву міста після команди /removefavorite 🌆.");
                }
            }
            else if (messageText.StartsWith("/listfavorites"))
            {
               
                await _botClient.SendTextMessageAsync(chatId, await ListFavorites(chatId), parseMode: ParseMode.Html);
            }
            else if (messageText.Equals("/joke"))
            {
                await SendRandomJoke(chatId);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Невідома команда 😶‍🌫️.\nВикористовуйте /start для початку або /w [місто] для отримання прогнозів погоди 🌩️❄️☀️.");
            }
        }
        catch (Exception ex)
        {
            await _botClient.SendTextMessageAsync(chatId, "Неправильно введені дані 😔. Спробуйте ще раз!");

        }
    }

    private async Task <string> ListFavorites(long chatId)
    {
        var request = new RestRequest($"/ApiController/GETWeatherForFavorites?chatid={chatId}", Method.Get);
        var response = Client.Execute(request, Method.Get).Content.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\"", "");
        return DecodeUnicodeCharacters(response);
    }

    private async Task SendRandomJoke(long chatId)
    {
        await _botClient.SendTextMessageAsync(chatId, GetRandomJoke() );
        return;
    }

    private async Task AddFavorite(long chatId, string city)
    {
        var request = new RestRequest($"/FavoriteCitiesController/ADDfavorite?chatid={chatId}&city={city}", Method.Put);
        Client.Execute(request, Method.Put);
        return;
    }

    private async Task RemoveFavorite(long chatId, string city)
    {
        var request = new RestRequest($"/FavoriteCitiesController/REMOVEfavorite?chatid={chatId}&city={city}", Method.Delete);
        Client.Execute(request, Method.Delete);
        return;
    }

    public string GetRandomJoke()
    {

        return Jokes[new Random().Next(Jokes.Count)];
    }

    private async Task<string> GetWeatherForCity(string city)
    {
        var request = new RestRequest($"/ApiController/GETWeatherForCity?city={city}", Method.Get);
        var response = Client.Execute(request, Method.Get).Content.Replace("\"", "");
        return DecodeUnicodeCharacters(response);
    }

    private async Task<string> GetWeatherForecast(string city)
    {
        var request = new RestRequest($"/ApiController/GETWeatherForecast?city={city}", Method.Get);
        var response = Client.Execute(request, Method.Get).Content.Replace("\"", "");

        return DecodeUnicodeCharacters(response);
    }
    private string DecodeUnicodeCharacters(string input)
    {
        return Regex.Unescape(input);
    }

}
