using Newtonsoft.Json;
using SFA.DAS.Common.Domain.Json;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SFA.DAS.Common.Domain.Models
{
    public class InvalidVacancyReferenceException : Exception
    {
        public InvalidVacancyReferenceException(string vacancyReference) : base($"The value '{vacancyReference}' is not a valid Vacancy Reference")
        {

        }
    }

    [JsonConverter(typeof(VacancyReferenceJsonConverter))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct VacancyReference : IEquatable<VacancyReference>,
        IEquatable<string>,
        IEquatable<long>
    {
        private bool IsNone => Value == 0;

        public static readonly VacancyReference None = new VacancyReference(string.Empty);
        public long Value { get; }

        public VacancyReference(string value)
        {
            Value = 0L;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            string baseValue = value.ToUpper().Replace("VAC", null);
            if (!long.TryParse(baseValue, out long result) || result < 1)
            {
                throw new InvalidVacancyReferenceException(value);
            }

            Value = result;
        }

        public VacancyReference(long? vacancyReference)
        {
            Value = 0L;
            switch (vacancyReference)
            {
                case null:
                    return;
                default:
                    if (vacancyReference < 1)
                    {
                        throw new InvalidVacancyReferenceException($"{vacancyReference}");
                    }
                    Value = vacancyReference.Value;
                    break;
            }
        }

        public static bool operator ==(VacancyReference? left, VacancyReference? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            return left?.Equals(right) ?? right.Value.Equals(left);
        }

        public static bool operator !=(VacancyReference? left, VacancyReference? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null when IsNone:
                    return true;
                case null:
                    return false;
                case VacancyReference vacancyReference:
                    return Value == vacancyReference.Value;
                case long value:
                    return value == Value;
                case string value:
                    return IsNone && (this.ToString() == value || this.ToShortString() == value);
                default:
                    return false;
            }
        }

        public bool Equals(VacancyReference other)
        {
            return Value == other.Value;
        }

        public bool Equals(long other)
        {
            return Value == other;
        }

        public bool Equals(string other)
        {
            return IsNone && other is null || (this.ToString() == other || this.ToShortString() == other);
        }

        public override string ToString()
        {
            return this == None
                ? string.Empty
                : $"VAC{Value}";
        }

        public string ToShortString()
        {
            return this == None
                ? string.Empty
                : $"{Value}";
        }

        public static implicit operator VacancyReference(long? value)
        {
            return value is null ? None : new VacancyReference(value.Value);
        }

        public static implicit operator VacancyReference(string value)
        {
            return value is null ? None : new VacancyReference(value);
        }

        public static VacancyReference Parse(string value, IFormatProvider provider)
        {
            if (TryParse(value, provider, out var result))
            {
                return result;
            }

            throw new InvalidVacancyReferenceException(value);
        }

        public static bool TryParse(string value, IFormatProvider provider, out VacancyReference result)
        {
            if (value is null)
            {
                result = None;
                return true;
            }

            try
            {
                result = new VacancyReference(value);
                return true;
            }
            catch
            {
                result = None;
                return false;
            }
        }

        public static bool TryParse(string value, out VacancyReference result)
        {
            return TryParse(value, CultureInfo.CurrentCulture, out result);
        }
    }
}
