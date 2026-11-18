namespace Clean.Core.Llm
{
    public record LlmMessage(string Role, string Text);

    public record LlmReply(string Text, int InputTokens, int OutputTokens);
}
