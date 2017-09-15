using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using html = HtmlAgilityPack;
using System.Collections;

namespace VBHelper
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class security
    {        
        public string Name;
        public string Code;
        public string Index;
        public string Sector;
        public string Currency;
        public string AssetType;
        public Dictionary<DateTime, double> historicalClose = new Dictionary<DateTime, double>();

        public security() { }

        public security(string code)
        {
            Code = code;
            //Fetch();
        }

        /*
        public void Fetch()
        {
            //Name = myConstituent.FirstChild.FirstChild.InnerText;
            //mySec.Index = indexName;
            html.HtmlWeb web = new html.HtmlWeb();
            html.HtmlDocument docSecurity = web.Load("https://markets.ft.com/data/equities/tearsheet/historical?s=" + Code);
            html.HtmlNode myTable = docSecurity.DocumentNode.SelectSingleNode("//table[@class='mod-ui-table mod-tearsheet-historical-prices__results mod-ui-table--freeze-pane']");
            foreach (html.HtmlNode myRow in myTable.SelectSingleNode("tbody").SelectNodes("tr"))
            {
                historicalClose.Add(myRow.FirstChild.FirstChild.InnerText, double.Parse(myRow.ChildNodes[4].InnerText));
            }
        }
        */
        public void Print()
        {
            Console.WriteLine("Name;Code;Index;Sector;Date;Price");
            for (int i = 0; i < historicalClose.Count; i++)
            {
                Console.WriteLine(Name + ";" + Code + ";" + Index + ";" + Sector + ";" + historicalClose.Keys.ToList()[i].ToString() + ";" + historicalClose.Values.ToList()[i].ToString());
            }
        }

        public List<double> toYield()
        {
            List<double> historicalPrices = new List<double>(historicalClose.Values.ToList());
            List<double> historicalYield = new List<double>();
            for(int i=0; i < historicalPrices.Count-1; i++)
            {
                if(historicalPrices[i]!=0) historicalYield.Add(historicalPrices[i + 1] / historicalPrices[i] - 1);
            }
            return historicalYield;
        }

        /*
        public Dictionary<string, double> toYieldDictionary()
        {
            Dictionary<string, double> historicalYield = new Dictionary<string, double>();
            for(int i=0;i<historicalClose.Count-1;i++)
            {
                historicalYield.Add(historicalClose.Keys.ToList()[i], toYield()[i]);
            }

            return historicalYield;
        }
        */
        public double Yield()
        {
            return average(toYield()) * 252;
        }

        public double StDev()
        {
            return standardDeviation(toYield()) * Math.Sqrt(252);
        }

        private double average(List<double> array)
        {
            if (array.Count > 0)
            {
                double accu = 0;
                for (int i = 0; i < array.Count; i++)
                {
                    accu += array[i];
                }
                return accu / array.Count;
            }
            else
            {
                return 0;
            }
        }

        private double standardDeviation(List<double> array)
        {
            if(array.Count > 1)
            { 
                double accu = 0;
                for (int i = 0; i < array.Count; i++)
                {
                    accu += Math.Pow(array[i]-average(toYield()),2);
                }
                return Math.Sqrt(accu / (array.Count - 1));
            }
            else
            {
                return 0;
            }
        }
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class securities : IEnumerable
    {
        public int Count;
        private List<security> _securities;
        public security this[int index]
        {
            get { return _securities[index]; }
        }
        public void Add(security item)
        {
            _securities.Add(item);
            Count++;
        }

        public securities()
        {
            _securities = new List<security>();
            Count = _securities.Count;
        }

        public IEnumerator GetEnumerator()
        {
            for(int i = 0; i < this.Count; i++)
            {
                yield return _securities[i];
            }
        }

        
        public List<DateTime> GetDates()
        {
            List<DateTime> dateUnionList = new List<DateTime>(_securities[0].historicalClose.Keys.ToList<DateTime>());
            foreach(security sec in _securities)
            {
                IEnumerable<DateTime> union = dateUnionList.Union(sec.historicalClose.Keys.ToList());
                dateUnionList = union.ToList<DateTime>();
            }

            return dateUnionList;
        }
        
        public void SavePriceToCSV()
        {
            
            using (System.IO.StreamWriter myStream = new System.IO.StreamWriter(@"C:\myCSV_prices_" + System.Guid.NewGuid().ToString() + ".csv"))
            {
                //header
                string header = "Date";
                foreach(security sec in _securities)
                {
                    header += ";" + sec.Code;
                }
                myStream.WriteLine(header);


                //data
                List<DateTime> dateList = GetDates();
                string newLine;
                foreach(DateTime date in dateList)
                {
                    newLine = date.ToString("d", System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"));
                    foreach(security sec in _securities)
                    {
                        newLine += ";";
                        if (sec.historicalClose.ContainsKey(date))
                        {
                            newLine += sec.historicalClose[date].ToString();
                        }
                    }

                    myStream.WriteLine(newLine);
                }

                myStream.Flush();
            }


        }

        public void SaveReferentialToCSV()
        {
            using (System.IO.StreamWriter myStream = new System.IO.StreamWriter(@"C:\myCSV_referential_" + System.Guid.NewGuid().ToString() + ".csv"))
            {
                string header = "RIC;Name;Asset Class;Country/Index;Sector;Currency";
                myStream.WriteLine(header);
                string line;
                foreach (security sec in _securities)
                {
                    line = sec.Code + ";";
                    line += sec.Name + ";";
                    line += sec.AssetType + ";";
                    line += sec.Index + ";";
                    line += sec.Sector + ";";
                    line += sec.Currency;
                    myStream.WriteLine(line);
                }
                myStream.Flush();
            }
        }
    }
}
