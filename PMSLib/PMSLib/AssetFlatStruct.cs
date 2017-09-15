using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSLib
{
    public class AssetFlatStruct
    {
        public string Name;
        public string Code;
        public string IndexOrCountry;
        public string Sector;
        public string Currency;
        public string AssetClass;
        public DateTime Date;
        public double Value;

        public List<AssetFlatStruct> ToList()
        {
            List<AssetFlatStruct> output = new List<AssetFlatStruct>();
            output.Add(this);
            return output;
        }

    }

    public class ProductComparer : IEqualityComparer<AssetFlatStruct>
    {

        public bool Equals(AssetFlatStruct x, AssetFlatStruct y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.Date.Equals(y.Date) && x.Date.Equals(y.Date);
        }

        public int GetHashCode(AssetFlatStruct obj)
        {
            //Get hash code for the Name field if it is not null. 
            int hashProductDate = obj.Date == null ? 0 : obj.Date.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductDate;
        }


    }
}
