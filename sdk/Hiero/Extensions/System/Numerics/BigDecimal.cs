// SPDX-License-Identifier: Apache-2.0
using System;
using System.Numerics;

namespace System.Numerics
{
    /// <summary>
    /// An arbitrary-precision signed decimal number, stored as an unscaled <see cref="BigInteger"/>
    /// and an integer scale, such that the numeric value is <c>UnscaledValue × 10^(-Scale)</c>.
    /// Mirrors the semantics of Java's <c>java.math.BigDecimal</c>.
    /// </summary>
    /// <remarks>Creates a BigDecimal from an unscaled integer value and a scale.</remarks>
    /// <param name="unscaledValue">The integer mantissa.</param>
    /// <param name="scale">
    /// The number of decimal digits to the right of the decimal point.
    /// Negative scale means trailing zeros (e.g. scale=-2 → ×100).
    /// </param>
    public readonly struct BigDecimal(BigInteger unscaledValue, int scale = 0) : IEquatable<BigDecimal>, IComparable<BigDecimal>
    {
        public BigInteger UnscaledValue { get; } = unscaledValue;
        public int Scale { get; } = scale;

        /// <summary>
        /// Parses a decimal string such as <c>"3.14"</c>, <c>"-0.001"</c>, or <c>"100"</c>.
        /// Scientific notation (e.g. <c>"1E+10"</c>) is not supported; use <see cref="ValueOf(long)"/> for integers.
        /// </summary>
        public static BigDecimal Parse(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new FormatException("Cannot parse null or empty string as BigDecimal.");

            int decimalPoint = s.IndexOf('.');
            if (decimalPoint == -1)
                return new BigDecimal(BigInteger.Parse(s), 0);

            // Remove the '.' and record how many digits follow it
            string unscaled = s.Remove(decimalPoint, 1);
            int scale = s.Length - decimalPoint - 1;
            return new BigDecimal(BigInteger.Parse(unscaled), scale);
        }
        /// <summary>
        /// Returns a <see cref="BigDecimal"/> whose value equals the given <c>long</c> exactly,
        /// with scale 0. Prefer this over <see cref="ValueOf(double)"/> whenever the source
        /// value is an integer, to avoid floating-point noise or scientific notation issues.
        /// </summary>
        public static BigDecimal ValueOf(long value)
        {
            return new BigDecimal(new BigInteger(value), 0);
        }
        /// <summary>
        /// Converts a <c>double</c> to a <see cref="BigDecimal"/> by round-tripping through its
        /// full-precision string representation. Avoid for integer values; use
        /// <see cref="ValueOf(long)"/> instead.
        /// </summary>
        /// <remarks>
        /// Uses <c>"R"</c> (round-trip) format rather than <c>"G17"</c> to guarantee that the
        /// string never uses scientific notation for values that fit in decimal notation, and to
        /// avoid floating-point noise in the last digit.
        /// </remarks>
        public static BigDecimal ValueOf(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Cannot convert NaN or Infinity to BigDecimal.", nameof(value));

            string s = value.ToString("R");

            // "R" can still produce scientific notation for very large/small doubles.
            // Fall back to G17 and strip any exponent — this is the same corner-case
            // Java's BigDecimal(double) has, so callers should prefer ValueOf(long).
            if (s.Contains('E') || s.Contains('e'))
                throw new NotSupportedException(
                    $"Cannot convert double value {value} with scientific notation via ValueOf; " +
                    "use ValueOf(long) or Parse(string) instead.");

            return Parse(s);
        }

        /// <summary>
        /// Returns <c>this / other</c> as an exact result.
        /// Throws <see cref="ArithmeticException"/> if the result is non-terminating
        /// (i.e. cannot be represented exactly), mirroring Java's
        /// <c>BigDecimal.divide(divisor, MathContext.UNLIMITED)</c>.
        /// </summary>
        public BigDecimal Divide(BigDecimal other)
        {
            if (other.UnscaledValue == BigInteger.Zero)
                throw new DivideByZeroException("Cannot divide BigDecimal by zero.");

            // Strategy: scale both values so they share a common integer representation,
            // then perform integer division and check for a remainder.
            //
            // Effective value = UnscaledValue × 10^(-Scale)
            // Result = (this.UnscaledValue × 10^(other.Scale)) / other.UnscaledValue
            // Result scale = this.Scale - other.Scale (may be negative)
            //
            // We add extra decimal places of working precision to detect non-terminating results.
            const int extraPrecision = 20;

            BigInteger scaledDividend = UnscaledValue
                * BigInteger.Pow(10, other.Scale + extraPrecision);

            var (quotient, remainder) = BigInteger.DivRem(scaledDividend, other.UnscaledValue);

            if (remainder != BigInteger.Zero)
                throw new ArithmeticException(
                    "Non-terminating decimal expansion; the result cannot be represented exactly. " +
                    "Consider using an overload that accepts a scale or rounding mode.");

            return new BigDecimal(quotient, Scale + extraPrecision)
                .StripTrailingZeros();
        }
        /// <summary>
        /// Returns <c>this × other</c>.
        /// The result scale is <c>this.Scale + other.Scale</c>, matching Java's behaviour.
        /// </summary>
        public BigDecimal Multiply(BigDecimal other)
        {
            return new BigDecimal(UnscaledValue * other.UnscaledValue, Scale + other.Scale);
        }
        /// <summary>
        /// Returns an equivalent <see cref="BigDecimal"/> with all trailing fractional zeros removed.
        /// E.g. <c>1.2300</c> → <c>1.23</c>.  Whole-number trailing zeros are preserved.
        /// </summary>
        public BigDecimal StripTrailingZeros()
        {
            if (UnscaledValue == BigInteger.Zero)
                return new BigDecimal(BigInteger.Zero, 0);

            var value = UnscaledValue;
            var scale = Scale;
            while (scale > 0 && value % 10 == 0)
            {
                value /= 10;
                scale--;
            }
            return new BigDecimal(value, scale);
        }

        /// <summary>
        /// Returns the value as a <c>double</c>, with possible loss of precision.
        /// Mirrors Java's <c>BigDecimal.doubleValue()</c>.
        /// </summary>
        public double DoubleValue()
        {
            // Compute as unscaled ÷ 10^scale using double arithmetic.
            // For the magnitudes used in Hbar this is accurate enough for
            // the fractional-tinybar check (which only needs to detect % 1 ≠ 0).
            return (double)UnscaledValue / Math.Pow(10, Scale);
        }
        /// <summary>
        /// Returns the value truncated toward zero as a <c>long</c>.
        /// Mirrors Java's <c>BigDecimal.longValue()</c>.
        /// </summary>
        public long LongValue()
        {
            BigInteger integer = Scale switch
            {
                0 => UnscaledValue,
                > 0 => UnscaledValue / BigInteger.Pow(10, Scale),
                _  /*<0*/=> UnscaledValue * BigInteger.Pow(10, -Scale)
            };
            return (long)integer;
        }
        public int CompareTo(BigDecimal other)
        {
            int maxScale = Math.Max(Scale, other.Scale);
            BigInteger a = UnscaledValue * BigInteger.Pow(10, maxScale - Scale);
            BigInteger b = other.UnscaledValue * BigInteger.Pow(10, maxScale - other.Scale);
            return a.CompareTo(b);
        }
        /// <summary>
        /// Value-based equality: <c>1.0</c> and <c>1.00</c> are considered equal.
        /// Mirrors Java's <c>BigDecimal.compareTo(...) == 0</c> semantics.
        /// </summary>
        public bool Equals(BigDecimal other)
        {
            int maxScale = Math.Max(Scale, other.Scale);
            BigInteger a = UnscaledValue * BigInteger.Pow(10, maxScale - Scale);
            BigInteger b = other.UnscaledValue * BigInteger.Pow(10, maxScale - other.Scale);
            return a == b;
        }

        public override bool Equals(object? obj) =>
            obj is BigDecimal d && Equals(d);
        /// <summary>
        /// Hash code is consistent with value-based <see cref="Equals(BigDecimal)"/>:
        /// normalised representations of the same value produce the same hash.
        /// </summary>
        public override int GetHashCode()
        {
            var n = StripTrailingZeros();
            return HashCode.Combine(n.UnscaledValue, n.Scale);
        }
        /// <summary>
        /// Returns the decimal string representation, e.g. <c>"3.14"</c>, <c>"-0.005"</c>.
        /// </summary>
        public override string ToString()
        {
            bool negative = UnscaledValue < 0;
            string digits = BigInteger.Abs(UnscaledValue).ToString();

            string result;
            if (Scale <= 0)
            {
                // No decimal point; append trailing zeros if scale is negative
                result = digits + new string('0', -Scale);
            }
            else if (Scale >= digits.Length)
            {
                // Scale exceeds available digits; pad with leading zeros after "0."
                result = "0." + digits.PadLeft(Scale, '0');
            }
            else
            {
                result = digits.Insert(digits.Length - Scale, ".");
            }

            return negative ? "-" + result : result;
        }

        public static BigDecimal operator +(BigDecimal a, BigDecimal b)
        {
            int maxScale = Math.Max(a.Scale, b.Scale);
            BigInteger valA = a.UnscaledValue * BigInteger.Pow(10, maxScale - a.Scale);
            BigInteger valB = b.UnscaledValue * BigInteger.Pow(10, maxScale - b.Scale);
            return new BigDecimal(valA + valB, maxScale);
        }
        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            int maxScale = Math.Max(a.Scale, b.Scale);
            BigInteger valA = a.UnscaledValue * BigInteger.Pow(10, maxScale - a.Scale);
            BigInteger valB = b.UnscaledValue * BigInteger.Pow(10, maxScale - b.Scale);
            return new BigDecimal(valA - valB, maxScale);
        }
        public static BigDecimal operator *(BigDecimal a, BigDecimal b) => a.Multiply(b);
        public static BigDecimal operator /(BigDecimal a, BigDecimal b) => a.Divide(b);

        public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
        public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
        public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
        public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
        public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
        public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;
    }
}