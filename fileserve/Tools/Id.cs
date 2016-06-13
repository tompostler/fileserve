﻿namespace Unlimitedinf.Fileserve.Tools
{
    using Newtonsoft.Json;
    using System;

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
            this.a = (byte)((id & 0xFF000000) >> 6);
            this.b = (byte)((id & 0x00FF0000) >> 4);
            this.c = (byte)((id & 0x0000FF00) >> 2);
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
                throw new ArgumentException();

            this.a = id[0];
            this.b = id[1];
            this.c = id[2];
            this.d = id[3];
        }

        /// <summary>
        /// Deserialize ctor.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public Id(string id) : this(uint.Parse(id, System.Globalization.NumberStyles.HexNumber)) { }

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
        public static implicit operator uint(Id id)
        {
            uint res = 0;
            res += (uint)(id.a << 6);
            res += (uint)(id.b << 4);
            res += (uint)(id.c << 2);
            res += id.d;
            return res;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Id"/> structure. Not guaranteed to be unique.
        /// </summary>
        /// <returns></returns>
        public static Id NewId()
        {
            string token = GenA.HexToken(8);
            return new Id(token);
        }

        /// <summary>
        /// A read-only instance of the <see cref="Id"/> structure whose value is all zeroes.
        /// </summary>
        public static Id Empty = new Id(0);

        int IComparable<Id>.CompareTo(Id other)
        {
            return ((uint)this).CompareTo(other);
        }
    }

    /// <summary>
    /// Enable Json.NET serialization and deserialization.
    /// </summary>
    internal sealed class IdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Id(serializer.Deserialize<uint>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }
}
