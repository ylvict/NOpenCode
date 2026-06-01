using System;
using System.Threading.Tasks;

namespace NOpenCode
{
    public static class OpenCode
    {
        public static Task<string> Ask(string prompt)
        {
            var cfg = new NOpenCodeBuilder();
            return cfg.Ask(prompt);
        }

        public static Task<string> Ask(string prompt, Action<NOpenCodeBuilder> configure)
        {
            var cfg = new NOpenCodeBuilder();
            configure(cfg);
            return cfg.Ask(prompt);
        }

        public static NOpenCodeBuilder Configure()
        {
            return new NOpenCodeBuilder();
        }
    }
}
