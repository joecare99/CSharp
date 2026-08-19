using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Protocol.Models;
using Ollama.Protocol.Parsing;
using Ollama.Protocol.Services;
using Ollama.Protocol.Tests.TestDoubles;

namespace Ollama.Protocol.Tests;

[TestClass]
public sealed class OllamaProtocolCoverageCompletionTests
{
    [TestMethod]
    public void OptionsAndStreamReaders_AcceptExpectedConstructorArguments()
    {
        JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaProtocolClientOptions(null!));
        Assert.IsNotNull(new OllamaChatStreamReader(serializerOptions));
        Assert.IsNotNull(new OllamaGenerateStreamReader(serializerOptions));
    }

    [TestMethod]
    public async Task StreamReaders_SupportAllEnumeratorCancellationTokenCombinations()
    {
        using CancellationTokenSource firstCancellationTokenSource = new();
        using CancellationTokenSource secondCancellationTokenSource = new();

        foreach ((CancellationToken methodToken, CancellationToken enumeratorToken) in GetTokenCombinations(
            firstCancellationTokenSource.Token,
            secondCancellationTokenSource.Token))
        {
            await ConsumeChatReaderAsync(methodToken, enumeratorToken);
            await ConsumeGenerateReaderAsync(methodToken, enumeratorToken);
        }
    }

    [TestMethod]
    public async Task StreamReaders_HandleEmptyStreams()
    {
        await using MemoryStream chatStream = new();
        await using MemoryStream generateStream = new();
        OllamaChatStreamReader chatReader = new();
        OllamaGenerateStreamReader generateReader = new();

        await ConsumeAsync(chatReader.ReadChunksAsync(chatStream), default);
        await ConsumeAsync(generateReader.ReadChunksAsync(generateStream), default);
    }

    [TestMethod]
    public async Task StreamReaders_PropagateMalformedPayloadsAndMidStreamCancellation()
    {
        await using MemoryStream malformedChatStream = new(Encoding.UTF8.GetBytes("not-json\n"));
        await using MemoryStream malformedGenerateStream = new(Encoding.UTF8.GetBytes("not-json\n"));
        await using MemoryStream cancelableChatStream = new(Encoding.UTF8.GetBytes(
            "{\"message\":{\"role\":\"assistant\",\"content\":\"first\"},\"done\":false}\n"
            + "{\"message\":{\"role\":\"assistant\",\"content\":\"second\"},\"done\":true}\n"));
        await using MemoryStream cancelableGenerateStream = new(Encoding.UTF8.GetBytes(
            "{\"response\":\"first\",\"done\":false}\n"
            + "{\"response\":\"second\",\"done\":true}\n"));
        OllamaChatStreamReader chatReader = new();
        OllamaGenerateStreamReader generateReader = new();

        await Assert.ThrowsExactlyAsync<JsonException>(() => ConsumeAsync(chatReader.ReadChunksAsync(malformedChatStream), default));
        await Assert.ThrowsExactlyAsync<JsonException>(() => ConsumeAsync(generateReader.ReadChunksAsync(malformedGenerateStream), default));
        await AssertCanceledDuringEnumerationAsync(chatReader.ReadChunksAsync(cancelableChatStream));
        await AssertCanceledDuringEnumerationAsync(generateReader.ReadChunksAsync(cancelableGenerateStream));
    }

    [TestMethod]
    public async Task StreamReaders_HandleAsynchronousReadsAndEarlyEnumerationDisposal()
    {
        byte[] chatPayload = Encoding.UTF8.GetBytes(
            "{\"message\":{\"role\":\"assistant\",\"content\":\"first\"},\"done\":false}\n"
            + "{\"message\":{\"role\":\"assistant\",\"content\":\"second\"},\"done\":true}\n");
        byte[] generatePayload = Encoding.UTF8.GetBytes(
            "{\"response\":\"first\",\"done\":false}\n"
            + "{\"response\":\"second\",\"done\":true}\n");
        OllamaChatStreamReader chatReader = new();
        OllamaGenerateStreamReader generateReader = new();

        await using (AsynchronousReadStream chatStream = new(chatPayload))
        {
            await ConsumeAsync(chatReader.ReadChunksAsync(chatStream), default);
        }

        await using (AsynchronousReadStream generateStream = new(generatePayload))
        {
            await ConsumeAsync(generateReader.ReadChunksAsync(generateStream), default);
        }

        await using (AsynchronousReadStream chatStream = new(chatPayload))
        {
            await foreach (OllamaChatResponseChunk _ in chatReader.ReadChunksAsync(chatStream))
            {
                break;
            }
        }

        await using (AsynchronousReadStream generateStream = new(generatePayload))
        {
            await foreach (OllamaGenerateResponseChunk _ in generateReader.ReadChunksAsync(generateStream))
            {
                break;
            }
        }
    }

    [TestMethod]
    public async Task StreamReaders_SupportDisposalBeforeEnumerationStarts()
    {
        await using MemoryStream chatStream = new(Encoding.UTF8.GetBytes("{\"message\":{\"role\":\"assistant\",\"content\":\"chat\"},\"done\":true}\n"));
        await using MemoryStream generateStream = new(Encoding.UTF8.GetBytes("{\"response\":\"generate\",\"done\":true}\n"));
        IAsyncEnumerator<OllamaChatResponseChunk> chatEnumerator = new OllamaChatStreamReader()
            .ReadChunksAsync(chatStream)
            .GetAsyncEnumerator();
        IAsyncEnumerator<OllamaGenerateResponseChunk> generateEnumerator = new OllamaGenerateStreamReader()
            .ReadChunksAsync(generateStream)
            .GetAsyncEnumerator();

        await chatEnumerator.DisposeAsync();
        await generateEnumerator.DisposeAsync();
    }

    [TestMethod]
    public async Task ProtocolClientStreams_SupportAllEnumeratorCancellationTokenCombinations()
    {
        TestHttpMessageHandler handler = new((request, cancellationToken) =>
        {
            string content = request.RequestUri?.AbsolutePath switch
            {
                "/api/chat" => "{\"message\":{\"role\":\"assistant\",\"content\":\"chat\"},\"done\":true}\n",
                "/api/generate" => "{\"response\":\"generate\",\"done\":true}\n",
                _ => throw new AssertFailedException($"Unexpected endpoint: {request.RequestUri}"),
            };

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(content))),
            });
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));
        using CancellationTokenSource firstCancellationTokenSource = new();
        using CancellationTokenSource secondCancellationTokenSource = new();

        foreach ((CancellationToken methodToken, CancellationToken enumeratorToken) in GetTokenCombinations(
            firstCancellationTokenSource.Token,
            secondCancellationTokenSource.Token))
        {
            await ConsumeAsync(
                client.ChatStreamingAsync(new OllamaChatRequest
                {
                    Model = "chat-model",
                    Messages =
                    [
                        new OllamaChatMessage
                        {
                            Role = "user",
                            Content = "hello",
                        },
                    ],
                }, methodToken),
                enumeratorToken);
            await ConsumeAsync(
                client.GenerateStreamingAsync(new OllamaGenerateRequest
                {
                    Model = "generate-model",
                    Prompt = "hello",
                }, methodToken),
                enumeratorToken);
        }
    }

    private static IEnumerable<(CancellationToken MethodToken, CancellationToken EnumeratorToken)> GetTokenCombinations(
        CancellationToken firstToken,
        CancellationToken secondToken)
    {
        yield return (default, default);
        yield return (firstToken, default);
        yield return (default, firstToken);
        yield return (firstToken, firstToken);
        yield return (firstToken, secondToken);
    }

    private static async Task ConsumeChatReaderAsync(CancellationToken methodToken, CancellationToken enumeratorToken)
    {
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes("{\"message\":{\"role\":\"assistant\",\"content\":\"chat\"},\"done\":true}\n"));
        OllamaChatStreamReader reader = new();

        await ConsumeAsync(reader.ReadChunksAsync(stream, methodToken), enumeratorToken);
    }

    private static async Task ConsumeGenerateReaderAsync(CancellationToken methodToken, CancellationToken enumeratorToken)
    {
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes("{\"response\":\"generate\",\"done\":true}\n"));
        OllamaGenerateStreamReader reader = new();

        await ConsumeAsync(reader.ReadChunksAsync(stream, methodToken), enumeratorToken);
    }

    private static async Task ConsumeAsync<T>(IAsyncEnumerable<T> values, CancellationToken cancellationToken)
    {
        await foreach (T _ in values.WithCancellation(cancellationToken))
        {
        }
    }

    private static async Task AssertCanceledDuringEnumerationAsync<T>(IAsyncEnumerable<T> values)
    {
        using CancellationTokenSource cancellationTokenSource = new();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () =>
        {
            await foreach (T _ in values.WithCancellation(cancellationTokenSource.Token))
            {
                cancellationTokenSource.Cancel();
            }
        });
    }

    private sealed class AsynchronousReadStream : Stream
    {
        private readonly MemoryStream _innerStream;

        public AsynchronousReadStream(byte[] content)
        {
            _innerStream = new MemoryStream(content);
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

        public override int Read(Span<byte> buffer) => _innerStream.Read(buffer);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => ReadAsynchronouslyAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => ReadAsynchronouslyAsync(buffer, cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();

        private async ValueTask<int> ReadAsynchronouslyAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            await Task.Yield();
            return await _innerStream.ReadAsync(buffer, cancellationToken);
        }
    }
}
