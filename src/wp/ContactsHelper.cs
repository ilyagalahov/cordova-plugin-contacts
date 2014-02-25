/*
	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.UserData;

namespace WPCordovaClassLib.Cordova.Commands
{
    /// <summary>
    /// Implements helper functionality to serialize contact to JSON string.
    /// </summary>
    internal static class ContactsHelper
    {
        public static string ToJson(this Contact contact, string[] desiredFields)
        {
            var contactFieldsWithJsonVals = contact.PopulateContactDictionary();
            if (desiredFields != null && desiredFields.Any())
            {
                return FillResultWithFields(desiredFields, contactFieldsWithJsonVals);
            }
            return FillResultWithFields(contactFieldsWithJsonVals.Keys.ToArray(), contactFieldsWithJsonVals);
        }

        private static string FillResultWithFields(string[] desiredFields, Dictionary<String, String> contactFieldsWithJsonVals)
        {
            var result = new StringBuilder();
            for (int i = 0; i < desiredFields.Count(); i++)
            {
                if (contactFieldsWithJsonVals.ContainsKey(desiredFields[i]))
                {
                    result.Append(contactFieldsWithJsonVals[desiredFields[i]]);
                    if (i != desiredFields.Count() - 1)
                        result.Append(",");
                }
            }
            return "{" + result + "}";
        }
        private static Dictionary<String, String> PopulateContactDictionary(this Contact contact)
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
                    {"phoneNumbers", String.Format("\"phoneNumbers\":[{0}]", FormatJsonPhoneNumbers(contact))},
                    {"emails", String.Format("\"emails\":[{0}]", FormatJsonEmails(contact))},
                    {"addresses", String.Format("\"addresses\":[{0}]", FormatJsonAddresses(contact))},
                    {"urls", String.Format("\"urls\":[{0}]", FormatJsonWebsites(contact))},
                    {"name", String.Format("\"name\":{0}", FormatJsonName(contact))},
                    {"note", String.Format("\"note\":\"{0}\"", EscapeJson(contact.Notes.FirstOrDefault()))},
                    {
                        "birthday", String.Format("\"birthday\":\"{0}\"",
                                                  EscapeJson(Convert.ToString(contact.Birthdays.FirstOrDefault())))
                    },
                };
            return contactFieldsJsonValsDictionary;
        }

        private static string EscapeJson(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Replace("\n", "\\n")
                      .Replace("\r", "\\r")
                      .Replace("\t", "\\t")
                      .Replace("\"", "\\\"")
                      .Replace("&", "\\&");
        }

        private static string FormatJsonPhoneNumbers(Contact con)
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

        private static string FormatJsonName(Contact con)
        {
            string retVal;
            string formatStr = "\"formatted\":\"{0}\"," +
                               "\"familyName\":\"{1}\"," +
                               "\"givenName\":\"{2}\"," +
                               "\"middleName\":\"{3}\"," +
                               "\"honorificPrefix\":\"{4}\"," +
                               "\"honorificSuffix\":\"{5}\"";

            if (con.CompleteName != null)
            {
                retVal = string.Format(formatStr,
                                       EscapeJson(con.CompleteName.FirstName + " " +
                                                                 con.CompleteName.LastName),
                                       // TODO: does this need suffix? middlename?
                                       EscapeJson(con.CompleteName.LastName),
                                       EscapeJson(con.CompleteName.FirstName),
                                       EscapeJson(con.CompleteName.MiddleName),
                                       EscapeJson(con.CompleteName.Title),
                                       EscapeJson(con.CompleteName.Suffix));
            }
            else
            {
                retVal = string.Format(formatStr, "", "", "", "", "", "");
            }

            return "{" + retVal + "}";
        }

        private static string FormatJsonEmails(Contact con)
        {
            string retVal = "";
            string contactFieldFormat = "\"type\":\"{0}\",\"value\":\"{1}\",\"pref\":\"false\"";
            foreach (ContactEmailAddress address in con.EmailAddresses)
            {
                string contactField = string.Format(contactFieldFormat,
                                                    address.Kind.ToString(),
                                                    EscapeJson(address.EmailAddress));

                retVal += "{" + contactField + "},";
            }
            return retVal.TrimEnd(',');
        }

        private static string FormatJsonAddresses(Contact con)
        {
            string retVal = "";
            foreach (ContactAddress address in con.Addresses)
            {
                retVal += GetFormattedJsonAddress(address, false) + ",";
            }

            //Debug.WriteLine("FormatJsonAddresses returning :: " + retVal);
            return retVal.TrimEnd(',');
        }

        private static string FormatJsonWebsites(Contact con)
        {
            string retVal = "";
            foreach (string website in con.Websites)
            {
                retVal += "\"" + EscapeJson(website) + "\",";
            }
            return retVal.TrimEnd(',');
        }


        private static string GetFormattedJsonAddress(ContactAddress address, bool isPrefered)
        {
            string addressFormatString = "\"pref\":{0}," + // bool
                                         "\"type\":\"{1}\"," +
                                         "\"formatted\":\"{2}\"," +
                                         "\"streetAddress\":\"{3}\"," +
                                         "\"locality\":\"{4}\"," +
                                         "\"region\":\"{5}\"," +
                                         "\"postalCode\":\"{6}\"," +
                                         "\"country\":\"{7}\"";

            string formattedAddress = EscapeJson(address.PhysicalAddress.AddressLine1 + " "
                                                                + address.PhysicalAddress.AddressLine2 + " "
                                                                + address.PhysicalAddress.City + " "
                                                                + address.PhysicalAddress.StateProvince + " "
                                                                + address.PhysicalAddress.CountryRegion + " "
                                                                + address.PhysicalAddress.PostalCode);

            string jsonAddress = string.Format(addressFormatString,
                                               isPrefered ? "\"true\"" : "\"false\"",
                                               address.Kind.ToString(),
                                               formattedAddress,
                                               EscapeJson(address.PhysicalAddress.AddressLine1 + " " +
                                                                         address.PhysicalAddress.AddressLine2),
                                               address.PhysicalAddress.City,
                                               address.PhysicalAddress.StateProvince,
                                               address.PhysicalAddress.PostalCode,
                                               address.PhysicalAddress.CountryRegion);

            return "{" + jsonAddress + "}";
        }
    }
}