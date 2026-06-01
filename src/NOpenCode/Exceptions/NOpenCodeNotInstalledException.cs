using System;

namespace NOpenCode
{
    public class NOpenCodeNotInstalledException : NOpenCodeException
    {
        public NOpenCodeNotInstalledException()
            : base("opencode CLI is not installed. Install it with: npm install -g @opencode/cli") { }

        public NOpenCodeNotInstalledException(string message) : base(message) { }
    }
}
