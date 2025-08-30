namespace PTP.Shared
{
    public enum FieldType
    {
        INT = 0,
        STRING = 1
    }

    public class Schema
    {
        private List<string> _fields = new List<string>();
        private Dictionary<string, FieldInfo> _info = new Dictionary<string, FieldInfo>();

        public Schema()
        {
            
        }

        public void AddField(string fieldName, int type, int length)
        {
            _fields.Add(fieldName);
            _info.Add(fieldName, new FieldInfo(type, length));
        }
        public void AddIntField(string fieldName)
        {
            this.AddField(fieldName, (int)FieldType.INT, 0);
        }
        public void AddStringField(string fieldName, int length)
        {
            this.AddField(fieldName, (int)FieldType.STRING, length);
        }
        public void Add(string fieldName, Schema schema)
        {
            int type = schema.Type(fieldName);
            int length = schema.Length(fieldName);

            this.AddField(fieldName, type, length);
        }
        public void AddAll(Schema schema)
        {
            foreach(var field in schema.Fields())
            {
                this.Add(field, schema);
            }
        }

        public List<string> Fields()
        {
            return _fields;
        }
        public bool HasField(string fieldName)
        {
            return _info.ContainsKey(fieldName);
        }

        public int Type(string fieldName)
        {
            return _info[fieldName].Type;
        }

        public int Length(string fieldName)
        {
            return _info[fieldName].Length;
        }
    }

    internal class FieldInfo
    {
        public int Type { get; }
        public int Length { get; }
        public FieldInfo(int type, int length)
        {
            this.Type = type;
            this.Length = length;
        }
    }
}
