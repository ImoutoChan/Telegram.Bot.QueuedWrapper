using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace Telegram.Bot.QueuedWrapper
{
    public class QueuedTelegramBotClient : ITelegramBotClient
    {
        private readonly ITelegramBotClient _telegramBotClientImplementation;
        private readonly TelegramMessageQueue _messageQueue;

        public QueuedTelegramBotClient(
            string token,
            HttpClient httpClient = null)
        {
            _telegramBotClientImplementation = new TelegramBotClient(token, httpClient);
            _messageQueue = new TelegramMessageQueue();
        }

        public async Task<TResponse> MakeRequestAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.MakeRequestAsync(request,
                                                                                                      cancellationToken));
        }

        public async Task<bool> TestApiAsync(
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.TestApiAsync(cancellationToken));
        }

        public void StartReceiving(
            UpdateType[] allowedUpdates = null,
            CancellationToken cancellationToken = default)
        {
            _telegramBotClientImplementation.StartReceiving(allowedUpdates, cancellationToken);
        }

        public void StopReceiving()
        {
            _telegramBotClientImplementation.StopReceiving();
        }

        public async Task<Update[]> GetUpdatesAsync(
            int offset = 0,
            int limit = 0,
            int timeout = 0,
            IEnumerable<UpdateType> allowedUpdates = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetUpdatesAsync(offset,
                                                                                                              limit,
                                                                                                              timeout,
                                                                                                              allowedUpdates,
                                                                                                              cancellationToken));
        }

        public async Task SetWebhookAsync(
            string url,
            InputFileStream certificate = null,
            int maxConnections = 0,
            IEnumerable<UpdateType> allowedUpdates = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetWebhookAsync(url,
                                                                                                       certificate,
                                                                                                       maxConnections,
                                                                                                       allowedUpdates,
                                                                                                       cancellationToken));
        }

        public async Task DeleteWebhookAsync(
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .DeleteWebhookAsync(cancellationToken));
        }

        public async Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                 .GetWebhookInfoAsync(cancellationToken));
        }

        public async Task<User> GetMeAsync(
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetMeAsync(cancellationToken));
        }

        public async Task<Message> SendTextMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() =>
                                                       {
                                                           Debug.WriteLine(DateTime.UtcNow.ToString("O"));
                                                           return _telegramBotClientImplementation
                                                              .SendTextMessageAsync(chatId,
                                                                                    text,
                                                                                    parseMode,
                                                                                    disableWebPagePreview,
                                                                                    disableNotification,
                                                                                    replyToMessageId,
                                                                                    replyMarkup,
                                                                                    cancellationToken);
                                                       },
                                                       chatId);
        }

        public async Task<Message> ForwardMessageAsync(
            ChatId chatId,
            ChatId fromChatId,
            int messageId,
            bool disableNotification = false,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.ForwardMessageAsync(chatId,
                                                                                                         fromChatId,
                                                                                                         messageId,
                                                                                                         disableNotification,
                                                                                                         cancellationToken),
                                              chatId);
        }

        public async Task<Message> SendPollAsync(
            ChatId chatId,
            string question,
            IEnumerable<string> options,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            bool? isAnonymous = default,
            PollType? type = default,
            bool? allowsMultipleAnswers = default,
            int? correctOptionId = default,
            bool? isClosed = default,
            string explanation = default,
            ParseMode explanationParseMode = default,
            int? openPeriod = default,
            DateTime? closeDate = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendPollAsync(chatId,
                                                                                                            question,
                                                                                                            options,
                                                                                                            disableNotification,
                                                                                                            replyToMessageId,
                                                                                                            replyMarkup,
                                                                                                            cancellationToken,
                                                                                                            isAnonymous,
                                                                                                            type,
                                                                                                            allowsMultipleAnswers,
                                                                                                            correctOptionId,
                                                                                                            isClosed,
                                                                                                            explanation,
                                                                                                            explanationParseMode,
                                                                                                            openPeriod,
                                                                                                            closeDate),
                                                       chatId);
        }

        public async Task<Poll> StopPollAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.StopPollAsync(chatId,
                                                                                                            messageId,
                                                                                                            replyMarkup,
                                                                                                            cancellationToken),
                                                       chatId);

        }

        public async Task<Message> SendDiceAsync(
            ChatId chatId,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            Emoji? emoji = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendDiceAsync(chatId,
                                                                                                            disableNotification,
                                                                                                            replyToMessageId,
                                                                                                            replyMarkup,
                                                                                                            cancellationToken,
                                                                                                            emoji),
                                                       chatId);
        }

        public async Task SetChatAdministratorCustomTitleAsync(
            ChatId chatId,
            int userId,
            string customTitle,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatAdministratorCustomTitleAsync(chatId,
                                                                                                                            userId,
                                                                                                                            customTitle,
                                                                                                                            cancellationToken),
                                                       chatId);
        }

        public async Task SetChatPermissionsAsync(
            ChatId chatId,
            ChatPermissions permissions,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatPermissionsAsync(chatId,
                                                                                                               permissions,
                                                                                                               cancellationToken),
                                                       chatId);
        }

        public async Task<BotCommand[]> GetMyCommandsAsync(CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetMyCommandsAsync(cancellationToken));
        }

        public async Task SetMyCommandsAsync(
            IEnumerable<BotCommand> commands,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetMyCommandsAsync(commands,
                                                                                                          cancellationToken));
        }

        public async Task<Message> SendPhotoAsync(
            ChatId chatId,
            InputOnlineFile photo,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendPhotoAsync(chatId,
                                                                                                             photo,
                                                                                                             caption,
                                                                                                             parseMode,
                                                                                                             disableNotification,
                                                                                                             replyToMessageId,
                                                                                                             replyMarkup,
                                                                                                             cancellationToken),
                                                       chatId);
        }

        public async Task<Message> SendAudioAsync(
            ChatId chatId,
            InputOnlineFile audio,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            int duration = 0,
            string performer = null,
            string title = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            InputMedia thumb = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendAudioAsync(chatId,
                                                                                                             audio,
                                                                                                             caption,
                                                                                                             parseMode,
                                                                                                             duration,
                                                                                                             performer,
                                                                                                             title,
                                                                                                             disableNotification,
                                                                                                             replyToMessageId,
                                                                                                             replyMarkup,
                                                                                                             cancellationToken,
                                                                                                             thumb),
                                                       chatId);
        }

        public async Task<Message> SendDocumentAsync(
            ChatId chatId,
            InputOnlineFile document,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            InputMedia thumb = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendDocumentAsync(chatId,
                                                                                                                document,
                                                                                                                caption,
                                                                                                                parseMode,
                                                                                                                disableNotification,
                                                                                                                replyToMessageId,
                                                                                                                replyMarkup,
                                                                                                                cancellationToken,
                                                                                                                thumb),
                                                       chatId);
        }

        public async Task<Message> SendStickerAsync(
            ChatId chatId,
            InputOnlineFile sticker,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendStickerAsync(chatId,
                                                                                                               sticker,
                                                                                                               disableNotification,
                                                                                                               replyToMessageId,
                                                                                                               replyMarkup,
                                                                                                               cancellationToken),
                                                       chatId);
        }

        public async Task<Message> SendVideoAsync(
            ChatId chatId,
            InputOnlineFile video,
            int duration = 0,
            int width = 0,
            int height = 0,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool supportsStreaming = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            InputMedia thumb = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendVideoAsync(chatId,
                                                                                                             video,
                                                                                                             duration,
                                                                                                             width,
                                                                                                             height,
                                                                                                             caption,
                                                                                                             parseMode,
                                                                                                             supportsStreaming,
                                                                                                             disableNotification,
                                                                                                             replyToMessageId,
                                                                                                             replyMarkup,
                                                                                                             cancellationToken,
                                                                                                             thumb),
                                                       chatId);
        }

        public async Task<Message> SendAnimationAsync(
            ChatId chatId,
            InputOnlineFile animation,
            int duration = 0,
            int width = 0,
            int height = 0,
            InputMedia thumb = null,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendAnimationAsync(chatId,
                                                                                                                 animation,
                                                                                                                 duration,
                                                                                                                 width,
                                                                                                                 height,
                                                                                                                 thumb,
                                                                                                                 caption,
                                                                                                                 parseMode,
                                                                                                                 disableNotification,
                                                                                                                 replyToMessageId,
                                                                                                                 replyMarkup,
                                                                                                                 cancellationToken),
                                                       chatId);
        }

        public async Task<Message> SendVoiceAsync(
            ChatId chatId,
            InputOnlineFile voice,
            string caption = null,
            ParseMode parseMode = ParseMode.Default,
            int duration = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _telegramBotClientImplementation.SendVoiceAsync(chatId,
                                                                         voice,
                                                                         caption,
                                                                         parseMode,
                                                                         duration,
                                                                         disableNotification,
                                                                         replyToMessageId,
                                                                         replyMarkup,
                                                                         cancellationToken);
        }

        public async Task<Message> SendVideoNoteAsync(
            ChatId chatId,
            InputTelegramFile videoNote,
            int duration = 0,
            int length = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            InputMedia thumb = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendVideoNoteAsync(chatId,
                                                                                                                 videoNote,
                                                                                                                 duration,
                                                                                                                 length,
                                                                                                                 disableNotification,
                                                                                                                 replyToMessageId,
                                                                                                                 replyMarkup,
                                                                                                                 cancellationToken,
                                                                                                                 thumb),
                                                       chatId);
        }

        [Obsolete]
        public async Task<Message[]> SendMediaGroupAsync(
            ChatId chatId,
            IEnumerable<InputMediaBase> media,
            bool disableNotification = false,
            int replyToMessageId = 0,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendMediaGroupAsync(chatId,
                                                                                                         media,
                                                                                                         disableNotification,
                                                                                                         replyToMessageId,
                                                                                                         cancellationToken),
                                              chatId);
        }

        public async Task<Message[]> SendMediaGroupAsync(
            IEnumerable<IAlbumInputMedia> inputMedia,
            ChatId chatId,
            bool disableNotification = false,
            int replyToMessageId = 0,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendMediaGroupAsync(inputMedia,
                                                                                                         chatId,
                                                                                                         disableNotification,
                                                                                                         replyToMessageId,
                                                                                                         cancellationToken),
                                              chatId);
        }

        public async Task<Message> SendLocationAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            int livePeriod = 0,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendLocationAsync(chatId,
                                                                                                                latitude,
                                                                                                                longitude,
                                                                                                                livePeriod,
                                                                                                                disableNotification,
                                                                                                                replyToMessageId,
                                                                                                                replyMarkup,
                                                                                                                cancellationToken),
                                                       chatId);
        }

        public async Task<Message> SendVenueAsync(
            ChatId chatId,
            float latitude,
            float longitude,
            string title,
            string address,
            string foursquareId = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            string foursquareType = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendVenueAsync(chatId,
                                                                                                             latitude,
                                                                                                             longitude,
                                                                                                             title,
                                                                                                             address,
                                                                                                             foursquareId,
                                                                                                             disableNotification,
                                                                                                             replyToMessageId,
                                                                                                             replyMarkup,
                                                                                                             cancellationToken,
                                                                                                             foursquareType),
                                                       chatId);
        }

        public async Task<Message> SendContactAsync(
            ChatId chatId,
            string phoneNumber,
            string firstName,
            string lastName = null,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            string vCard = null)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendContactAsync(chatId,
                                                                                                               phoneNumber,
                                                                                                               firstName,
                                                                                                               lastName,
                                                                                                               disableNotification,
                                                                                                               replyToMessageId,
                                                                                                               replyMarkup,
                                                                                                               cancellationToken,
                                                                                                               vCard),
                                                       chatId);
        }

        public async Task SendChatActionAsync(
            ChatId chatId,
            ChatAction chatAction,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendChatActionAsync(chatId,
                                                                                                           chatAction,
                                                                                                           cancellationToken),
                                                chatId);
        }

        public async Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            int userId,
            int offset = 0,
            int limit = 0,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetUserProfilePhotosAsync(userId,
                                                                                                               offset,
                                                                                                               limit,
                                                                                                               cancellationToken));
        }

        public async Task<File> GetFileAsync(
            string fileId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetFileAsync(fileId,
                                                                                                  cancellationToken));
        }

        [Obsolete]
        public async Task<Stream> DownloadFileAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.DownloadFileAsync(filePath,
                                                                                                       cancellationToken));
        }

        public async Task DownloadFileAsync(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.DownloadFileAsync(filePath,
                                                                                                         destination,
                                                                                                         cancellationToken));
        }

        public async Task<File> GetInfoAndDownloadFileAsync(
            string fileId,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                          .GetInfoAndDownloadFileAsync(fileId,
                                                                                       destination,
                                                                                       cancellationToken));
        }

        public async Task KickChatMemberAsync(
            ChatId chatId,
            int userId,
            DateTime untilDate = new DateTime(),
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.KickChatMemberAsync(chatId,
                                                                                                           userId,
                                                                                                           untilDate,
                                                                                                           cancellationToken),
                                                chatId);
        }

        public async Task LeaveChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.LeaveChatAsync(chatId,
                                                                                                      cancellationToken),
                                                chatId);
        }

        public async Task UnbanChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.UnbanChatMemberAsync(chatId,
                                                                                                            userId,
                                                                                                            cancellationToken),
                                                chatId);
        }

        public async Task<Chat> GetChatAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetChatAsync(chatId,
                                                                                                  cancellationToken),
                                              chatId);
        }

        public async Task<ChatMember[]> GetChatAdministratorsAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetChatAdministratorsAsync(chatId,
                                                                                                                cancellationToken),
                                              chatId);
        }

        public async Task<int> GetChatMembersCountAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetChatMembersCountAsync(chatId,
                                                                                                              cancellationToken),
                                              chatId);
        }

        public async Task<ChatMember> GetChatMemberAsync(
            ChatId chatId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetChatMemberAsync(chatId,
                                                                                                        userId,
                                                                                                        cancellationToken),
                                              chatId);
        }

        public async Task AnswerCallbackQueryAsync(
            string callbackQueryId,
            string text = null,
            bool showAlert = false,
            string url = null,
            int cacheTime = 0,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .AnswerCallbackQueryAsync(callbackQueryId,
                                                                             text,
                                                                             showAlert,
                                                                             url,
                                                                             cacheTime,
                                                                             cancellationToken));
        }

        public async Task RestrictChatMemberAsync(
            ChatId chatId,
            int userId,
            ChatPermissions permissions,
            DateTime untilDate = default,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.RestrictChatMemberAsync(chatId,
                                                                                                               userId,
                                                                                                               permissions,
                                                                                                               untilDate,
                                                                                                               cancellationToken),
                                                chatId);
        }

        public async Task PromoteChatMemberAsync(
            ChatId chatId,
            int userId,
            bool? canChangeInfo = null,
            bool? canPostMessages = null,
            bool? canEditMessages = null,
            bool? canDeleteMessages = null,
            bool? canInviteUsers = null,
            bool? canRestrictMembers = null,
            bool? canPinMessages = null,
            bool? canPromoteMembers = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.PromoteChatMemberAsync(chatId,
                                                                                                              userId,
                                                                                                              canChangeInfo,
                                                                                                              canPostMessages,
                                                                                                              canEditMessages,
                                                                                                              canDeleteMessages,
                                                                                                              canInviteUsers,
                                                                                                              canRestrictMembers,
                                                                                                              canPinMessages,
                                                                                                              canPromoteMembers,
                                                                                                              cancellationToken),
                                                chatId);
        }

        public async Task<Message> EditMessageTextAsync(
            ChatId chatId,
            int messageId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.EditMessageTextAsync(chatId,
                                                                                                          messageId,
                                                                                                          text,
                                                                                                          parseMode,
                                                                                                          disableWebPagePreview,
                                                                                                          replyMarkup,
                                                                                                          cancellationToken),
                                              chatId);
        }

        public async Task EditMessageTextAsync(
            string inlineMessageId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .EditMessageTextAsync(inlineMessageId,
                                                                         text,
                                                                         parseMode,
                                                                         disableWebPagePreview,
                                                                         replyMarkup,
                                                                         cancellationToken));
        }

        public async Task<Message> StopMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                          .StopMessageLiveLocationAsync(chatId,
                                                                                        messageId,
                                                                                        replyMarkup,
                                                                                        cancellationToken),
                                                       chatId);
        }

        public async Task StopMessageLiveLocationAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .StopMessageLiveLocationAsync(inlineMessageId,
                                                                                 replyMarkup,
                                                                                 cancellationToken));
        }

        public async Task<Message> EditMessageCaptionAsync(
            ChatId chatId,
            int messageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            ParseMode parseMode = ParseMode.Default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.EditMessageCaptionAsync(chatId,
                                                                                                             messageId,
                                                                                                             caption,
                                                                                                             replyMarkup,
                                                                                                             cancellationToken,
                                                                                                             parseMode),
                                              chatId);
        }

        public async Task EditMessageCaptionAsync(
            string inlineMessageId,
            string caption,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            ParseMode parseMode = ParseMode.Default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .EditMessageCaptionAsync(inlineMessageId,
                                                                            caption,
                                                                            replyMarkup,
                                                                            cancellationToken,
                                                                            parseMode));
        }

        public async Task<Message> EditMessageMediaAsync(
            ChatId chatId,
            int messageId,
            InputMediaBase media,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.EditMessageMediaAsync(chatId,
                                                                                                           messageId,
                                                                                                           media,
                                                                                                           replyMarkup,
                                                                                                           cancellationToken),
                                              chatId);
        }

        public async Task EditMessageMediaAsync(
            string inlineMessageId,
            InputMediaBase media,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .EditMessageMediaAsync(inlineMessageId,
                                                                          media,
                                                                          replyMarkup,
                                                                          cancellationToken));
        }


        public async Task<Message> EditMessageReplyMarkupAsync(
            ChatId chatId,
            int messageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                          .EditMessageReplyMarkupAsync(chatId,
                                                                                       messageId,
                                                                                       replyMarkup,
                                                                                       cancellationToken),
                                                       chatId);
        }

        public async Task EditMessageReplyMarkupAsync(
            string inlineMessageId,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .EditMessageReplyMarkupAsync(inlineMessageId,
                                                                                replyMarkup,
                                                                                cancellationToken));
        }

        public async Task<Message> EditMessageLiveLocationAsync(
            ChatId chatId,
            int messageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                          .EditMessageLiveLocationAsync(chatId,
                                                                                        messageId,
                                                                                        latitude,
                                                                                        longitude,
                                                                                        replyMarkup,
                                                                                        cancellationToken),
                                                       chatId);
        }

        public async Task EditMessageLiveLocationAsync(
            string inlineMessageId,
            float latitude,
            float longitude,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .EditMessageLiveLocationAsync(inlineMessageId,
                                                                                 latitude,
                                                                                 longitude,
                                                                                 replyMarkup,
                                                                                 cancellationToken));
        }

        public async Task DeleteMessageAsync(
            ChatId chatId,
            int messageId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.DeleteMessageAsync(chatId,
                                                                                                          messageId,
                                                                                                          cancellationToken),
                                                chatId);
        }

        public async Task AnswerInlineQueryAsync(
            string inlineQueryId,
            IEnumerable<InlineQueryResultBase> results,
            int? cacheTime = null,
            bool isPersonal = false,
            string nextOffset = null,
            string switchPmText = null,
            string switchPmParameter = null,
            CancellationToken cancellationToken = default)
        {
            await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                 .AnswerInlineQueryAsync(inlineQueryId,
                                                                         results,
                                                                         cacheTime,
                                                                         isPersonal,
                                                                         nextOffset,
                                                                         switchPmText,
                                                                         switchPmParameter,
                                                                         cancellationToken));
        }

        public async Task<Message> SendInvoiceAsync(
            int chatId,
            string title,
            string description,
            string payload,
            string providerToken,
            string startParameter,
            string currency,
            IEnumerable<LabeledPrice> prices,
            string providerData = default,
            string photoUrl = default,
            int photoSize = default,
            int photoWidth = default,
            int photoHeight = default,
            bool needName = default,
            bool needPhoneNumber = default,
            bool needEmail = default,
            bool needShippingAddress = default,
            bool isFlexible = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            InlineKeyboardMarkup replyMarkup = default,
            CancellationToken cancellationToken = default,
            bool sendPhoneNumberToProvider = default,
            bool sendEmailToProvider = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendInvoiceAsync(chatId,
                                                                                                               title,
                                                                                                               description,
                                                                                                               payload,
                                                                                                               providerToken,
                                                                                                               startParameter,
                                                                                                               currency,
                                                                                                               prices,
                                                                                                               providerData,
                                                                                                               photoUrl,
                                                                                                               photoSize,
                                                                                                               photoWidth,
                                                                                                               photoHeight,
                                                                                                               needName,
                                                                                                               needPhoneNumber,
                                                                                                               needEmail,
                                                                                                               needShippingAddress,
                                                                                                               isFlexible,
                                                                                                               disableNotification,
                                                                                                               replyToMessageId,
                                                                                                               replyMarkup,
                                                                                                               cancellationToken,
                                                                                                               sendPhoneNumberToProvider,
                                                                                                               sendEmailToProvider));
        }

        public async Task AnswerShippingQueryAsync(
            string shippingQueryId,
            IEnumerable<ShippingOption> shippingOptions,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .AnswerShippingQueryAsync(shippingQueryId,
                                                                             shippingOptions,
                                                                             cancellationToken));
        }

        public async Task AnswerShippingQueryAsync(
            string shippingQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .AnswerShippingQueryAsync(shippingQueryId,
                                                                             errorMessage,
                                                                             cancellationToken));
        }

        public async Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .AnswerPreCheckoutQueryAsync(preCheckoutQueryId, cancellationToken));
        }

        public async Task AnswerPreCheckoutQueryAsync(
            string preCheckoutQueryId,
            string errorMessage,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .AnswerPreCheckoutQueryAsync(preCheckoutQueryId,
                                                                                errorMessage,
                                                                                cancellationToken));
        }

        public async Task<Message> SendGameAsync(
            long chatId,
            string gameShortName,
            bool disableNotification = false,
            int replyToMessageId = 0,
            InlineKeyboardMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SendGameAsync(chatId,
                                                                                                            gameShortName,
                                                                                                            disableNotification,
                                                                                                            replyToMessageId,
                                                                                                            replyMarkup,
                                                                                                            cancellationToken),
                                                       chatId.ToString());
        }

        public async Task<Message> SetGameScoreAsync(
            int userId,
            int score,
            long chatId,
            int messageId,
            bool force = false,
            bool disableEditMessage = false,
            CancellationToken cancellationToken = default)
        {
            return await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetGameScoreAsync(userId,
                                                                                                                score,
                                                                                                                chatId,
                                                                                                                messageId,
                                                                                                                force,
                                                                                                                disableEditMessage,
                                                                                                                cancellationToken));
        }

        public async Task SetGameScoreAsync(
            int userId,
            int score,
            string inlineMessageId,
            bool force = false,
            bool disableEditMessage = false,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetGameScoreAsync(userId,
                                                                                                         score,
                                                                                                         inlineMessageId,
                                                                                                         force,
                                                                                                         disableEditMessage,
                                                                                                         cancellationToken));
        }

        public async Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            long chatId,
            int messageId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetGameHighScoresAsync(userId,
                                                                                                            chatId,
                                                                                                            messageId,
                                                                                                            cancellationToken),
                                              chatId.ToString());
        }

        public async Task<GameHighScore[]> GetGameHighScoresAsync(
            int userId,
            string inlineMessageId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetGameHighScoresAsync(userId,
                                                                                                            inlineMessageId,
                                                                                                            cancellationToken));
        }

        public async Task<StickerSet> GetStickerSetAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.GetStickerSetAsync(name,
                                                                                                        cancellationToken));
        }

        public async Task<File> UploadStickerFileAsync(
            int userId,
            InputFileStream pngSticker,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.UploadStickerFileAsync(userId,
                                                                                                            pngSticker,
                                                                                                            cancellationToken));
        }

        public async Task CreateNewStickerSetAsync(
            int userId,
            string name,
            string title,
            InputOnlineFile pngSticker,
            string emojis,
            bool isMasks = false,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.CreateNewStickerSetAsync(userId,
                                                                                                                name,
                                                                                                                title,
                                                                                                                pngSticker,
                                                                                                                emojis,
                                                                                                                isMasks,
                                                                                                                maskPosition,
                                                                                                                cancellationToken));
        }

        public async Task AddStickerToSetAsync(
            int userId,
            string name,
            InputOnlineFile pngSticker,
            string emojis,
            MaskPosition maskPosition = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.AddStickerToSetAsync(userId,
                                                                                                            name,
                                                                                                            pngSticker,
                                                                                                            emojis,
                                                                                                            maskPosition,
                                                                                                            cancellationToken));
        }

        public async Task CreateNewAnimatedStickerSetAsync(
            int userId,
            string name,
            string title,
            InputFileStream tgsSticker,
            string emojis,
            bool isMasks = default,
            MaskPosition maskPosition = default,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.CreateNewAnimatedStickerSetAsync(userId,
                                                                                                                        name,
                                                                                                                        title,
                                                                                                                        tgsSticker,
                                                                                                                        emojis,
                                                                                                                        isMasks,
                                                                                                                        maskPosition,
                                                                                                                        cancellationToken));
        }

        public async Task AddAnimatedStickerToSetAsync(
            int userId,
            string name,
            InputFileStream tgsSticker,
            string emojis,
            MaskPosition maskPosition = default,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.AddAnimatedStickerToSetAsync(userId,
                                                                                                                    name,
                                                                                                                    tgsSticker,
                                                                                                                    emojis,
                                                                                                                    maskPosition,
                                                                                                                    cancellationToken));
        }

        public async Task SetStickerSetThumbAsync(
            string name,
            int userId,
            InputOnlineFile thumb = default,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetStickerSetThumbAsync(name,
                                                                                                               userId,
                                                                                                               thumb,
                                                                                                               cancellationToken));
        }

        public async Task SetStickerPositionInSetAsync(
            string sticker,
            int position,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .SetStickerPositionInSetAsync(sticker, position, cancellationToken));
        }

        public async Task DeleteStickerFromSetAsync(
            string sticker,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation
                                                   .DeleteStickerFromSetAsync(sticker, cancellationToken));
        }

        public async Task<string> ExportChatInviteLinkAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            return await
                _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.ExportChatInviteLinkAsync(chatId,
                                                                                                               cancellationToken),
                                              chatId);
        }

        public async Task SetChatPhotoAsync(
            ChatId chatId,
            InputFileStream photo,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatPhotoAsync(chatId,
                                                                                                         photo,
                                                                                                         cancellationToken),
                                                chatId);
        }

        public async Task DeleteChatPhotoAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.DeleteChatPhotoAsync(chatId,
                                                                                                            cancellationToken),
                                                chatId);
        }

        public async Task SetChatTitleAsync(
            ChatId chatId,
            string title,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatTitleAsync(chatId,
                                                                                                         title,
                                                                                                         cancellationToken),
                                                chatId);
        }

        public async Task SetChatDescriptionAsync(
            ChatId chatId,
            string description = null,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatDescriptionAsync(chatId,
                                                                                                               description,
                                                                                                               cancellationToken),
                                                chatId);
        }

        public async Task PinChatMessageAsync(
            ChatId chatId,
            int messageId,
            bool disableNotification = false,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.PinChatMessageAsync(chatId,
                                                                                                           messageId,
                                                                                                           disableNotification,
                                                                                                           cancellationToken),
                                                chatId);
        }

        public async Task UnpinChatMessageAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.UnpinChatMessageAsync(chatId,
                                                                                                             cancellationToken),
                                                chatId);
        }

        public async Task SetChatStickerSetAsync(
            ChatId chatId,
            string stickerSetName,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.SetChatStickerSetAsync(chatId,
                                                                                                              stickerSetName,
                                                                                                              cancellationToken),
                                                chatId);
        }

        public async Task DeleteChatStickerSetAsync(
            ChatId chatId,
            CancellationToken cancellationToken = default)
        {
            await _messageQueue.RunThroughQueue(() => _telegramBotClientImplementation.DeleteChatStickerSetAsync(chatId,
                                                                                                                 cancellationToken),
                                                chatId);
        }

        public int BotId => _telegramBotClientImplementation.BotId;

        public TimeSpan Timeout
        {
            get => _telegramBotClientImplementation.Timeout;
            set => _telegramBotClientImplementation.Timeout = value;
        }

        public bool IsReceiving => _telegramBotClientImplementation.IsReceiving;

        public int MessageOffset
        {
            get => _telegramBotClientImplementation.MessageOffset;
            set => _telegramBotClientImplementation.MessageOffset = value;
        }

        public event EventHandler<ApiRequestEventArgs> MakingApiRequest
        {
            add => _telegramBotClientImplementation.MakingApiRequest += value;
            remove => _telegramBotClientImplementation.MakingApiRequest -= value;
        }

        public event EventHandler<ApiResponseEventArgs> ApiResponseReceived
        {
            add => _telegramBotClientImplementation.ApiResponseReceived += value;
            remove => _telegramBotClientImplementation.ApiResponseReceived -= value;
        }

        public event EventHandler<UpdateEventArgs> OnUpdate
        {
            add => _telegramBotClientImplementation.OnUpdate += value;
            remove => _telegramBotClientImplementation.OnUpdate -= value;
        }

        public event EventHandler<MessageEventArgs> OnMessage
        {
            add => _telegramBotClientImplementation.OnMessage += value;
            remove => _telegramBotClientImplementation.OnMessage -= value;
        }

        public event EventHandler<MessageEventArgs> OnMessageEdited
        {
            add => _telegramBotClientImplementation.OnMessageEdited += value;
            remove => _telegramBotClientImplementation.OnMessageEdited -= value;
        }

        public event EventHandler<InlineQueryEventArgs> OnInlineQuery
        {
            add => _telegramBotClientImplementation.OnInlineQuery += value;
            remove => _telegramBotClientImplementation.OnInlineQuery -= value;
        }

        public event EventHandler<ChosenInlineResultEventArgs> OnInlineResultChosen
        {
            add => _telegramBotClientImplementation.OnInlineResultChosen += value;
            remove => _telegramBotClientImplementation.OnInlineResultChosen -= value;
        }

        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery
        {
            add => _telegramBotClientImplementation.OnCallbackQuery += value;
            remove => _telegramBotClientImplementation.OnCallbackQuery -= value;
        }

        public event EventHandler<ReceiveErrorEventArgs> OnReceiveError
        {
            add => _telegramBotClientImplementation.OnReceiveError += value;
            remove => _telegramBotClientImplementation.OnReceiveError -= value;
        }

        public event EventHandler<ReceiveGeneralErrorEventArgs> OnReceiveGeneralError
        {
            add => _telegramBotClientImplementation.OnReceiveGeneralError += value;
            remove => _telegramBotClientImplementation.OnReceiveGeneralError -= value;
        }
    }
}