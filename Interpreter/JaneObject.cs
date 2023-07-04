﻿using System.Numerics;
using System.Runtime.InteropServices.ObjectiveC;

namespace SHJI.Interpreter
{
    internal interface IJaneObject
    {
        public readonly static IJaneObject JANE_ABYSS = new JaneAbyss();
        public readonly static IJaneObject JANE_TRUE = new JaneBool() { Value = true };
        public readonly static IJaneObject JANE_FALSE = new JaneBool() { Value = false };

        public ObjectType Type();
        public string Inspect();

        public string? ToString() => Inspect(); 
    }

    #region Integer Types
    internal struct JaneInt : IJaneObject
    {
        public int Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Int32;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneShort : IJaneObject
    {
        public short Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Int16;
        public readonly string Inspect() => Value.ToString();
    }
    internal struct JaneLong : IJaneObject
    {
        public long Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Int64;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneInt128 : IJaneObject
    {
        public Int128 Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Int128;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneByte : IJaneObject
    {
        public byte Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.UInt8;
        public readonly string Inspect() => Value.ToString();
    }
    internal struct JaneSByte : IJaneObject
    {
        public sbyte Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.Int8;
        public readonly string Inspect() => Value.ToString();
    }
    internal struct JaneUInt : IJaneObject
    {
        public uint Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.UInt32;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneUShort : IJaneObject
    {
        public ushort Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.UInt16;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneULong : IJaneObject
    {
        public ulong Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.UInt64;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneUInt128 : IJaneObject
    {
        public UInt128 Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.UInt128;
        public readonly string Inspect() => Value.ToString();
    }
    #endregion
    #region Floating Point Numbers
    internal struct JaneFloat : IJaneObject
    {
        public float Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Float32;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneDouble : IJaneObject
    {
        public double Value { get; set; }
        public readonly ObjectType Type() => ObjectType.Float64;
        public readonly string Inspect() => Value.ToString();
    }
    #endregion
    #region Other Primitives
    internal struct JaneString : IJaneObject
    {
        public string Value { get; set; }
        public readonly ObjectType Type() => ObjectType.String;
        public readonly string Inspect() => Value;
    }

    internal struct JaneBool : IJaneObject
    {
        public bool Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.Bool;
        public readonly string Inspect() => Value.ToString().ToLower();
    }

    internal struct JaneChar : IJaneObject
    {
        public char Value
        {
            get; set;
        }
        public readonly ObjectType Type() => ObjectType.Char;
        public readonly string Inspect() => Value.ToString();
    }

    internal struct JaneAbyss : IJaneObject
    {
        public readonly ObjectType Type() => ObjectType.Abyss;
        public readonly string Inspect() => "abyss";
    }
    #endregion

    // Also defines precedence of accuracy
    internal enum ObjectType
    {
        Abyss,
        Bool,
        Int8,
        Int16,
        Int32,
        Int64,
        Int128,
        UInt8,
        UInt16,
        UInt32,
        UInt64,
        UInt128,
        Float32,
        Float64,
        Char,
        String,
    }
}
