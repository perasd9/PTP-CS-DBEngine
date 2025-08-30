using PTP.Disk;

namespace PTP.Shared
{
    public class Layout
    {
        private readonly Schema _schema;
        private IDictionary<string, int> _offsets;
        private int _slotSize;

        public Layout(Schema schema)
        {
            _schema = schema;
            _offsets = new Dictionary<string, int>();
            int pos = sizeof(int); // First 4B reserved for flag which represents if record is deleted or not

            foreach (var field in schema.Fields())
            {
                _offsets[field] = pos;
                pos += this.LengthInBytes(field);
            }

            _slotSize = pos;
        }

        public Layout(Schema schema, IDictionary<string, int> offsets, int slotSize)
        {
            _schema = schema;
            _offsets = offsets;
            _slotSize = slotSize;
        }

        public Schema Schema => _schema;

        public int Offset(string fieldName)
        {
            return _offsets[fieldName];
        }

        public int SlotSize => _slotSize;
        private int LengthInBytes(string field)
        {
            FieldType type = (FieldType)_schema.Type(field);

            if (type == FieldType.INT) return sizeof(int);
            else return Page.MaxLength(_schema.Length(field));
        }
    }
}
