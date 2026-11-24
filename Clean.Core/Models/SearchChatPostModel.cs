namespace Clean.Core.Models
{
    public record SearchChatPostModel(string Text, SearchIntent? Known, SearchField? Asked);
}
