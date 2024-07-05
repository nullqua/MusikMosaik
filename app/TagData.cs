namespace app
{
    internal class TagData(Guid id, string type)
    {
        public Guid Id { get; set; } = id;
        public string Type { get; set; } = type;
    }
}
