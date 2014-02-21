using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.UserData;

namespace io.cordova.hellocordova.Plugins.org.apache.cordova.contacts
{
    static class ContactsHelper
    {
        public static Dictionary<String, String> PopulateContactDictionary(this Contact contact)
        {
            var contactFieldsJsonValsDictionary = new Dictionary<string, string>
                    {
                        {"id", String.Format("\"id\":\"{0}\"", contact.GetHashCode())},
                        {"displayName", String.Format("\"displayName\":\"{0}\"", EscapeJson(contact.DisplayName))},
                        {
                            "nickname",
                            String.Format("\"nickname\":\"{0}\"",
                                          EscapeJson(contact.CompleteName != null ? contact.CompleteName.Nickname : ""))
                        },
                        {"phoneNumbers", String.Format("\"phoneNumbers\":[{0}]", FormatJSONPhoneNumbers(contact))},
                        {"emails", String.Format("\"emails\":[{0}]", FormatJSONEmails(contact))},
                        {"addresses", String.Format("\"addresses\":[{0}]", FormatJSONAddresses(contact))},
                        {"urls", String.Format("\"urls\":[{0}]", FormatJSONWebsites(contact))},
                        {"name", String.Format("\"name\":{0}", FormatJSONName(contact))},
                        {"note", String.Format("\"note\":\"{0}\"", EscapeJson(contact.Notes.FirstOrDefault()))},
                        {"birthday", String.Format("\"birthday\":\"{0}\"",
                                          EscapeJson(Convert.ToString(contact.Birthdays.FirstOrDefault())))},
                    };
            return contactFieldsJsonValsDictionary;
        }

        public static string EscapeJson(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"").Replace("&", "\\&");
        }

        public static string FormatJSONPhoneNumbers(Contact con)
        {
            string retVal = "";
            string contactFieldFormat = "\"type\":\"{0}\",\"value\":\"{1}\",\"pref\":\"false\"";
            foreach (ContactPhoneNumber number in con.PhoneNumbers)
            {

                string contactField = string.Format(contactFieldFormat,
                                                    number.Kind.ToString(),
                                                    number.PhoneNumber);

                retVal += "{" + contactField + "},";
            }
            return retVal.TrimEnd(',');
        }

        /*
 *  formatted: The complete name of the contact. (DOMString)
    familyName: The contacts family name. (DOMString)
    givenName: The contacts given name. (DOMString)
    middleName: The contacts middle name. (DOMString)
    honorificPrefix: The contacts prefix (example Mr. or Dr.) (DOMString)
    honorificSuffix: The contacts suffix (example Esq.). (DOMString)
 */
        public static string FormatJSONName(Contact con)
        {
            string retVal = "";
            string formatStr = "\"formatted\":\"{0}\"," +
                                "\"familyName\":\"{1}\"," +
                                "\"givenName\":\"{2}\"," +
                                "\"middleName\":\"{3}\"," +
                                "\"honorificPrefix\":\"{4}\"," +
                                "\"honorificSuffix\":\"{5}\"";

            if (con.CompleteName != null)
            {
                retVal = string.Format(formatStr,
                                   ContactsHelper.EscapeJson(con.CompleteName.FirstName + " " + con.CompleteName.LastName), // TODO: does this need suffix? middlename?
                                   ContactsHelper.EscapeJson(con.CompleteName.LastName),
                                   ContactsHelper.EscapeJson(con.CompleteName.FirstName),
                                   ContactsHelper.EscapeJson(con.CompleteName.MiddleName),
                                   ContactsHelper.EscapeJson(con.CompleteName.Title),
                                   ContactsHelper.EscapeJson(con.CompleteName.Suffix));
            }
            else
            {
                retVal = string.Format(formatStr, "", "", "", "", "", "");
            }

            return "{" + retVal + "}";
        }

        public static string FormatJSONEmails(Contact con)
        {
            string retVal = "";
            string contactFieldFormat = "\"type\":\"{0}\",\"value\":\"{1}\",\"pref\":\"false\"";
            foreach (ContactEmailAddress address in con.EmailAddresses)
            {
                string contactField = string.Format(contactFieldFormat,
                                                    address.Kind.ToString(),
                                                    ContactsHelper.EscapeJson(address.EmailAddress));

                retVal += "{" + contactField + "},";
            }
            return retVal.TrimEnd(',');
        }

        public static string FormatJSONAddresses(Contact con)
        {
            string retVal = "";
            foreach (ContactAddress address in con.Addresses)
            {
                retVal += getFormattedJSONAddress(address, false) + ",";
            }

            //Debug.WriteLine("FormatJSONAddresses returning :: " + retVal);
            return retVal.TrimEnd(',');
        }

        public static string FormatJSONWebsites(Contact con)
        {
            string retVal = "";
            foreach (string website in con.Websites)
            {
                retVal += "\"" + ContactsHelper.EscapeJson(website) + "\",";
            }
            return retVal.TrimEnd(',');
        }


        public static string getFormattedJSONAddress(ContactAddress address, bool isPrefered)
        {

            string addressFormatString = "\"pref\":{0}," + // bool
                          "\"type\":\"{1}\"," +
                          "\"formatted\":\"{2}\"," +
                          "\"streetAddress\":\"{3}\"," +
                          "\"locality\":\"{4}\"," +
                          "\"region\":\"{5}\"," +
                          "\"postalCode\":\"{6}\"," +
                          "\"country\":\"{7}\"";

            string formattedAddress = ContactsHelper.EscapeJson(address.PhysicalAddress.AddressLine1 + " "
                                    + address.PhysicalAddress.AddressLine2 + " "
                                    + address.PhysicalAddress.City + " "
                                    + address.PhysicalAddress.StateProvince + " "
                                    + address.PhysicalAddress.CountryRegion + " "
                                    + address.PhysicalAddress.PostalCode);

            string jsonAddress = string.Format(addressFormatString,
                                               isPrefered ? "\"true\"" : "\"false\"",
                                               address.Kind.ToString(),
                                               formattedAddress,
                                               ContactsHelper.EscapeJson(address.PhysicalAddress.AddressLine1 + " " + address.PhysicalAddress.AddressLine2),
                                               address.PhysicalAddress.City,
                                               address.PhysicalAddress.StateProvince,
                                               address.PhysicalAddress.PostalCode,
                                               address.PhysicalAddress.CountryRegion);

            //Debug.WriteLine("getFormattedJSONAddress returning :: " + jsonAddress);

            return "{" + jsonAddress + "}";
        }





    }
}
