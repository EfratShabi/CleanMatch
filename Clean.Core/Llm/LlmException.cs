namespace Clean.Core.Llm
{
    public class LlmException : Exception
    {
        public bool IsTemporary { get; }

        public LlmException(string message, bool isTemporary, Exception? inner = null)
            : base(message, inner)
        {
            IsTemporary = isTemporary;
        }
    }
}
