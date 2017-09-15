using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using html = HtmlAgilityPack;

namespace VBHelper
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class VBHelper
    {
        private string[] IndexList;
        private string[] SecurityList;
        public securities Securities = new securities();

        public VBHelper(string @ref, string[] list)
        {
            if (@ref.Equals("index")) { IndexList = list; }
            else { SecurityList = list; }
        }

        public VBHelper()
        {
        }

        public void Index(string myIndex)
        {
            IndexList = myIndex.Split(',');
        }

        public void Security(string mySecurity)
        {
            SecurityList = mySecurity.Split(',');
        }

        private string ExceptBlanks(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private DateTime stringToDT(string inputString)
        {
            string[] split = inputString.Split(' ');
            int day = int.Parse(split[0]);
            int year = int.Parse(split[2]);
            int month = 0;
            string moisFR = split[1].Trim().ToLower();
            
            if (moisFR.Contains("ja")) month = 1;
            if (moisFR.Contains("fe")) month = 2;
            if (moisFR.Contains("mar")) month = 3;
            if (moisFR.Contains("av")) month = 4;
            if (moisFR.Contains("mai")) month = 5;
            if (moisFR.Contains("juin")) month = 6;
            if (moisFR.Contains("juil")) month = 7;
            if (moisFR.Contains("ao")) month = 8;
            if (moisFR.Contains("sep")) month = 9;
            if (moisFR.Contains("oct")) month = 10;
            if (moisFR.Contains("nov")) month = 11;
            if (moisFR.Contains("dec")) month = 12;

            DateTime outputDT = new DateTime(year,month,day);
            return outputDT;
        }


        public void FetchData()
        {
            
            html.HtmlWeb web = new html.HtmlWeb();
            html.HtmlDocument docMain = web.Load("https://fr.finance.yahoo.com/indices-mondiaux");
            string myIndexURLCode;
            string myIndexName;
            string mySecURLCode;
            Random rnd = new Random();
            //....docMain.DocumentNode.LastChild.LastChild.FirstChild.FirstChild.FirstChild.FirstChild.FirstChild.ChildNodes[1].
            //FirstChild.FirstChild.ChildNodes[3].FirstChild.FirstChild.FirstChild.ChildNodes[1].FirstChild.FirstChild.LastChild.ChildNodes)


            //INDEX
            foreach (html.HtmlNode MainNode in 
                docMain.DocumentNode.SelectSingleNode("//table[@class='yfinlist-table W(100%) BdB Bdc($tableBorderGray)']").SelectSingleNode("tbody").ChildNodes)
            {

                security mySec = new security();
                
                mySec.Name = MainNode.FirstChild.FirstChild.GetAttributeValue("title", "");
                myIndexName = mySec.Name;
                mySec.AssetType = "Equity Index";
                mySec.Code = MainNode.FirstChild.FirstChild.InnerText;
                myIndexURLCode = MainNode.FirstChild.FirstChild.GetAttributeValue("href", "").Split('=').Last();

                html.HtmlDocument docIndex = web.Load("https://fr.finance.yahoo.com/quote/" + myIndexURLCode + "/history?p=" + myIndexURLCode);
                html.HtmlNode IndexNode = docIndex.DocumentNode.SelectSingleNode("//div[@class='C($c-fuji-grey-j) Fz(12px)']");

                try { 
                    mySec.Currency = IndexNode.FirstChild.InnerText.Substring(IndexNode.FirstChild.InnerText.Length - 3);
                    foreach(html.HtmlNode IndexPrice in docIndex.DocumentNode.SelectSingleNode("//table[@data-test='historical-prices']").SelectSingleNode("tbody").ChildNodes)
                    {
                        try
                        {
                            mySec.historicalClose.Add(stringToDT(IndexPrice.FirstChild.FirstChild.InnerText), Double.Parse(ExceptBlanks(IndexPrice.ChildNodes[5].FirstChild.InnerText.Replace(',', '.'))));
                        }
                        catch(Exception err) { Console.WriteLine(DateTime.Now.ToString() + " : " + err.Message + " ; " + mySec.Name + " ; " + mySec.Code); }
                    }

                    this.Securities.Add(mySec);
                    Console.WriteLine("added " + mySec.Code);
                }
                catch(Exception err) { Console.WriteLine(DateTime.Now.ToString() + " : " + err.Message + " ; " + mySec.Name + " ; " + mySec.Code); }

                System.Threading.Thread.Sleep(rnd.Next(10000));

                //SECURITIES CONSTITUANTS
                try
                {
                    docIndex = web.Load("https://fr.finance.yahoo.com/quote/" + myIndexURLCode + "/components?p=" + myIndexURLCode);
                    foreach(html.HtmlNode ConstituantNode in docIndex.DocumentNode.SelectSingleNode("//table[@class='W(100%) M(0) BdB Bdc($finLightGray)']").SelectSingleNode("tbody").ChildNodes)
                    {
                        try
                        {


                            mySec = new security();
                            mySec.Name = ConstituantNode.FirstChild.FirstChild.GetAttributeValue("title", "");
                            mySec.Code = ConstituantNode.FirstChild.FirstChild.InnerText;
                            mySecURLCode = ConstituantNode.FirstChild.FirstChild.GetAttributeValue("href", "").Split('=').Last();
                            mySec.Index = myIndexName;
                            mySec.AssetType = "Equity";
                            System.Threading.Thread.Sleep(rnd.Next(10000));
                            html.HtmlDocument docSec = web.Load("https://fr.finance.yahoo.com/quote/" + mySecURLCode + "/history?p=" + mySecURLCode);
                            html.HtmlNode SecNode = docSec.DocumentNode.SelectSingleNode("//div[@class='C($c-fuji-grey-j) Fz(12px)']");
                            mySec.Currency = SecNode.FirstChild.InnerText.Substring(SecNode.FirstChild.InnerText.Length - 3);

                            foreach (html.HtmlNode SecPrice in docSec.DocumentNode.SelectSingleNode("//table[@data-test='historical-prices']").SelectSingleNode("tbody").ChildNodes)
                            {
                                try
                                {
                                    mySec.historicalClose.Add(stringToDT(SecPrice.FirstChild.FirstChild.InnerText), Double.Parse(ExceptBlanks(SecPrice.ChildNodes[5].FirstChild.InnerText.Replace(',', '.'))));
                                }
                                catch (Exception err) { }
                            }

                            System.Threading.Thread.Sleep(rnd.Next(10000));
                            docSec = web.Load("https://fr.finance.yahoo.com/quote/" + mySecURLCode + "/profile?p=" + mySecURLCode);
                            SecNode = docSec.DocumentNode.SelectSingleNode("//p[@class='D(ib) Va(t)']");

                            for (int j = 0; j < SecNode.ChildNodes.Count; j++)
                            {
                                if (SecNode.ChildNodes[j].Name == "strong")
                                {
                                    //data-reactid = 23
                                    mySec.Sector = SecNode.ChildNodes[j].InnerText;
                                    break;
                                }
                            }
                            this.Securities.Add(mySec);
                            Console.WriteLine("added " + mySec.Code);
                        }
                        catch(Exception err) { Console.WriteLine(DateTime.Now.ToString() + " : " + err.Message + " ; " + mySec.Name + " ; " + mySec.Code); }
                    }
                }
                catch(Exception err) { Console.WriteLine(DateTime.Now.ToString() + " : " + err.Message + " ; " + mySec.Name + " ; " + mySec.Code); }
            }
            /*
            for (int i = 0; i < IndexList.Count(); i++)
            {
                


                string indexCode = IndexList[i];
                html.HtmlDocument docIndex = web.Load("https://markets.ft.com/data/indices/tearsheet/constituents?s=" + indexCode);
                string indexName = docIndex.DocumentNode.SelectSingleNode("//h1[@class='mod-tearsheet-overview__header__name mod-tearsheet-overview__header__name--large']").InnerText;
                foreach (html.HtmlNode myConstituent in docIndex.DocumentNode.SelectSingleNode("//table[@class='mod-ui-table mod-ui-table--freeze-pane']").SelectSingleNode("tbody").SelectNodes("tr"))
                {
                    security mySec = new security();
                    mySec.Code = myConstituent.FirstChild.LastChild.InnerText;
                    mySec.Name = myConstituent.FirstChild.FirstChild.InnerText;
                    mySec.Index = indexName;

                    html.HtmlDocument docSecurity = web.Load("https://markets.ft.com/data/equities/tearsheet/historical?s=" + mySec.Code);
                    html.HtmlNode myTable = docSecurity.DocumentNode.SelectSingleNode("//table[@class='mod-ui-table mod-tearsheet-historical-prices__results mod-ui-table--freeze-pane']");
                    foreach (html.HtmlNode myRow in myTable.SelectSingleNode("tbody").SelectNodes("tr"))
                    {
                        mySec.historicalClose.Add(myRow.FirstChild.FirstChild.InnerText, double.Parse(myRow.ChildNodes[4].InnerText));
                    }
                    Securities.Add(mySec);
                }
            }

            for (int i = 0; i < SecurityList.Count(); i++)
            {
                security mySec = new security(SecurityList[i]);
                Securities.Add(mySec);
            }
            */
        }
    }

    public class program
    {
        static void Main()
        {
            VBHelper vbh = new VBHelper();
            /*
            DateTime a = DateTime.Now;
            security mysec = new security();
            mysec.Code = "dd";
            mysec.historicalClose.Add(DateTime.Now, 100);
            mysec.historicalClose.Add(a, 103);
            vbh.Securities.Add(mysec);

            mysec = new security();
            mysec.Code = "ee";
            mysec.historicalClose.Add(a, 109);
            mysec.historicalClose.Add(DateTime.Now, 106);
            vbh.Securities.Add(mysec);
            */
            vbh.FetchData();
            vbh.Securities.SaveReferentialToCSV();
            vbh.Securities.SavePriceToCSV();
        }
    }
}

/* Sub d()
Dim a As securities
Dim b As security
Dim c As VBHelper.VBHelper

Set b = New security
b.Code = "SCMN:VTX"
Set a = New securities
a.Add b

For Each b In a
    Debug.Print b.Code
Next b
End Sub
*/
