using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using html = HtmlAgilityPack;

namespace DataRetriever
{
    class Quotation
    {
        static html.HtmlNode myNode;

        private string companyCode;
        private DateTime obsDate;
        private string obsTime;
        private double obsPrice;
        private double referencePrice;
        private DateTime referenceDate;
        private string companyName;

        public Quotation(string CompanyCode)
        {
            companyCode = CompanyCode;
            obsDate = new DateTime();
            obsTime = "";
            obsPrice = 0;
            referencePrice = 0;
            referenceDate = new DateTime();
            companyName = "";
        }
        public Quotation(string CompanyCode, string ObsDate, string ObsTime, double ObsPrice, double ReferencePrice, string ReferenceDate, string CompanyName)
        {
            companyCode = CompanyCode;
            obsDate = stringToDateTime(ObsDate);
            obsTime = ObsTime;
            obsPrice = ObsPrice;
            referencePrice = ReferencePrice;
            referenceDate = stringToDateTime(ReferenceDate);
            companyName = CompanyName;
        }

        public string CompanyCode
        {
            get { return companyCode; }
            set { companyCode = value; }
        }

        public DateTime ObsDate
        {
            get { return obsDate; }
            set { obsDate = value; }
         }

        public string ObsTime
        {
            get { return obsTime; }
            set { obsTime = value; }
            }

        public double ObsPrice
        {
            get { return obsPrice; }
            set { obsPrice = value; }
            }

        public double ReferencePrice
        {
            get { return referencePrice; }
            set { referencePrice = value; }
            }

        public DateTime ReferenceDate
        {
            get { return referenceDate; }
            set { referenceDate = value; }
            }

        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
         }

        //xpath
        public void SetDataFromXPath(html.HtmlDocument doc)
        {
            html.HtmlNode node;

            //hh:mm AMPM
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[6]/span[1]");
            this.ObsTime = node.InnerText.Trim();

            //MM/DD/YYYY
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[3]/span[1]");
            this.ObsDate = stringToDateTime(node.InnerText.Trim().ToLower(), this.ObsTime);

            double price;
            //get the Last price
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[3]/td[2]/span[1]");
            if (Double.TryParse(node.InnerText.Trim().Replace(",", ""), out price)) { this.ObsPrice = price; }

            //referencePrice
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/div[1]/table[1]/tbody[1]/tr[1]/td[6]");
            if (Double.TryParse(node.InnerText.Trim().Replace(",", ""), out price)) { this.ReferencePrice = price; }

            //referenceDate
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/div[1]/table[1]/tbody[1]/tr[1]/td[2]");
            this.ReferenceDate = stringToDateTime(node.InnerText.Trim().ToLower());

            int obsDay = ObsDate.Day;
            int refDay = ReferenceDate.Day;

            if (obsDay == refDay)
            {
                //referencePrice
                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/div[1]/table[1]/tbody[1]/tr[2]/td[6]");
                if (Double.TryParse(node.InnerText.Trim().Replace(",", ""), out price)) { this.ReferencePrice = price; }

                //referenceDate
                node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[7]/tr[1]/td[1]/div[1]/table[1]/tbody[1]/tr[2]/td[2]");
                this.ReferenceDate = stringToDateTime(node.InnerText.Trim().ToLower());
            }

            //companyName
            node = doc.DocumentNode.SelectSingleNode("//html[1]/body[1]/div[1]/div[4]/div[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[3]/span[1]");
            this.CompanyName = node.InnerText.Trim();
        }

        //ref
        public void SetDataFromNodeLabel(html.HtmlDocument doc)
        {

            double price;


            myNode = null;
            SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_lblValueTime");
            this.ObsTime = myNode.InnerText.Trim().ToLower();

            myNode = null;
            SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_lblValueDate");
            this.ObsDate = stringToDateTime(myNode.InnerText.Trim().ToLower(), this.ObsTime);

            myNode = null;
            SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_lblValueLast");
            if (Double.TryParse(myNode.InnerText.Trim().Replace(",", ""), out price)) this.ObsPrice = price;

            myNode = null;
            SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_lblValueName");
            this.CompanyName = myNode.InnerText.Trim();


            myNode = null;
            SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_rgTradingInfo_ctl00__0");
            //ref date = 2nd column of the table
            this.ReferenceDate = stringToDateTime(myNode.ChildNodes[2].InnerText.Trim().ToLower());

            int obsDay = ObsDate.Day;
            int refDay = ReferenceDate.Day;

            if (obsDay == refDay)
            {
                //it's the same day so we need to get the second row in the table
                myNode = null;
                SearchNodeFromNodeID(doc.DocumentNode.ChildNodes, "dnn_ctr950_MainView_rgTradingInfo_ctl00__1");
                //ref date = 2nd column of the table
                this.ReferenceDate = stringToDateTime(myNode.ChildNodes[2].InnerText.Trim().ToLower());
            }
            if (Double.TryParse(myNode.ChildNodes[6].InnerText.Trim().Replace(",", ""), out price)) this.ReferencePrice = price;

        }

        private void SearchNodeFromNodeID(html.HtmlNodeCollection nodeCollection, string nodeID)
        {
            foreach (html.HtmlNode node in nodeCollection)
            {
                if (node.Id.Equals(nodeID))
                {
                    myNode = node;
                    return;
                }
                if (node.HasChildNodes) SearchNodeFromNodeID(node.ChildNodes, nodeID);
            }
        }

        private DateTime stringToDateTime(string refDate, string refTime = "00:00 AM")
        {
            string[] refDateArray;
            DateTime newDate;
            if (refDate.Contains("-"))
            {
                refDateArray = refDate.Split('-');
                //16:00 is closure time of Jakarta stock exchange
                newDate = new DateTime(int.Parse(refDateArray[2]), MonthToInt(refDateArray[1]), int.Parse(refDateArray[0]), 16, 0, 0);
            }
            else
            {
                refDateArray = refDate.Split('/');
                int hour = int.Parse(refTime.Split(':')[0]);
                int minute = int.Parse(refTime.Split(':')[1].Split(' ')[0]);
                if (refTime.Contains("pm")) hour += 12;
                newDate = new DateTime(int.Parse(refDateArray[2]), int.Parse(refDateArray[0]), int.Parse(refDateArray[1]), hour, minute, 0);
            }
            return newDate;
        }

        private int MonthToInt(string refMonth)
        {
            int monthNumber = 0;
            refMonth = refMonth.Trim().ToLower();
            switch (refMonth)
            {
                case "jan":
                    monthNumber = 1;
                    break;
                case "feb":
                    monthNumber = 2;
                    break;
                case "mar":
                    monthNumber = 3;
                    break;
                case "apr":
                    monthNumber = 4;
                    break;
                case "may":
                    monthNumber = 5;
                    break;
                case "jun":
                    monthNumber = 6;
                    break;
                case "jul":
                    monthNumber = 7;
                    break;
                case "aug":
                    monthNumber = 8;
                    break;
                case "sep":
                    monthNumber = 9;
                    break;
                case "oct":
                    monthNumber = 10;
                    break;
                case "nov":
                    monthNumber = 11;
                    break;
                case "dec":
                    monthNumber = 12;
                    break;
                default:
                    break;
            }
            return monthNumber;
        }
    }
}