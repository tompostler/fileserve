namespace Unlimitedinf.Fileserve.Tools
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// A lighter-weight id tracker to replace the heftiness of Guid. 4 billion unique Ids, with 2 billion before
    /// statistically likely collisions built on a backing datatype of 4 bytes.
    /// </summary>
    [JsonConverter(typeof(IdConverter))]
    internal struct Id : IComparable<Id>
    {
        private byte a, b, c, d;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="id"></param>
        public Id(uint id)
        {
            this.a = (byte)((id & 0xFF000000) >> 24);
            this.b = (byte)((id & 0x00FF0000) >> 16);
            this.c = (byte)((id & 0x0000FF00) >> 8);
            this.d = (byte)(id & 0xFF);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Id(byte[] id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length != 4)
                throw new ArgumentException("Invalid length array", nameof(id));

            this.a = id[0];
            this.b = id[1];
            this.c = id[2];
            this.d = id[3];
        }

        /// <summary>
        /// Deserialize ctor.
        /// </summary>
        /// <param name="sid"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public Id(string id) : this(uint.Parse(id.TrimStart('0'), System.Globalization.NumberStyles.HexNumber)) { }

        /// <summary>
        /// Eight lower case characters.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ((uint)this).ToString("x8");
        }

        /// <summary>
        /// Make it easier to work with an Id's formatting.
        /// </summary>
        /// <param name="id"></param>
        public static explicit operator uint(Id id)
        {
            uint res = 0;
            res += (uint)id.a << 24;
            res += (uint)id.b << 16;
            res += (uint)id.c << 8;
            res += id.d;
            return res;
        }

        /// <summary>
        /// Convert a string to Id. Needed for newtonsoft.
        /// </summary>
        /// <param name="id"></param>
        public static explicit operator Id(string id)
        {
            return new Id(id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> structure. Not guaranteed to be unique.
        /// </summary>
        /// <returns></returns>
        public static Id NewId()
        {
            return new Id(GenA.ByteArray(4));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> structure. Will try up to 10 times to be unique from the
        /// given set. If not unique, throws.
        /// </summary>
        /// <param name="ids"></param>
        /// <exception cref="ArgumentException">Thrown if a unique value could not be generated in 10 attempts.</exception>
        /// <returns></returns>
        public static Id NewId(HashSet<Id> ids)
        {
            int retryCount = 10;

            Id id = Id.NewId();
            while (ids.Contains(id) && retryCount-- > 0)
                id = Id.NewId();

            if (ids.Contains(id))
                throw new ArgumentException("Could not generate a unique id.");

            return id;
        }

        /// <summary>
        /// A read-only instance of the <see cref="Id"/> structure whose value is all zeroes.
        /// </summary>
        public static Id Empty = new Id(0);

        int IComparable<Id>.CompareTo(Id other)
        {
            return ((uint)this).CompareTo(other);
        }

        public override bool Equals(object o)
        {
            Id other = (Id)o;

            return this.a == other.a
                && this.b == other.b
                && this.c == other.c
                && this.d == other.d;
        }

        public static bool operator ==(Id left, Id right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Id left, Id right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return ((uint)this).GetHashCode();
        }
    }

    #region IdConverter

    /// <summary>
    /// Enable Json.NET serialization and deserialization.
    /// </summary>
    internal sealed class IdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Id);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Id(serializer.Deserialize<string>(reader));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }

    #endregion
}
