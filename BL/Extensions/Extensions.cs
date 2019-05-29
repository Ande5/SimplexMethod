using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Extensions
{
    internal static class Extensions
    {
        public static string ReplacePointToComma(this string str) 
            => str.Trim().Replace(" ", "").Replace(".", ",");
    }
}
