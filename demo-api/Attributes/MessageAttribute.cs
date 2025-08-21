namespace demo_api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageAttribute : Attribute
    {
        public string Text { get; }
        public MessageAttribute(string text)
        {
            Text = text;
        }
    }

}
