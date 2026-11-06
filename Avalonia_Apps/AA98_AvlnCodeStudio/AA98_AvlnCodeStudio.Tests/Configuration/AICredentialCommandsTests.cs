using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Config.Secrets;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Configuration;

[TestClass]
public sealed class AICredentialCommandsTests
{
    [TestMethod]
    public void AIProviderSettings_UseSelectedSystemTemplateUnlessUserOverridesIt()
    {
        AA98_AvlnCodeStudio.UI.Configuration.CodeStudioSettings settings = new()
        {
            AIProvider = AA98_AvlnCodeStudio.UI.Configuration.AIProviderPreset.IFM,
        };

        Assert.AreEqual("https://api.ifm.ai/v1/chat/completions", settings.GetEffectiveAIEndpoint());
        Assert.AreEqual("IFM/K2-Horizon-375B-A23B", settings.GetEffectiveAIModelId());

        settings.AIEndpoint = "https://custom.example/v1/chat/completions";
        settings.AIModelId = "custom-model";
        Assert.AreEqual("https://custom.example/v1/chat/completions", settings.GetEffectiveAIEndpoint());
        Assert.AreEqual("custom-model", settings.GetEffectiveAIModelId());
    }

    [TestMethod]
    public async Task AIChatCredentialCommands_RequireSelectionAndExplicitlySaveLoadAndDelete()
    {
        FakeCredentialProvider provider = new("test-keyring");
        CredentialProviderCatalog catalog = new([provider]);
        OpenAICompatibleAISettings aiSettings = new();
        MainWindowViewModel viewModel = CreateViewModel(catalog, aiSettings);

        Assert.IsNull(viewModel.SelectedCredentialProviderId);
        Assert.IsFalse(viewModel.SaveAICredentialCommand.CanExecute(null));
        Assert.AreEqual(string.Empty, aiSettings.ApiKey);

        viewModel.SelectedCredentialProviderId = provider.ProviderId;
        viewModel.AiApiKey = "api-key-test-value";
        await viewModel.SaveAICredentialCommand.ExecuteAsync(null);

        Assert.AreEqual("api-key-test-value", provider.StoredValue);
        Assert.AreEqual("api-key-test-value", aiSettings.ApiKey);
        Assert.IsFalse(JsonSerializer.Serialize(new AA98_AvlnCodeStudio.UI.Configuration.CodeStudioSettings())
            .Contains("api-key-test-value", StringComparison.Ordinal));

        viewModel.AiApiKey = string.Empty;
        await viewModel.LoadAICredentialCommand.ExecuteAsync(null);

        Assert.AreEqual("api-key-test-value", viewModel.AiApiKey);
        Assert.AreEqual("api-key-test-value", aiSettings.ApiKey);
        StringAssert.Contains(viewModel.CredentialStorageStatus, "loaded");

        await viewModel.DeleteAICredentialCommand.ExecuteAsync(null);

        Assert.IsNull(provider.StoredValue);
        Assert.AreEqual(string.Empty, viewModel.AiApiKey);
        Assert.AreEqual(string.Empty, aiSettings.ApiKey);
        StringAssert.Contains(viewModel.CredentialStorageStatus, "deleted");
    }

    [TestMethod]
    public async Task AIChatCredentialCommands_IsolateKeysByEndpoint()
    {
        FakeCredentialProvider provider = new("test-keyring");
        MainWindowViewModel viewModel = CreateViewModel(
            new CredentialProviderCatalog([provider]),
            new OpenAICompatibleAISettings());
        viewModel.SelectedCredentialProviderId = provider.ProviderId;

        viewModel.AiEndpoint = "https://api.ifm.ai/v1/chat/completions";
        viewModel.AiApiKey = "ifm-key";
        await viewModel.SaveAICredentialCommand.ExecuteAsync(null);

        viewModel.AiEndpoint = "https://api.openai.com/v1/chat/completions";
        Assert.AreEqual(string.Empty, viewModel.AiApiKey);
        await viewModel.LoadAICredentialCommand.ExecuteAsync(null);
        Assert.AreEqual(string.Empty, viewModel.AiApiKey);
        StringAssert.Contains(viewModel.CredentialStorageStatus, "for this endpoint");

        viewModel.AiApiKey = "openai-key";
        await viewModel.SaveAICredentialCommand.ExecuteAsync(null);
        Assert.AreEqual(2, provider.StoredCount);

        viewModel.AiEndpoint = "https://api.ifm.ai/v1/chat/completions";
        viewModel.AiApiKey = string.Empty;
        await viewModel.LoadAICredentialCommand.ExecuteAsync(null);
        Assert.AreEqual("ifm-key", viewModel.AiApiKey);

        viewModel.AiEndpoint = "https://api.openai.com/v1/chat/completions";
        viewModel.AiApiKey = string.Empty;
        await viewModel.LoadAICredentialCommand.ExecuteAsync(null);
        Assert.AreEqual("openai-key", viewModel.AiApiKey);

        viewModel.AiEndpoint = "not-an-http-endpoint";
        await viewModel.DeleteAICredentialCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.CredentialStorageStatus, "valid absolute HTTP(S) endpoint");
        Assert.AreEqual(2, provider.StoredCount);
    }

    [TestMethod]
    public async Task AIChatCredentials_AllowSessionOnlyInputWithoutProvider()
    {
        MainWindowViewModel viewModel = CreateViewModel(new CredentialProviderCatalog([]), new OpenAICompatibleAISettings());
        bool opened = false;
        viewModel.OpenAICredentialsDialogAsync = () =>
        {
            opened = true;
            return Task.CompletedTask;
        };

        await viewModel.OpenAICredentialsCommand.ExecuteAsync(null);
        Assert.IsTrue(opened);
        Assert.AreEqual("None (session only)", viewModel.CredentialProviderDisplay);
        viewModel.AiApiKey = "session-key";
        Assert.IsFalse(viewModel.SaveAICredentialCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task AIChatCredentials_DiscardLateLoadAfterEndpointChange()
    {
        TaskCompletionSource<CredentialSecret?> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ICredentialProvider provider = Substitute.For<ICredentialProvider>();
        provider.ProviderId.Returns("delayed");
        provider.GetStatusAsync(Arg.Any<CancellationToken>()).Returns(
            ValueTask.FromResult(new CredentialProviderStatus("delayed", CredentialProviderAvailability.Available)));
        provider.GetAsync(Arg.Any<CredentialId>(), Arg.Any<CancellationToken>()).Returns(
            _ => new ValueTask<CredentialSecret?>(pending.Task));
        OpenAICompatibleAISettings settings = new();
        MainWindowViewModel viewModel = CreateViewModel(new CredentialProviderCatalog([provider]), settings);
        viewModel.SelectedCredentialProviderId = "delayed";
        viewModel.AiEndpoint = "https://first.example/v1/chat/completions";
        Task load = viewModel.LoadAICredentialCommand.ExecuteAsync(null);

        viewModel.AiEndpoint = "https://second.example/v1/chat/completions";
        viewModel.AiApiKey = "new-endpoint-key";
        pending.SetResult(new CredentialSecret("old-endpoint-key".AsSpan()));
        await load;

        Assert.AreEqual("new-endpoint-key", viewModel.AiApiKey);
        Assert.AreEqual("new-endpoint-key", settings.ApiKey);
        StringAssert.Contains(viewModel.CredentialStorageStatus, "discarded");
    }

    [TestMethod]
    public async Task AIChat_EndpointChangeClearsKeyAndDoesNotForwardPreviousConversation()
    {
        List<AICompletionRequest> requests = [];
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                requests.Add(call.Arg<AICompletionRequest>());
                return Task.FromResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready")));
            });
        OpenAICompatibleAISettings settings = new();
        MainWindowViewModel viewModel = CreateViewModel(
            new CredentialProviderCatalog([]), settings, new AIChatService(client, new AIChatToolCatalog([])));
        viewModel.AiModel = "model";
        viewModel.AiApiKey = "old-key";
        viewModel.AiChatInput = "Private conversation";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);

        viewModel.AiEndpoint = "https://different.example/v1/chat/completions";
        Assert.AreEqual(string.Empty, viewModel.AiApiKey);
        Assert.AreEqual(string.Empty, settings.ApiKey);
        viewModel.AiChatInput = "New conversation";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);

        Assert.AreEqual(2, requests.Count);
        Assert.IsFalse(requests[1].Messages.Any(message => message.Content == "Private conversation"));
        Assert.IsTrue(viewModel.AIChatMessages.Any(message => message.Content.Contains("not be sent")));
    }

    [TestMethod]
    public async Task AIChat_SessionModelIsSentWithoutChangingDefaultAndClearsOnEndpointChange()
    {
        List<AICompletionRequest> requests = [];
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                requests.Add(call.Arg<AICompletionRequest>());
                return Task.FromResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready")));
            });
        OpenAICompatibleAISettings settings = new() { ModelId = "configured" };
        MainWindowViewModel viewModel = CreateViewModel(
            new CredentialProviderCatalog([]), settings, new AIChatService(client, new AIChatToolCatalog([])));
        viewModel.SessionAIModel = " alternate ";
        viewModel.AiChatInput = "First";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);

        Assert.AreEqual("alternate", requests[0].Options.ModelId);
        Assert.AreEqual("configured", settings.ModelId);
        viewModel.SessionAIModel = "  ";
        viewModel.AiChatInput = "Second";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);
        Assert.AreEqual("configured", requests[1].Options.ModelId);

        viewModel.SessionAIModel = "alternate";
        viewModel.AiEndpoint = "https://other.example/chat";
        Assert.AreEqual(string.Empty, viewModel.SessionAIModel);
        Assert.AreEqual("configured", viewModel.EffectiveAIChatModel);
        Assert.AreEqual("configured", settings.ModelId);
    }

    [TestMethod]
    public async Task AIChat_ModelSelectionIsCapturedBeforeAnAsynchronousTurn()
    {
        TaskCompletionSource<AICompletionResponse> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IAIClient client = Substitute.For<IAIClient>();
        AICompletionRequest? captured = null;
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                captured = call.Arg<AICompletionRequest>();
                return completion.Task;
            });
        MainWindowViewModel viewModel = CreateViewModel(
            new CredentialProviderCatalog([]),
            new OpenAICompatibleAISettings { ModelId = "default" },
            new AIChatService(client, new AIChatToolCatalog([])));
        viewModel.SessionAIModel = "first";
        viewModel.AiChatInput = "Hello";
        Task turn = viewModel.SendAIChatMessageCommand.ExecuteAsync(null);
        Assert.IsTrue(viewModel.AiChatBusy);
        viewModel.SessionAIModel = "second";
        Assert.AreEqual("first", captured?.Options.ModelId);
        completion.SetResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready")));
        await turn;
        Assert.IsFalse(viewModel.AiChatBusy);
    }

    [TestMethod]
    public async Task AIChat_CancelDiscardsIncompleteTurnAndPreservesEarlierConversation()
    {
        List<AICompletionRequest> requests = [];
        TaskCompletionSource<AICompletionResponse> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                requests.Add(call.Arg<AICompletionRequest>());
                return requests.Count == 2
                    ? pending.Task
                    : Task.FromResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready")));
            });
        MainWindowViewModel vm = CreateViewModel(new CredentialProviderCatalog([]),
            new OpenAICompatibleAISettings { ModelId = "model" },
            new AIChatService(client, new AIChatToolCatalog([])));
        vm.AiChatInput = "Earlier";
        await vm.SendAIChatMessageCommand.ExecuteAsync(null);
        vm.AiChatInput = "Cancelled question";
        Task turn = vm.SendAIChatMessageCommand.ExecuteAsync(null);
        Assert.IsTrue(vm.AiChatBusy);
        Assert.IsFalse(vm.ClearAIChatCommand.CanExecute(null));
        Assert.IsTrue(vm.SendAIChatMessageCancelCommand.CanExecute(null));
        vm.SendAIChatMessageCancelCommand.Execute(null);
        pending.SetResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Late response")));
        await turn;

        Assert.IsFalse(vm.AiChatBusy);
        Assert.IsTrue(vm.ClearAIChatCommand.CanExecute(null));
        Assert.IsFalse(vm.AIChatMessages.Any(message => message.Content == "Late response"));
        Assert.IsTrue(vm.AIChatMessages.Any(message => message.Content.Contains("may have made changes")));
        vm.AiChatInput = "Next";
        await vm.SendAIChatMessageCommand.ExecuteAsync(null);
        Assert.IsTrue(requests[2].Messages.Any(message => message.Content == "Earlier"));
        Assert.IsFalse(requests[2].Messages.Any(message => message.Content == "Cancelled question"));
    }

    [TestMethod]
    public async Task AIChat_FailedTurnIsNotAutomaticallyRepeated()
    {
        List<AICompletionRequest> requests = [];
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                requests.Add(call.Arg<AICompletionRequest>());
                return requests.Count == 1
                    ? Task.FromException<AICompletionResponse>(new InvalidOperationException("Provider unavailable"))
                    : Task.FromResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready")));
            });
        MainWindowViewModel vm = CreateViewModel(new CredentialProviderCatalog([]),
            new OpenAICompatibleAISettings { ModelId = "model" },
            new AIChatService(client, new AIChatToolCatalog([])));
        vm.AiChatInput = "Failed prompt";
        await vm.SendAIChatMessageCommand.ExecuteAsync(null);
        vm.AiChatInput = "New prompt";
        await vm.SendAIChatMessageCommand.ExecuteAsync(null);

        Assert.IsFalse(requests[1].Messages.Any(message => message.Content == "Failed prompt"));
        Assert.IsTrue(vm.AIChatMessages.Any(message => message.Content.Contains("Provider unavailable")));
        Assert.IsTrue(vm.AIChatMessages.Any(message => message.Content.Contains("may have made changes")));
    }

    [TestMethod]
    public async Task AIChat_EndpointChangeCancelsOutstandingTurn()
    {
        TaskCompletionSource<AICompletionResponse> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>()).Returns(_ => pending.Task);
        MainWindowViewModel vm = CreateViewModel(new CredentialProviderCatalog([]),
            new OpenAICompatibleAISettings { ModelId = "model" },
            new AIChatService(client, new AIChatToolCatalog([])));
        vm.AiChatInput = "Private";
        Task turn = vm.SendAIChatMessageCommand.ExecuteAsync(null);
        vm.AiEndpoint = "https://changed.example/chat";
        pending.SetResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Late response")));
        await turn;
        Assert.IsFalse(vm.AIChatMessages.Any(message => message.Content == "Late response"));
        Assert.IsTrue(vm.AIChatMessages.Any(message => message.Content.Contains("cancelled")));
    }

    [TestMethod]
    public async Task AIChat_WorkspaceChangeDuringContextLoadDoesNotSendOldContext()
    {
        string projectPath = Path.Combine(Path.GetTempPath(), $"ai-workspace-{Guid.NewGuid():N}.csproj");
        File.WriteAllText(projectPath, "<Project />");
        try
        {
            TaskCompletionSource<VersionControlStatus> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
            IVersionControlService versioning = Substitute.For<IVersionControlService>();
            versioning.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(_ => pending.Task);
            IAIClient client = Substitute.For<IAIClient>();
            MainWindowViewModel vm = CreateViewModel(new CredentialProviderCatalog([]),
                new OpenAICompatibleAISettings { ModelId = "model" },
                new AIChatService(client, new AIChatToolCatalog([])), versioning);
            vm.Workspace = new CSharpWorkspaceLoader().Load(projectPath);
            vm.AiChatInput = "Question";
            Task turn = vm.SendAIChatMessageCommand.ExecuteAsync(null);
            vm.Workspace = null;
            pending.SetResult(new VersionControlStatus());
            await turn;

            await client.DidNotReceive().CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>());
            Assert.IsTrue(vm.AIChatMessages.Any(message => message.Content.Contains("cancelled")));
        }
        finally
        {
            File.Delete(projectPath);
        }
    }

    [TestMethod]
    public async Task AIChat_SessionModelAllowsSendingWhenDefaultIsMissing()
    {
        IAIClient client = Substitute.For<IAIClient>();
        client.CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Ready"))));
        OpenAICompatibleAISettings settings = new() { ModelId = string.Empty };
        MainWindowViewModel viewModel = CreateViewModel(
            new CredentialProviderCatalog([]), settings, new AIChatService(client, new AIChatToolCatalog([])));
        viewModel.AiChatInput = "Hello";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);
        await client.DidNotReceive().CompleteAsync(Arg.Any<AICompletionRequest>(), Arg.Any<CancellationToken>());
        Assert.IsTrue(viewModel.AIChatMessages.Any(message => message.Content.Contains("model ID")));

        viewModel.SessionAIModel = "session-model";
        await viewModel.SendAIChatMessageCommand.ExecuteAsync(null);
        await client.Received(1).CompleteAsync(
            Arg.Is<AICompletionRequest>(request => request.Options.ModelId == "session-model"),
            Arg.Any<CancellationToken>());
        Assert.AreEqual(string.Empty, settings.ModelId);
    }

    [TestMethod]
    public async Task AIChatCredentialCommands_SurfaceLockedProviderErrorsWithoutFallback()
    {
        FakeCredentialProvider provider = new("locked-keyring") { IsLocked = true };
        CredentialProviderCatalog catalog = new([provider]);
        MainWindowViewModel viewModel = CreateViewModel(catalog, new OpenAICompatibleAISettings());
        viewModel.SelectedCredentialProviderId = provider.ProviderId;
        viewModel.AiApiKey = "session-key";

        await viewModel.SaveAICredentialCommand.ExecuteAsync(null);

        StringAssert.Contains(viewModel.CredentialStorageStatus, "locked");
        Assert.IsNull(provider.StoredValue);
        Assert.AreEqual("session-key", viewModel.AiApiKey);
    }

    private static MainWindowViewModel CreateViewModel(
        CredentialProviderCatalog catalog,
        OpenAICompatibleAISettings aiSettings,
        AIChatService? chatService = null,
        IVersionControlService? versionControlService = null)
    {
        IPlanningProvider planningProvider = Substitute.For<IPlanningProvider>();
        planningProvider.ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PlanningReadResult()));
        PlanningExplorerViewModel planningExplorer = new(planningProvider);
        return new MainWindowViewModel(
            new TestEditorComponent(),
            planningExplorer,
            new DiagnosticCollectionViewModel(),
            configUi: null,
            projectExplorer: null,
            versionControlService: versionControlService,
            aiChatService: chatService,
            aiSettings: aiSettings,
            credentialProviderCatalog: catalog);
    }

    private sealed class TestEditorComponent : IAvaloniaEditorComponent
    {
        public EditorViewModel EditorViewModel { get; } = new();
        public Control View => null!;
        public IEditorWorkflow Workflow => null!;
    }

    private sealed class FakeCredentialProvider(string providerId) : ICredentialProvider
    {
        private readonly System.Collections.Generic.Dictionary<CredentialId, string> _storedValues = [];

        public string ProviderId { get; } = providerId;
        public bool IsLocked { get; set; }
        public string? StoredValue => _storedValues.Values.SingleOrDefault();
        public int StoredCount => _storedValues.Count;

        public ValueTask<CredentialProviderStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new CredentialProviderStatus(
                ProviderId,
                IsLocked ? CredentialProviderAvailability.Locked : CredentialProviderAvailability.Available));
        }

        public ValueTask<CredentialSecret?> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureUnlocked();
            return ValueTask.FromResult(!_storedValues.TryGetValue(id, out string? storedValue)
                ? null
                : new CredentialSecret(storedValue.AsSpan()));
        }

        public ValueTask SetAsync(
            CredentialId id,
            CredentialSecret secret,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureUnlocked();
            _storedValues[id] = new string(secret.Reveal());
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureUnlocked();
            _storedValues.Remove(id);
            return ValueTask.CompletedTask;
        }

        private void EnsureUnlocked()
        {
            if (IsLocked)
            {
                throw new CredentialProviderLockedException(ProviderId);
            }
        }
    }
}
