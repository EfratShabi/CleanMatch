namespace Clean.Core.DTO
{
    public record AgentToolCallDTO(string Name, string ArgumentsJson, string? ProviderData);

    // One message of the conversation, as it travels between the browser and the API.
    public record AgentMessageDTO(string Role, string Text, AgentToolCallDTO? ToolCall, string? ToolName);

    public record AgentChatReplyDTO(string Text, List<AgentMessageDTO> NewMessages);
}
