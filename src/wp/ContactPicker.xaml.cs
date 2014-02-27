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
using System.Linq;
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using DeviceContacts = Microsoft.Phone.UserData.Contacts;

namespace WPCordovaClassLib.Cordova.Commands

{
    public partial class ContactPicker
    {
        #region Constants

        #endregion

        #region Callbacks

        public event EventHandler<ContactPickerTask.PickResult> Completed;

        #endregion

        #region Fields

        #endregion

        /// <summary>
        ///     Initializes components
        /// </summary>
        public ContactPicker()
        {
            InitializeComponent();
            var cons = new DeviceContacts();
            cons.SearchCompleted += Contacts_SearchCompleted;
            cons.SearchAsync(String.Empty, FilterKind.None, "Contacts Test #1");
        }

        private void Contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            lstWebsites.ItemsSource = e.Results.ToList();
        }

        private void LLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContactPickerTask.PickResult result = new ContactPickerTask.PickResult(TaskResult.OK);
            result.Contact = e.AddedItems[0] as Contact;
            Completed(this, result);

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        public class ContactInfo
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }
    }
}