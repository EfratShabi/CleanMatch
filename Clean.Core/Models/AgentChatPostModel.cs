using Clean.Core.DTO;

namespace Clean.Core.Models
{
    // History: the messages the browser got back last time (the server keeps no memory between requests).
    public record AgentChatPostModel(string Text, List<AgentMessageDTO>? History);
}
