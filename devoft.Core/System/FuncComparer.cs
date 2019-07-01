using System;
using System.Collections.Generic;
using System.Text;

namespace devoft.System
{
    public class FuncComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public FuncComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }

    public static class FuncComparerExtensions
    {
        public static FuncComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            return new FuncComparer<T>(equals, getHashCode);
        }
    }

    public static class ComparerExtensions
    {
        public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
        {
            return new ComparisonToComparer<T>(comparison);
        }

        private class ComparisonToComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> _comparison;

            public ComparisonToComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }
        }
    }
}
