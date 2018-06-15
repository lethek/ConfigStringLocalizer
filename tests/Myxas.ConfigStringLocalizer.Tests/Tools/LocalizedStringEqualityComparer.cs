using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;


namespace Myxas.ConfigStringLocalizer.Tools
{

    public class LocalizedStringEqualityComparer : IEqualityComparer<LocalizedString>
    {

        public bool Equals(LocalizedString x, LocalizedString y)
        {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            return String.Equals(x.Name, y.Name)
                && String.Equals(x.Value, y.Value)
                && x.ResourceNotFound == y.ResourceNotFound;
        }


        public int GetHashCode(LocalizedString obj)
        {
            int hash = 14;
            hash = 92821 * hash + (obj.Name ?? "").GetHashCode();
            hash = 92821 * hash + (obj.Value ?? "").GetHashCode();
            hash = 92821 * hash + obj.ResourceNotFound.GetHashCode();
            return hash;
        }
    }

}
