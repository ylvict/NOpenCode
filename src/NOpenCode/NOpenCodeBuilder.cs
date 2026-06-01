using System;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class NOpenCodeBuilder
    {
        internal string? Model { get; set; }
        internal string? Agent { get; set; }
        internal string? Directory { get; set; }
        internal int? Port { get; set; }
        internal string? Username { get; set; }
        internal string? Password { get; set; }
        internal int StartTimeoutSeconds { get; set; } = 30;
        internal Action<string>? LogCallback { get; set; }

        public NOpenCodeBuilder WithModel(string model)
        {
            Model = model;
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
            return client;
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
}
