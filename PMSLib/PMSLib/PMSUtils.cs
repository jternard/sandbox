using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
//LINQ: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/introduction-to-linq-queries
//DEBUG EXCEL https://stackoverflow.com/questions/24979165/how-can-i-debug-a-c-dll-function-called-from-vba-using-visual-studio
//expose C# to VBA https://msdn.microsoft.com/en-us/library/bb608604.aspx
//rajouter une interface avec COM visible et early binding, et faire en sorte que la classe implemente cette interface en etant cachée
//deploiement C# et dll https://msdn.microsoft.com/en-us/library/ms973843.aspx
namespace PMSLib
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PMSUtils
    {
        private static List<AssetFlatStruct> refAssets = new List<AssetFlatStruct>();
        private static List<AssetFlatStruct> histAssets = new List<AssetFlatStruct>();
        public static List<AssetFlatStruct> Assets = new List<AssetFlatStruct>();
        public static List<AssetFlatStruct> SlicedAssets = new List<AssetFlatStruct>();
        public static List<AssetAggregatedStruct> AggrAssets = new List<AssetAggregatedStruct>();


        public void LoadAssetReferential(object VBinput)
        {
            var VBList = (Array) VBinput; 
            //loop on assets
            for(int i = 0; i < VBList?.GetUpperBound(0); i++)
            {
                AssetFlatStruct myAsset = new AssetFlatStruct();
                myAsset.Code = VBList.GetValue(i + 1,1).ToString();
                myAsset.Name = VBList.GetValue(i + 1, 2)?.ToString();
                myAsset.AssetClass = VBList.GetValue(i + 1, 3)?.ToString();
                myAsset.IndexOrCountry = VBList.GetValue(i + 1, 4)?.ToString();
                myAsset.Sector = VBList.GetValue(i + 1, 5)?.ToString();
                myAsset.Currency = VBList.GetValue(i + 1, 6)?.ToString();
                refAssets.Add(myAsset);
            }
        }

        public void LoadAssetData(object VBinput)
        {
            var VBList = (Array)VBinput;
            //loop on assets
            for (int i = 1; i < VBList?.GetUpperBound(1); i++)
            {
                string code = VBList.GetValue(1, i+1)?.ToString();

                for(int j = 1; j< VBList.GetUpperBound(0);j++)
                {
                    try { 
                        DateTime date = DateTime.Parse(VBList.GetValue(j+1, 1).ToString());
                        AssetFlatStruct myAsset = new AssetFlatStruct();
                        myAsset.Code = code;
                        myAsset.Date = date;
                        myAsset.Value = Double.Parse(VBList.GetValue(j+1, i + 1).ToString());
                        histAssets.Add(myAsset);
                    }
                    catch { Exception err; }
                }
                
            }
        }

        public void Merge()
        {
            var innerJoinQuery = (
                from refA in refAssets
                join histA in histAssets on refA.Code equals histA.Code
                select new AssetFlatStruct { Code = refA.Code, Currency = refA.Currency, AssetClass =refA.Currency, IndexOrCountry = refA.IndexOrCountry,
                                             Name = refA.Name, Sector = refA.Sector, Date = histA.Date, Value = histA.Value}).ToList();
            Assets = innerJoinQuery;
            SlicedAssets = innerJoinQuery;

        }

        public void Append()
        {
            Assets = (List<AssetFlatStruct>)refAssets.Concat(histAssets);
        }

        public void CleanMissingData()
        {
            var correctData = (
                from ass in Assets
                where (!IsXLCVErr(ass.Value, CVErrEnum.ErrNA))
                select ass)
                .ToList();

            Assets = correctData;
            SlicedAssets = correctData;
        }

        enum CVErrEnum : Int32
        {
            ErrDiv0 = -2146826281,
            ErrNA = -2146826246,
            ErrName = -2146826259,
            ErrNull = -2146826288,
            ErrNum = -2146826252,
            ErrRef = -2146826265,
            ErrValue = -2146826273
        }



        private bool IsXLCVErr(object obj, CVErrEnum whichError)
        {
            int parsedObj;
            Int32.TryParse(obj.ToString(), out parsedObj);
            bool output = parsedObj == (Int32)whichError;
            return output;

        }

        public enum CubeDimension
        {
            Code,
            IndexOrCountry,
            AssetClass,
            Sector,
            Currency,
            Time,
            None
        }

        private bool IsDimension(object obj, CubeDimension whichDimension)
        {
            string parsedObj = obj.ToString().Trim().ToLower();
            bool output = parsedObj == whichDimension.ToString().Trim().ToLower();
            return output;
        }

        public void ResetSlices()
        {
            SlicedAssets = Assets;
            AggrAssets.Clear();
        }

        public void Slice(string dimension, string myDimValue)
        {

            //date ref du fichier excel est le 4 septembre 2017
            //pour enlever n jours, il faut donner n en parametres et pas -n
            DateTime refDate = new DateTime(2017, 9, 4);
            if (IsDimension(dimension, CubeDimension.AssetClass))
            {
                SlicedAssets = (
                    from ass in SlicedAssets
                    where ass.AssetClass == myDimValue
                    select ass)
                    .ToList();
            }
            else if (IsDimension(dimension, CubeDimension.Currency))
            {
                SlicedAssets = (
                    from ass in SlicedAssets
                    where ass.Currency == myDimValue
                    select ass)
                    .ToList();
            }
            else if (IsDimension(dimension, CubeDimension.IndexOrCountry))
            {
                SlicedAssets = (
                    from ass in SlicedAssets
                    where ass.IndexOrCountry == myDimValue
                    select ass)
                    .ToList();
            }
            else if (IsDimension(dimension, CubeDimension.Sector))
            {
                SlicedAssets = (
                    from ass in SlicedAssets
                    where ass.Sector == myDimValue
                    select ass)
                    .ToList();
            }
            else if (IsDimension(dimension, CubeDimension.Time))
            {
                SlicedAssets = (
                    from ass in SlicedAssets
                    where ass.Date >= refDate.AddDays(- double.Parse(myDimValue))
                    select ass)
                    .ToList();
            }


        }

        public void CreateAggregates(string dimension)
        {
            DateTime refDate = new DateTime(2017, 9, 4);

            //IEnumerable<IGrouping<string, AssetFlatStruct>> groupQuery;

            if (IsDimension(dimension, CubeDimension.AssetClass)) AggrAssets = (from ass in SlicedAssets
                                                                                group ass by ass.AssetClass into aggrass
                                                                                select new AssetAggregatedStruct
                                                                                {
                                                                                    Dimension = "AssetClass",
                                                                                    Name = aggrass.Key,
                                                                                    Yield = aggrass.Average(a => a.Value)*252,
                                                                                    StDev = aggrass.StdDev(a => a.Value)*Math.Sqrt(252)
                                                                                }).ToList();
            if (IsDimension(dimension, CubeDimension.Currency)) AggrAssets = (from ass in SlicedAssets
                                                                                group ass by ass.Currency into aggrass
                                                                                select new AssetAggregatedStruct
                                                                                {
                                                                                    Dimension = "Currency",
                                                                                    Name = aggrass.Key,
                                                                                    Yield = aggrass.Average(a => a.Value) * 252,
                                                                                    StDev = aggrass.StdDev(a => a.Value) * Math.Sqrt(252)
                                                                                }).ToList();

            if (IsDimension(dimension, CubeDimension.Code)) AggrAssets = (from ass in SlicedAssets
                                                                                group ass by ass.Code into aggrass
                                                                                select new AssetAggregatedStruct
                                                                                {
                                                                                    Dimension = "Code",
                                                                                    Name = aggrass.Key,
                                                                                    Yield = aggrass.Average(a => a.Value) * 252,
                                                                                    StDev = aggrass.StdDev(a => a.Value) * Math.Sqrt(252)
                                                                                }).ToList();

            if (IsDimension(dimension, CubeDimension.IndexOrCountry)) AggrAssets = (from ass in SlicedAssets
                                                                                group ass by ass.IndexOrCountry into aggrass
                                                                                select new AssetAggregatedStruct
                                                                                {
                                                                                    Dimension = "IndexOrCountry",
                                                                                    Name = aggrass.Key,
                                                                                    Yield = aggrass.Average(a => a.Value) * 252,
                                                                                    StDev = aggrass.StdDev(a => a.Value) * Math.Sqrt(252)
                                                                                }).ToList();

            if (IsDimension(dimension, CubeDimension.Sector)) AggrAssets = (from ass in SlicedAssets
                                                                                group ass by ass.Sector into aggrass
                                                                                select new AssetAggregatedStruct
                                                                                {
                                                                                    Dimension = "Sector",
                                                                                    Name = aggrass.Key,
                                                                                    Yield = aggrass.Average(a => a.Value) * 252,
                                                                                    StDev = aggrass.StdDev(a => a.Value) * Math.Sqrt(252)
                                                                                }).ToList();
        }

        //reprendre pour avoir la forme générale qui appelle la forme avec 2 elements
        public object[,] GetCorrel(string Asset1 = "", string Asset2 = "")
        {
            if(Asset1 != "" && Asset2 != "")
            {

               

                List<AssetFlatStruct> myAsset1Data = (from ass in Assets
                                                   where ass.Code == Asset1
                                                   select ass).ToList();

                List<AssetFlatStruct> myAsset2Data = (from ass in Assets
                                                      where ass.Code == Asset2                                                      
                                                      select ass).ToList();

                List<DateTime> sameDates = myAsset1Data.Intersect(myAsset2Data, new ProductComparer()).Select(a => a.Date).ToList();
                myAsset1Data = myAsset1Data.Where(a => a.Date.In(sameDates.ToArray())).ToList();
                myAsset2Data = myAsset2Data.Where(a => a.Date.In(sameDates.ToArray())).ToList();

                


                object[,] VBOutput = new object[2, 2];
                VBOutput[0, 0] = 1;
                VBOutput[1, 1] = 1;

                VBOutput[0, 1] = VBOutput[1,0] = (myAsset1Data.SumProd(myAsset2Data, a => a.Value, b => b.Value) / myAsset1Data.Count() - myAsset1Data.Average(a => a.Value) * myAsset2Data.Average(a => a.Value)) /
                                 (myAsset1Data.StdDev(a => a.Value) * myAsset2Data.StdDev(a => a.Value));

                return VBOutput;
            }
            else
            {
                object[,] VBOutput = new object[refAssets.Count, refAssets.Count];
                for (int i = 0; i < refAssets.Count; i++)
                {
                    for (int j = i; j < refAssets.Count; j++)
                    {
                        if (i != j)
                        {
                            List<AssetFlatStruct> myAsset1Data = (from ass in Assets
                                                                  where ass.Code == refAssets[i].Code
                                                                  select ass).ToList();
                            List<AssetFlatStruct> myAsset2Data = (from ass in Assets
                                                                  where ass.Code == refAssets[j].Code
                                                                  select ass).ToList();

                            List<DateTime> sameDates = myAsset1Data.Intersect(myAsset2Data, new ProductComparer()).Select(a => a.Date).ToList();
                            myAsset1Data = myAsset1Data.Where(a => a.Date.In(sameDates.ToArray())).ToList();
                            myAsset2Data = myAsset2Data.Where(a => a.Date.In(sameDates.ToArray())).ToList();

                            VBOutput[i, j] = VBOutput[j, i] = (myAsset1Data.SumProd(myAsset2Data, a => a.Value, b => b.Value) / myAsset1Data.Count() - myAsset1Data.Average(a => a.Value) * myAsset2Data.Average(a => a.Value)) /
                                (myAsset1Data.StdDev(a => a.Value) * myAsset2Data.StdDev(a => a.Value));
                        }
                        else
                        {
                            VBOutput[i, j] = 1;
                        }
                    }
                }
                return VBOutput;
            }

        }

        public object[,] GetSlicedData(string OrderByDimension = "None")
        {
            //OrderBy Ascending sur les dimensions, il faudrait changer le coder pour pouvoir faire un tri sur les performances

            //doit suivre l'output attendu dans VBA
            //Code, Yield, Filter Yield, Relative
            List<string> CodeList = new List<string>();

            if(IsDimension(OrderByDimension, CubeDimension.None) || IsDimension(OrderByDimension, CubeDimension.Time))
            CodeList = (from ass in SlicedAssets
                                     select ass.Code).Distinct().ToList();

            if(IsDimension(OrderByDimension, CubeDimension.AssetClass))
                CodeList = (from ass in SlicedAssets
                            orderby ass.AssetClass
                            select ass.Code).Distinct().ToList();

            if (IsDimension(OrderByDimension, CubeDimension.Code))
                CodeList = (from ass in SlicedAssets
                            orderby ass.Code
                            select ass.Code).Distinct().ToList();

            if (IsDimension(OrderByDimension, CubeDimension.Currency))
                CodeList = (from ass in SlicedAssets
                            orderby ass.Currency
                            select ass.Code).Distinct().ToList();

            if (IsDimension(OrderByDimension, CubeDimension.IndexOrCountry))
                CodeList = (from ass in SlicedAssets
                            orderby ass.IndexOrCountry
                            select ass.Code).Distinct().ToList();

            if (IsDimension(OrderByDimension, CubeDimension.Sector))
                CodeList = (from ass in SlicedAssets
                            orderby ass.Sector
                            select ass.Code).Distinct().ToList();


            object[,] VBOutput = new object[CodeList.Count,4];

            double averageYield = (from ass in SlicedAssets
                                   select ass.Value).Average() * 252;

            int i = 0;
            foreach(string Code in CodeList)
            {
                VBOutput[i, 0] = Code;

                VBOutput[i,1] = (from ass in SlicedAssets
                                 where ass.Code == Code
                                 select ass.Value).Average() * 252;

                VBOutput[i, 2] = averageYield;

                VBOutput[i, 3] = Double.Parse(VBOutput[i, 1].ToString()) - averageYield;
                i++;
            }

            return VBOutput;
        }

       
    }

    public static class Extensions
    {
        public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values)
        {
            // ref: https://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
            // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/ 
            var mean = 0.0;
            var sum = 0.0;
            var stdDev = 0.0;
            var n = 0;
            foreach (var value in list.Select(values))
            {
                n++;
                var delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdDev = Math.Sqrt(sum / (n - 1));

            return stdDev;

        }

        public static double SumProd<T>(this IEnumerable<T> list1, IEnumerable<T> list2, Func<T, double> values1, Func<T, double> values2)
        {
            
           
            double sum = 0.0;
            int n=0;
            foreach (double value in list1.Select(values1))
            {
                
                sum += value * list2.Select(values2).ToArray()[n];
                n++;
            }
            return sum;

        }

        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }
    }
}
