using System;

namespace devoft.System
{
    public static class BooleanExtensions
    {
        /// <summary>
        /// Convierte <b>true</b> en <b>1</b> y <b>false</b> en <b>0</b>
        /// </summary>
        /// <param name="value">valor bool a convertir</param>
        /// <returns><b>1</b> si <b>true</b>, <b>0</b> si <b>false</b>.</returns>
        public static int ToBit(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
