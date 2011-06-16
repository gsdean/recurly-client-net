﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;

namespace Recurly
{
    public class RecurlyBillingInfo
    {
        /// <summary>
        /// Account Code or unique ID for the account in Recurly
        /// </summary>
        public string AccountCode { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string IpAddress { get; set; }
        public string PhoneNumber { get; set; }
        public RecurlyCreditCard CreditCard { get; private set; }

        private const string UrlPrefix = "/accounts/";
        private const string UrlPostfix = "/billing_info";

        public RecurlyBillingInfo(string accountCode) : this()
        {
            this.AccountCode = accountCode;
        }

        public RecurlyBillingInfo(RecurlyAccount account) : this()
        {
            this.AccountCode = account.AccountCode;
        }

        private RecurlyBillingInfo()
        {
            this.CreditCard = new RecurlyCreditCard();
        }

        /// <summary>
        /// Lookup a Recurly account's billing info
        /// </summary>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        public static RecurlyBillingInfo Get(string accountCode)
        {
            RecurlyBillingInfo billingInfo = new RecurlyBillingInfo();

            HttpStatusCode statusCode = RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Get,
                BillingInfoUrl(accountCode), 
                new RecurlyClient.ReadXmlDelegate(billingInfo.ReadXml));

            if (statusCode == HttpStatusCode.NotFound)
                return null;

            return billingInfo;
        }

        /// <summary>
        /// Update an account's billing info in Recurly
        /// </summary>
        public void Create()
        {
            Update();
        }

        /// <summary>
        /// Update an account's billing info in Recurly
        /// </summary>
        public void Update()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Put, 
                BillingInfoUrl(this.AccountCode), 
                new RecurlyClient.WriteXmlDelegate(this.WriteXml));
        }

        /// <summary>
        /// Delete an account's billing info.
        /// </summary>
        public void ClearBillingInfo()
        {
            RecurlyClient.PerformRequest(RecurlyClient.HttpRequestMethod.Delete, 
                BillingInfoUrl(this.AccountCode));
        }

        private static string BillingInfoUrl(string accountCode)
        {
            return UrlPrefix + System.Web.HttpUtility.UrlEncode(accountCode) + UrlPostfix;
        }

        internal void ReadXml(XmlTextReader reader)
        {
            while (reader.Read())
            {
                // End of billing_info element, get out of here
                if (reader.Name == "billing_info" && reader.NodeType == XmlNodeType.EndElement)
                    break;

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "account_code":
                            this.AccountCode = reader.ReadElementContentAsString();
                            break;

                        case "first_name":
                            this.FirstName = reader.ReadElementContentAsString();
                            break;

                        case "last_name":
                            this.LastName = reader.ReadElementContentAsString();
                            break;

                        case "address1":
                            this.Address1 = reader.ReadElementContentAsString();
                            break;
                            
                        case "address2":
                            this.Address2 = reader.ReadElementContentAsString();
                            break;

                        case "city":
                            this.City = reader.ReadElementContentAsString();
                            break;

                        case "state":
                            this.State = reader.ReadElementContentAsString();
                            break;

                        case "zip":
                            this.PostalCode = reader.ReadElementContentAsString();
                            break;

                        case "country":
                            this.Country = reader.ReadElementContentAsString();
                            break;

                        case "ip_address":
                            this.IpAddress = reader.ReadElementContentAsString();
                            break;

                        case "phone_number":
                            this.PhoneNumber = reader.ReadElementContentAsString();
                            break;

                        case "credit_card":
                            this.CreditCard = new RecurlyCreditCard();
                            this.CreditCard.ReadXml(reader);
                            break;
                    }
                }
            }
        }

        internal void WriteXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("billing_info"); // Start: billing_info

            xmlWriter.WriteElementString("account_code", this.AccountCode);
            xmlWriter.WriteElementString("first_name", this.FirstName);
            xmlWriter.WriteElementString("last_name", this.LastName);
            xmlWriter.WriteElementString("address1", this.Address1);
            xmlWriter.WriteElementString("address2", this.Address2);
            xmlWriter.WriteElementString("city", this.City);
            xmlWriter.WriteElementString("state", this.State);
            xmlWriter.WriteElementString("zip", this.PostalCode);
            xmlWriter.WriteElementString("country", this.Country);

            if (!String.IsNullOrEmpty(this.IpAddress))
                xmlWriter.WriteElementString("ip_address", this.IpAddress);

            if (this.CreditCard != null)
                this.CreditCard.WriteXml(xmlWriter);

            xmlWriter.WriteEndElement(); // End: billing_info
        }
    }
}
