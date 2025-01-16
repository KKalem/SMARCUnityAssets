namespace SmarcGUI
{
    public interface IJsonSerializable
    {
        // Why? Because there are some keys in wara that are not allowed as c# variable names
        // So we need to use a dictionary to store the fields and then jsonify them...
        // We might also need more fields in the c# version than the JSON version
        // since types are a thing here and not in c#.
        // So we make it all explicit.
        public string ToJson();
    }
}
