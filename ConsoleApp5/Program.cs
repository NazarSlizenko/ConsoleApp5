using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using System.Threading;
using Telegram.Bot.Types.Enums;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace Telegram_Bot
{
    class Program
    {
        private static string Token { get; set; } = "6993650528:AAFI6hc2v0kWS3BhpTUym8XDuhOu7QY7UOI";
        private static TelegramBotClient client;
        private static EventHandler<MessageEventArgs> _handler;

        static void Main(string[] args)
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            client.OnCallbackQuery += BotOnCallbackQueryReceived;
            Console.WriteLine("Бот запущен. Press any key to exit...");
            Console.ReadLine();
            client.StopReceiving();
        }
        public static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg.Text != null)
            {
                Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");//  на консольку выводится
                var replyMarkup1 = new ReplyKeyboardRemove();//пустая клавиатура
                switch (msg.Text)// нажатие кнопок в главном меню
                {
                    case "/start":
                        await client.SendTextMessageAsync(
                            chatId: msg.Chat.Id,
                            text: "Выберите категорию:",
                            replyMarkup: GetButtons());

                        break;

                    case "Книга":
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - Книга" + " : \n" +
                            "Фамилия и инициалы авторов:\n" +
                            "<i>Пример: Марцулевич.А.Ю.</i>\n" +
                            "Название книги:\n" +
                            "<i>100 совет начинающему водителю</i>\n" +
                            "Номер издания:\n" +
                            "<i>1-е</i>\n" +
                            "Город издательства:\n" +
                            "<i>Гродно</i>\n" +
                            "Название издательства\n" +
                            "<i>Просвещение</i>\n" +
                            "Год издания\n" +
                            "<i>2024</i>\n" +
                            "Количество страниц:\n" +
                            "<i>228</i>\n", replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Марцулевич А М. 100 советов начинающему водителю. - 1-е изд. - Гродно: Просвещение, 2024. - 228 с.", replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал книга: {msg.Text}");//консолька
                        
                        //await client.SendTextMessageAsync(msg.Chat.Id, "Введите фамилию и инициалы автора", replyMarkup: GetInlineButtons());// Замена меню
                        await client.SendTextMessageAsync(
                       chatId: msg.Chat.Id,
                       text: "Введите фамилию автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var authorLastName = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название книги:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var bookTitle = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите номер издания:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        if (int.TryParse(args2.Message.Text, out int editionNumber))
                                        {
                                            await client.SendTextMessageAsync(
                                                chatId: args2.Message.Chat.Id,
                                                text: "Введите город издательства:");

                                            client.OnMessage -= _handler;
                                            _handler = async (s3, args3) =>
                                            {
                                                var city = args3.Message.Text;

                                                await client.SendTextMessageAsync(
                                                    chatId: args3.Message.Chat.Id,
                                                    text: "Введите название издательства:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s4, args4) =>
                                                {
                                                    var publisher = args4.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args4.Message.Chat.Id,
                                                        text: "Введите год издания:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s5, args5) =>
                                                    {
                                                        if (int.TryParse(args5.Message.Text, out int year))
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args5.Message.Chat.Id,
                                                                text: "Введите количество страниц:");

                                                            client.OnMessage -= _handler;
                                                            _handler = async (s6, args6) =>
                                                            {
                                                                if (int.TryParse(args6.Message.Text, out int pages))
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args6.Message.Chat.Id,
                                                                        text: $"{authorLastName} {bookTitle}.-{editionNumber}-е изд. - {city}: {publisher}. {year}.- {pages} c.", replyMarkup: GetInlineButtons());
                                                                }
                                                                else
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args6.Message.Chat.Id,
                                                                        text: "Введите корректное количество страниц:");
                                                                }
                                                            };

                                                            client.OnMessage += _handler;
                                                        }
                                                        else
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args5.Message.Chat.Id,
                                                                text: "Введите корректный год издания:");
                                                        }
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            };

                                            client.OnMessage += _handler;
                                        }
                                        else
                                        {
                                            await client.SendTextMessageAsync(
                                                chatId: args2.Message.Chat.Id,
                                                text: "Введите корректный номер издания:");
                                        }
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Интернет-ресурс"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - Интернет-ресурс" + " : \n" +
                            "Заголовок статьи или страницы:\n" +
                            "<i>Алкоголизм и его последовия</i>\n" +
                            "Название сайта:\n" +
                            "<i>Анонимные алкоголики</i>\n" +
                            "Гиперссылка:\n" +
                            "<i> https://aabelarus.org/</i>\n" +
                            "Дата обращения на сайт:\n" +
                            "<i>22.10.2023</i>\n"
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Алкоголизм и его последовательности // Анонимные алкоголики URL: https://aabelarus.org/ (дата обращения: 22.10.2023).", replyMarkup: replyMarkup1);

                        Console.WriteLine($"Пользователь выбрал книга: {msg.Text}");//консолька
                        //await client.SendTextMessageAsync(msg.Chat.Id, "Введите заголовок статьи или страницы", replyMarkup: GetInlineButtons());// Замена меню
                        await client.SendTextMessageAsync(
                       chatId: msg.Chat.Id,
                       text: "Введите заголовок статьи или страницы:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var articleTitle = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название сайта:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var websiteName = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите гиперссылку:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var hyperlink = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите дату обращения на сайт:");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            var date = args3.Message.Text;

                                            await client.SendTextMessageAsync(
                                                chatId: args3.Message.Chat.Id,
                                                text: $"{articleTitle} // "+
                                                      $"{websiteName} URL:" +
                                                      $"{hyperlink}" +
                                                      $" ( дата обращения: {date})", replyMarkup: GetInlineButtons());
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Законы, нормативный акт"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - Законы, нормативный акт" + " : \n" +
                            "Тип нормативного акта:\n" +
                            "<i>Конституция Республики Беларусь</i>\n" +
                            "Полное название нормативного акта:\n" +
                            "<i>Положение о порядке государственной регистрации научно-исследовательских, опытно-конструкторских и опытно-технологических работ</i>\n" +
                            "Дата принятия:\n" +
                            "<i> 25.05.2006</i>\n" +
                            "Номер нормативного акта:\n" +
                            "<i>1234-56</i>\n" +
                            "Официальный источник публикования:\n" +
                            "<i>Собрание законодательства РБ</i>\n"+
                            "Год публикации источника:\n" +
                            "<i> 2006</i>\n" +
                            "Номер выхода источника:\n" +
                            "<i>5</i>\n"+
                            "Номер статьи:\n" +
                            "<i> 15</i>\n" +
                            "В редакции от:\n" +
                            "<i>26.05.2006</i>\n"
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Конституции Республики Беларусь \"Положение о порядке государственной регистрации научно-исследовательских, опытно-конструкторских и опытно-технологических работ\" от 25.05.2006 № 1234-56 // Собрание законодательства РБ. - 2006 г. - № 5. - Ст. 16 с изм. и допол. в ред. от 26.05.2006.", replyMarkup: replyMarkup1);

                        Console.WriteLine($"Пользователь выбрал -  Законы, нормативный акт: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите тип нормативного акта:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var type = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите полное название нормативного акта:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var fullName = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите дату принятия:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var date = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите номер нормативного акта:");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            var number = args3.Message.Text;

                                            await client.SendTextMessageAsync(
                                                chatId: args3.Message.Chat.Id,
                                                text: "Введите официальный источник публикования:");

                                            client.OnMessage -= _handler;
                                            _handler = async (s4, args4) =>
                                            {
                                                var source = args4.Message.Text;

                                                await client.SendTextMessageAsync(
                                                    chatId: args4.Message.Chat.Id,
                                                    text: "Введите год публикации источника:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s5, args5) =>
                                                {
                                                    var publicationYear = args5.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args5.Message.Chat.Id,
                                                        text: "Введите номер выхода источника:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s6, args6) =>
                                                    {
                                                        var issueNumber = args6.Message.Text;

                                                        await client.SendTextMessageAsync(
                                                            chatId: args6.Message.Chat.Id,
                                                            text: "Введите номер статьи:");

                                                        client.OnMessage -= _handler;
                                                        _handler = async (s7, args7) =>
                                                        {
                                                            var articleNumber = args7.Message.Text;

                                                            await client.SendTextMessageAsync(
                                                                chatId: args7.Message.Chat.Id,
                                                                text: "Введите информацию о редакции:");

                                                            client.OnMessage -= _handler;
                                                            _handler = async (s8, args8) =>
                                                            {
                                                                var editorialInformation = args8.Message.Text;

                                                                await client.SendTextMessageAsync(
                                                                    chatId: args8.Message.Chat.Id,
                                                                    text: $"{type} \" " +"{fullName}\"" +
                                                                          $"от {date}" +
                                                                          $"№ {number} //" +
                                                                          $"{source}.- " +
                                                                          $"{publicationYear} г.- №" +
                                                                          $"{issueNumber}.-" +
                                                                          $"Ст. {articleNumber} с изм. и допол. в ред. от" +
                                                                          $"{editorialInformation}.",replyMarkup: GetInlineButtons());
                                                            };

                                                            client.OnMessage += _handler;
                                                        };

                                                        client.OnMessage += _handler;
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            };

                                            client.OnMessage += _handler;
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;
                        
                    case "Диссертация"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - диссертация" + " : \n" +
                            "Фамилия и инициалы автора:\n" +
                            "<i>Иванов И.М</i>\n" +
                            "Название диссертации:\n" +
                            "<i>Наука как искусство</i>\n" +
                            "Доктора или кандидата:\n" +
                            "<i> д-р. / канд.</i>\n" +
                            "Отрасль наук (сокращенно):\n" +
                            "<i>экон.</i>\n" +
                            "Код специальности:\n" +
                            "<i>01.01.01</i>\n" +
                            "Город издательства:\n" +
                            "<i>Гродно</i>\n" +
                            "Год:\n" +
                            "<i>2020</i>\n" +
                            "Количество страниц:\n" +
                            "<i> 150</i>\n"
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Иванов И.М Наука как искусство: дис. д-р. экон. наук: 01.01.01. - Гродно, 2020. - 150 с.\n" , replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал -  Диссертация: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите фамилию и инициалы автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var author = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название диссертации:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var title = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите степень (доктор или кандидат):");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var degree = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите отрасль наук (сокращенно):");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            var branch = args3.Message.Text;

                                            await client.SendTextMessageAsync(
                                                chatId: args3.Message.Chat.Id,
                                                text: "Введите код специальности:");

                                            client.OnMessage -= _handler;
                                            _handler = async (s4, args4) =>
                                            {
                                                var specialtyCode = args4.Message.Text;

                                                await client.SendTextMessageAsync(
                                                    chatId: args4.Message.Chat.Id,
                                                    text: "Введите город издательства:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s5, args5) =>
                                                {
                                                    var city = args5.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args5.Message.Chat.Id,
                                                        text: "Введите год:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s6, args6) =>
                                                    {
                                                        if (int.TryParse(args6.Message.Text, out int year))
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args6.Message.Chat.Id,
                                                                text: "Введите количество страниц:");

                                                            client.OnMessage -= _handler;
                                                            _handler = async (s7, args7) =>
                                                            {
                                                                if (int.TryParse(args7.Message.Text, out int pages))
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args7.Message.Chat.Id,
                                                                        text: $"{author}" +
                                                                              $"{title}:" +
                                                                              $"дис.{degree}" +
                                                                              $"{branch} наук:" +
                                                                              $"{specialtyCode}.-" +
                                                                              $"{city}." +
                                                                              $"{year}.- " +
                                                                              $"{pages} c.",replyMarkup: GetInlineButtons());
                                                                }
                                                                else
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args7.Message.Chat.Id,
                                                                        text: "Введите корректное количество страниц:");
                                                                }
                                                            };

                                                            client.OnMessage += _handler;
                                                        }
                                                        else
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args6.Message.Chat.Id,
                                                                text: "Введите корректный год:");
                                                        }
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            };

                                            client.OnMessage += _handler;
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Автореферат"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - диссертация" + " : \n" +
                            "Фамилия и инициалы автора:\n" +
                            "<i>Иванов И.М</i>\n" +
                            "Название автореферата:\n" +
                            "<i>Наука как искусство</i>\n" +
                            "Доктора или кандидата:\n" +
                            "<i> д-р. / канд.</i>\n" +
                            "Отрасль наук (сокращенно):\n" +
                            "<i>экон.</i>\n" +
                            "Код специальности:\n" +
                            "<i>01.01.01</i>\n" +
                            "Город издательства:\n" +
                            "<i>Гродно</i>\n" +
                            "Год:\n" +
                            "<i>2020</i>\n" +
                            "Количество страниц:\n" +
                            "<i> 150</i>\n"
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Иванов И.М Наука как искусство: автореф. дис. д-р. экон. наук: 01.01.01. - Гродно, 2020. - 150 с.\n", replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал -  Автореферат: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите фамилию и инициалы автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var author = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название автореферата:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var title = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите степень (доктор или кандидат):");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var degree = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите отрасль наук (сокращенно):");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            var branch = args3.Message.Text;

                                            await client.SendTextMessageAsync(
                                                chatId: args3.Message.Chat.Id,
                                                text: "Введите код специальности:");

                                            client.OnMessage -= _handler;
                                            _handler = async (s4, args4) =>
                                            {
                                                var specialtyCode = args4.Message.Text;

                                                await client.SendTextMessageAsync(
                                                    chatId: args4.Message.Chat.Id,
                                                    text: "Введите город издательства:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s5, args5) =>
                                                {
                                                    var city = args5.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args5.Message.Chat.Id,
                                                        text: "Введите год:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s6, args6) =>
                                                    {
                                                        if (int.TryParse(args6.Message.Text, out int year))
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args6.Message.Chat.Id,
                                                                text: "Введите количество страниц:");

                                                            client.OnMessage -= _handler;
                                                            _handler = async (s7, args7) =>
                                                            {
                                                                if (int.TryParse(args7.Message.Text, out int pages))
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args7.Message.Chat.Id,
                                                                        text: $"{author}" +
                                                                              $" {title}:" +
                                                                              $"автореф. дис. {degree}" +
                                                                              $" {branch} наук:" +
                                                                              $" {specialtyCode}-" +
                                                                              $"{city}." +
                                                                              $"{year}.-" +
                                                                              $" {pages} c.", replyMarkup: GetInlineButtons());
                                                                }
                                                                else
                                                                {
                                                                    await client.SendTextMessageAsync(
                                                                        chatId: args7.Message.Chat.Id,
                                                                        text: "Введите корректное количество страниц:");
                                                                }
                                                            };

                                                            client.OnMessage += _handler;
                                                        }
                                                        else
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args6.Message.Chat.Id,
                                                                text: "Введите корректный год:");
                                                        }
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            };

                                            client.OnMessage += _handler;
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Статья из журнала"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - диссертация" + " : \n" +
                            "Фамилия и инициалы автора:\n" +
                            "<i>Иванов И.М</i>\n" +
                            "Название статьи:\n" +
                            "<i>Наука как искусство</i>\n" +
                            "Название журнала:\n" +
                            "<i> Образование и наука</i>\n" +
                            "Год издания:\n" +
                            "<i>2020</i>\n" +
                            "Номер журнала:\n" +
                            "<i> 10</i>\n"+
                            "Страницы статьи в журнале" +
                            "<i> 25-30</i>\n" 
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Иванов И.М Наука как искусство // Образование и наука. - 2020. - №10. - С. 25-30.\n", replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал -  статья из журнала: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите фамилию и инициалы автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var author = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название статьи:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var articleTitle = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите название журнала:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var journalTitle = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите год издания:");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            if (int.TryParse(args3.Message.Text, out int year))
                                            {
                                                await client.SendTextMessageAsync(
                                                    chatId: args3.Message.Chat.Id,
                                                    text: "Введите номер журнала:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s4, args4) =>
                                                {
                                                    var journalNumber = args4.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args4.Message.Chat.Id,
                                                        text: "Введите страницы статьи в журнале:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s5, args5) =>
                                                    {
                                                        var pages = args5.Message.Text;

                                                        await client.SendTextMessageAsync(
                                                            chatId: args5.Message.Chat.Id,
                                                            text: $"{author}" +
                                                                  $" {articleTitle} //" +
                                                                  $" {journalTitle}.-" +
                                                                  $" {year}.-" +
                                                                  $" №{journalNumber}.-" +
                                                                  $" С. {pages}.",replyMarkup: GetInlineButtons());
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            }
                                            else
                                            {
                                                await client.SendTextMessageAsync(
                                                    chatId: args3.Message.Chat.Id,
                                                    text: "Введите корректный год издания:");
                                            }
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Статья из сборника"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - статьи из сборника" + " : \n" +
                            "Фамилия и инициалы автора:\n" +
                            "<i>Иванов И.М</i>\n" +
                            "Название статьи:\n" +
                            "<i>Наука как искусство</i>\n" +
                            "Название сборника:\n" +
                            "<i> Сборник научных трудов</i>\n" +
                            "Город издательства:\n" +
                            "<i>Гродно</i>\n" +
                            "Название издательства:\n" +
                            "<i>АСТ</i>\n" +
                            "Год издания:\n" +
                            "<i>2020</i>\n" +
                            "Страницы статьи в журнале" +
                            "<i> 25-30</i>\n"
                            , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Ивнанов И.М Наука как искусство // Сборник научных трудов. - Гродно: АСТ, 2020. - С. 25-30.\n", replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал -  статья из сборника: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите фамилию и инициалы автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var author = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название статьи:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var articleTitle = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите название сборника:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var anthologyTitle = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите город издательства:");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            var city = args3.Message.Text;

                                            await client.SendTextMessageAsync(
                                                chatId: args3.Message.Chat.Id,
                                                text: "Введите год издания:");

                                            client.OnMessage -= _handler;
                                            _handler = async (s4, args4) =>
                                            {
                                                if (int.TryParse(args4.Message.Text, out int year))
                                                {
                                                    await client.SendTextMessageAsync(
                                                        chatId: args4.Message.Chat.Id,
                                                        text: "Введите количество страниц в сборнике:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s5, args5) =>
                                                    {
                                                        if (int.TryParse(args5.Message.Text, out int pageCount))
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args5.Message.Chat.Id,
                                                                text: $"{author}" +
                                                                      $" {articleTitle} //" +
                                                                      $"{anthologyTitle}.-" +
                                                                      $" {city}:" +
                                                                      $"{year}.-" +
                                                                      $" C.{pageCount}.", replyMarkup: GetInlineButtons());
                                                        }
                                                        else
                                                        {
                                                            await client.SendTextMessageAsync(
                                                                chatId: args5.Message.Chat.Id,
                                                                text: "Введите корректное количество страниц:");
                                                        }
                                                    };

                                                    client.OnMessage += _handler;
                                                }
                                                else
                                                {
                                                    await client.SendTextMessageAsync(
                                                        chatId: args4.Message.Chat.Id,
                                                        text: "Введите корректный год издания:");
                                                }
                                            };

                                            client.OnMessage += _handler;
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;

                    case "Статья из газеты"://переделать для Интернет-ресурса
                        await client.SendTextMessageAsync(msg.Chat.Id, "Пример оформления списка летературы для типа источника" + " - статьи из газеты" + " : \n" +
                            "Фамилия и инициалы автора:\n" +
                            "<i>Иванов И.М</i>\n" +
                            "Название статьи:\n" +
                            "<i>Наука как искусство</i>\n" +
                            "Название газеты:\n" +
                            "<i> Вечерний Гродно</i>\n" +
                            "Год издания:\n" +
                            "<i>2020</i>\n" +
                            "Дата выхода газеты:\n" +
                            "<i>01.10</i>\n" +
                            "Номер статьи:\n" +
                            "<i>5</i>\n"                             , replyMarkup: GetButtons(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        await client.SendTextMessageAsync(msg.Chat.Id, "Результат:\n" +
                            "Иванов И.М Наука как искусство // Вечерний Гродно. - 2020. - 01.10. - Ст. 5\n", replyMarkup: replyMarkup1);
                        Console.WriteLine($"Пользователь выбрал -  статья из газеты: {msg.Text}");//консолька
                        await client.SendTextMessageAsync(
                        chatId: msg.Chat.Id,
                        text: "Введите фамилию и инициалы автора:");

                        // Отписываемся от предыдущего обработчика, если он существует
                        if (_handler != null)
                            client.OnMessage -= _handler;

                        _handler = async (s, args) =>
                        {
                            var innerMessage = args.Message;
                            if (innerMessage.Text != null)
                            {
                                var author = innerMessage.Text;

                                await client.SendTextMessageAsync(
                                    chatId: innerMessage.Chat.Id,
                                    text: "Введите название статьи:");

                                // Отписываемся от текущего обработчика, чтобы избежать дублирования вызовов
                                client.OnMessage -= _handler;
                                _handler = async (s1, args1) =>
                                {
                                    var articleTitle = args1.Message.Text;

                                    await client.SendTextMessageAsync(
                                        chatId: args1.Message.Chat.Id,
                                        text: "Введите название газеты:");

                                    client.OnMessage -= _handler;
                                    _handler = async (s2, args2) =>
                                    {
                                        var newspaperTitle = args2.Message.Text;

                                        await client.SendTextMessageAsync(
                                            chatId: args2.Message.Chat.Id,
                                            text: "Введите год издания:");

                                        client.OnMessage -= _handler;
                                        _handler = async (s3, args3) =>
                                        {
                                            if (int.TryParse(args3.Message.Text, out int year))
                                            {
                                                await client.SendTextMessageAsync(
                                                    chatId: args3.Message.Chat.Id,
                                                    text: "Введите дату выхода газеты:");

                                                client.OnMessage -= _handler;
                                                _handler = async (s4, args4) =>
                                                {
                                                    var releaseDate = args4.Message.Text;

                                                    await client.SendTextMessageAsync(
                                                        chatId: args4.Message.Chat.Id,
                                                        text: "Введите номер статьи:");

                                                    client.OnMessage -= _handler;
                                                    _handler = async (s5, args5) =>
                                                    {
                                                        var articleNumber = args5.Message.Text;

                                                        await client.SendTextMessageAsync(
                                                            chatId: args5.Message.Chat.Id,
                                                            text: $"{author}" +
                                                                  $" {articleTitle} // " +
                                                                  $" {newspaperTitle}.-" +
                                                                  $" {year}.-" +
                                                                  $"{releaseDate}.-" +
                                                                  $" C. {articleNumber}", replyMarkup: GetInlineButtons());
                                                    };

                                                    client.OnMessage += _handler;
                                                };

                                                client.OnMessage += _handler;
                                            }
                                            else
                                            {
                                                await client.SendTextMessageAsync(
                                                    chatId: args3.Message.Chat.Id,
                                                    text: "Введите корректный год издания:");
                                            }
                                        };

                                        client.OnMessage += _handler;
                                    };

                                    client.OnMessage += _handler;
                                };

                                client.OnMessage += _handler;
                            }
                        };

                        client.OnMessage += _handler;
                        break;
                }
            }
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Книга"}, new KeyboardButton { Text = "Интернет-ресурс"} },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Законы, нормативный акт"}, new KeyboardButton { Text = "Диссертация"} },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Автореферат"}, new KeyboardButton { Text = "Статья из журнала"} },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Статья из сборника"}, new KeyboardButton { Text = "Статья из газеты"} }
                }
            };
        }

        private static IReplyMarkup GetInlineButtons()
        {

            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("↪️Главное меню")
                }
            });
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var replyMarkup1 = new ReplyKeyboardRemove();
            if (e.CallbackQuery.Data == "↪️Главное меню")
            {
                await client.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "Вы вернулись в главное меню!", replyMarkup: GetButtons());
            }
        }
    }
}



