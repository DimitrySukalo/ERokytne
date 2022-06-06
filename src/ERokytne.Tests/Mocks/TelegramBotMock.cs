using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace ERokytne.Tests.Mocks;

public class MockClientOptions
{
    public bool HandleNegativeOffset { get; set; }
    public string[] Messages { get; set; } = Array.Empty<string>();
    public int RequestDelay { get; set; } = 10;
    public Exception? ExceptionToThrow { get; set; }

}

public class TelegramBotMock : ITelegramBotClient
{
    private readonly Queue<string[]> _messages;

    public int MessageGroupsLeft => _messages.Count;
    public MockClientOptions Options { get; }

    public TelegramBotMock(MockClientOptions? options = default)
    {
        Options = options ?? new MockClientOptions();
        _messages = new Queue<string[]>(
            Options.Messages.Select(message => message.Split('-').ToArray())
        );
    }

    public TelegramBotMock(params string[] messages)
    {
        Options = new MockClientOptions();
        _messages = new Queue<string[]>(messages.Select(message => message.Split('-').ToArray()));
    }

    public async Task<TResponse> MakeRequestAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request is not GetUpdatesRequest getUpdatesRequest)
        {
            throw new NotImplementedException();
        }

        await Task.Delay(Options.RequestDelay, cancellationToken);

        if (Options.ExceptionToThrow is not null)
        {
            throw Options.ExceptionToThrow;
        }

        if (Options.HandleNegativeOffset && getUpdatesRequest.Offset == -1)
        {
            var messageCount = _messages.Select(group => group.Length).Sum() + 1;
            var lastMessage = _messages.Last().Last();

            _messages.Clear();

            return (TResponse) (object) new[]
            {
                new Update
                {
                    Message = new Message
                    {
                        Text = lastMessage
                    },
                    Id = messageCount
                }
            };
        }

        if (!_messages.TryDequeue(out string[]? messages))
        {
            return (TResponse) (object) Array.Empty<Update>();
        }

        return (TResponse) (object) messages.Select((_, i) => new Update
        {
            Message = new Message
            {
                Text = messages[i]
            },
            Id = getUpdatesRequest.Offset ?? 0 + i + 1
        }).ToArray();
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task DownloadFileAsync(string filePath, Stream destination,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public long? BotId { get; }
    public TimeSpan Timeout { get; set; }
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}