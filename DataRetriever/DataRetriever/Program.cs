using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using html = HtmlAgilityPack;

namespace DataRetriever
{
    class Program
    {
        static void Main(string[] args)
        {


            List<string> IndexList = new List<string>();
            html.HtmlDocument docIndex = new html.HtmlDocument();
            html.HtmlDocument docSecurity = new html.HtmlDocument();

            try
            {
                html.HtmlWeb web = new html.HtmlWeb();

                IndexList.Add("CAC:PAR");
                IndexList.Add("DAXX:GER");


                for(int i = 0; i < IndexList.Count; i++)
                {
                    string indexCode = IndexList[i];
                    docIndex = web.Load("https://markets.ft.com/data/indices/tearsheet/constituents?s=" + indexCode);
                    string indexName = docIndex.DocumentNode.SelectSingleNode("//h1[@class='mod-tearsheet-overview__header__name mod-tearsheet-overview__header__name--large']").InnerText;

                    foreach(html.HtmlNode myConstituent in docIndex.DocumentNode.SelectSingleNode("//table[@class='mod-ui-table mod-ui-table--freeze-pane']").SelectSingleNode("tbody").SelectNodes("tr"))
                    {
                        security mySec = new security();
                        mySec.Code = myConstituent.FirstChild.LastChild.InnerText;
                        mySec.Name = myConstituent.FirstChild.FirstChild.InnerText;
                        mySec.Index = indexName;

                        docSecurity = web.Load("https://markets.ft.com/data/equities/tearsheet/historical?s=" + mySec.Code);
                        html.HtmlNode myTable = docSecurity.DocumentNode.SelectSingleNode("//table[@class='mod-ui-table mod-tearsheet-historical-prices__results mod-ui-table--freeze-pane']");
                        foreach (html.HtmlNode myRow in myTable.SelectSingleNode("tbody").SelectNodes("tr"))
                        {
                            mySec.historicalClose.Add(myRow.FirstChild.FirstChild.InnerText, double.Parse(myRow.ChildNodes[4].InnerText));
                        }

                        mySec.Print();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured in " + e.Source + " : " + e.Message);
            }

            Console.WriteLine("ok");
            Console.ReadLine();
                    
        }


        //Used to obtain the XPath and names / values of the HTML elements
        //Can be reused in a more generic way by removing the compteur, the output and the tests
        static void parseHTMLLeaves(html.HtmlNodeCollection nodeCollection)
        {
            foreach (html.HtmlNode node in nodeCollection)
            {
                if (node.HasChildNodes)
                {
                    parseHTMLLeaves(node.ChildNodes);
                }
                else
                {
                    //leaf
                    Console.WriteLine(node.XPath + " " + node.Id + node.NodeType.ToString() + " " + node.GetType().ToString() + " " + node.Name + " " + node.InnerText);
                }
            }

        }

        

    }
}