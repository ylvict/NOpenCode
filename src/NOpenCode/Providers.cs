namespace NOpenCode
{
    /// <summary>
    /// Well-known upstream provider ids supported by the opencode CLI.
    /// </summary>
    /// <remarks>
    /// The opencode CLI supports 75+ providers and the list grows over time; only
    /// the most commonly used ids are exposed here as constants to guard against
    /// typos. For any provider not listed, pass the raw id as a string — the
    /// server is the source of truth and you can discover all currently
    /// connected providers at runtime via
    /// <c>OpenCodeClient.Providers.List()</c>.
    /// </remarks>
    public static class Providers
    {
        public const string OpenCode      = "opencode";
        public const string OpenCodeZen   = "opencode-zen";
        public const string Anthropic     = "anthropic";
        public const string OpenAI        = "openai";
        public const string GitHubCopilot = "github-copilot";
        public const string GoogleVertex  = "google-vertex";
        public const string AmazonBedrock = "amazon-bedrock";
        public const string Azure         = "azure";
        public const string DeepSeek      = "deepseek";
        public const string Groq          = "groq";
        public const string OpenRouter    = "openrouter";
        public const string Ollama        = "ollama";
        public const string LMStudio      = "lmstudio";
        public const string Cerebras      = "cerebras";
        public const string Minimax       = "minimax";
        public const string MoonshotAI    = "moonshotai";
        public const string NVIDIA        = "nvidia";
        public const string HuggingFace   = "hugging-face";
        public const string Helicone      = "helicone";
        public const string VercelGateway = "vercel-ai-gateway";
        public const string XAI           = "xai";
    }
}
