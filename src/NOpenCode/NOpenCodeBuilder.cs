using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class NOpenCodeBuilder
    {
        private static readonly Func<ModelInfo, bool> AnyFreeSelector =
            m => m.Id != null && m.Id.EndsWith("-free", StringComparison.OrdinalIgnoreCase);

        internal string? Model { get; set; }
        internal ModelSelectionMode ModelSelection { get; set; } = ModelSelectionMode.AnyFree;
        internal string ModelSelectionProvider { get; set; } = Providers.OpenCode;
        internal Func<ModelInfo, bool>? ModelSelector { get; set; } = AnyFreeSelector;
        internal string? Agent { get; set; }
        internal string? Directory { get; set; }
        internal int? Port { get; set; }
        internal string? Username { get; set; }
        internal string? Password { get; set; }
        internal int StartTimeoutSeconds { get; set; } = 30;
        internal Action<string>? LogCallback { get; set; }

        /// <summary>
        /// Pins the SDK to a specific upstream model id of the form "provider/model".
        /// </summary>
        /// <remarks>
        /// Model ids are owned by the upstream model provider (e.g. opencode) and may
        /// change or disappear without notice when new versions are released.
        /// Hardcoding a specific model id is therefore <b>not recommended for
        /// production code</b>, where an upstream model version bump can break your
        /// application.
        ///
        /// If no With*Model method is called, the SDK defaults to the first free-tier
        /// model exposed by the <see cref="Providers.OpenCode"/> provider, resolved at
        /// <see cref="Launch"/> time. To override that default with a custom predicate,
        /// use <see cref="WithModel(Func{ModelInfo, bool}, string)"/>. To override only
        /// the provider, use <see cref="WithAnyFreeModel(string)"/>. Use the obsolete
        /// string overload only when you deliberately want to pin a specific version
        /// (for example, for reproducible research or short-lived experiments).
        /// </remarks>
        /// <param name="model">A model id such as "opencode/deepseek-v4-flash-free".</param>
        [Obsolete("Direct model ids are owned by the upstream provider and may change " +
                  "or disappear without notice, which can break production. The default " +
                  "behavior already auto-picks a free model at launch time; only call " +
                  "this overload when you deliberately want to pin a specific version.")]
        public NOpenCodeBuilder WithModel(string model)
        {
            Model = model;
            ModelSelection = ModelSelectionMode.Explicit;
            ModelSelector = null;
            return this;
        }

        /// <summary>
        /// Resolves, at launch time, to the first model exposed by the upstream
        /// provider that satisfies <paramref name="selector"/>, and pins the SDK to
        /// that model. This makes the SDK resilient to upstream model versioning
        /// and renames.
        /// </summary>
        /// <remarks>
        /// Resolution happens against the live server during <see cref="Launch"/>;
        /// if no model matches the selector at that moment, <see cref="Launch"/>
        /// throws <see cref="NOpenCodeException"/> with a descriptive message that
        /// includes the ids the server actually exposed.
        /// </remarks>
        /// <param name="selector">Predicate run against every model from
        /// <paramref name="provider"/>. The first match is used.</param>
        /// <param name="provider">The upstream provider to query. Defaults to "opencode".</param>
        public NOpenCodeBuilder WithModel(Func<ModelInfo, bool> selector, string provider = Providers.OpenCode)
        {
            ModelSelector = selector ?? throw new ArgumentNullException(nameof(selector));
            ModelSelectionProvider = provider;
            ModelSelection = ModelSelectionMode.Selector;
            Model = null;
            return this;
        }

        /// <summary>
        /// Overrides the default free-model selection to use a non-default provider.
        /// </summary>
        /// <remarks>
        /// If no With*Model method is called, the SDK already auto-selects the first
        /// free-tier model from the <see cref="Providers.OpenCode"/> provider at
        /// <see cref="Launch"/> time. Use this method only when you need to pull a
        /// free model from a different provider (e.g. <see cref="Providers.Anthropic"/>).
        /// </remarks>
        /// <param name="provider">The upstream provider to query. Defaults to "opencode".</param>
        public NOpenCodeBuilder WithAnyFreeModel(string provider = Providers.OpenCode)
        {
            ModelSelector = AnyFreeSelector;
            ModelSelectionProvider = provider;
            ModelSelection = ModelSelectionMode.AnyFree;
            Model = null;
            return this;
        }

        public NOpenCodeBuilder WithAgent(string agent)
        {
            Agent = agent;
            return this;
        }

        public NOpenCodeBuilder InDirectory(string directory)
        {
            Directory = directory;
            return this;
        }

        public NOpenCodeBuilder OnPort(int port)
        {
            Port = port;
            return this;
        }

        public NOpenCodeBuilder WithAutoServer()
        {
            return this;
        }

        public NOpenCodeBuilder WithCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            return this;
        }

        public NOpenCodeBuilder WithLogging(Action<string> onLog)
        {
            LogCallback = onLog;
            return this;
        }

        public async Task<OpenCodeClient> Launch()
        {
            var options = new ServerOptions
            {
                Port = Port,
                StartTimeoutSeconds = StartTimeoutSeconds
            };

            var server = await ServerManager.Start(options);
            var client = new OpenCodeClient(server, this);

            if (ModelSelection != ModelSelectionMode.Explicit && ModelSelector != null)
                await ResolveSelectorAsync(client);

            return client;
        }

        private async Task ResolveSelectorAsync(OpenCodeClient client)
        {
            var models = await client.Models.List(ModelSelectionProvider);
            var match = models.FirstOrDefault(ModelSelector);
            if (match == null)
            {
                var discovered = models.Count == 0
                    ? "<none>"
                    : string.Join(", ", models.Select(m => $"{m.Provider}/{m.Id}"));
                throw new NOpenCodeException(
                    $"No model exposed by provider \"{ModelSelectionProvider}\" matched " +
                    $"the configured selector. Discovered models: {discovered}.");
            }
            Model = $"{match.Provider}/{match.Id}";
        }

        internal async Task<string> Ask(string prompt)
        {
            await using var client = await Launch();
            var reply = await client
                .Ask(prompt)
                .Execute();
            return reply;
        }
    }

    internal enum ModelSelectionMode
    {
        None = 0,
        Explicit = 1,
        Selector = 2,
        AnyFree = 3,
    }
}
