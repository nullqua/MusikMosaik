namespace app
{
    /// <summary>
    /// A data structure for UI element tags
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="type">The type of code block.</param>
    internal class TagData(Guid id, string type)
    {
        public Guid Id { get; set; } = id;
        public string Type { get; set; } = type;
    }
}
