using System;

namespace BL.Struct
{
    internal struct Elements : IComparable
    {
        public string Element;
        public int ElementValue;

        public Elements(string element, int elementValue)
        {
            Element = element;
            ElementValue = elementValue;
        }

        public int CompareTo(object sender)
        {
            if (sender is Elements elements)
                return CompareTo(elements.ElementValue);
            throw new Exception("Невозможно сравнить два объекта");
        }
    }
}
