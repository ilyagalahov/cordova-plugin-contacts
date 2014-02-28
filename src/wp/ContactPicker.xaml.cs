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
using System.Windows.Navigation;
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

        private ContactPickerTask.PickResult _result;

        #endregion

        /// <summary>
        ///     Initializes components
        /// </summary>
        public ContactPicker()
        {
            InitializeComponent();
            var cons = new DeviceContacts();
            cons.SearchCompleted +=
                (sender, e) => { lstContacts.ItemsSource = e.Results.ToList(); }
                ;
            cons.SearchAsync(String.Empty, FilterKind.None, "");
        }

        private void LLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _result = new ContactPickerTask.PickResult(TaskResult.OK);
            _result.Contact = e.AddedItems[0] as Contact;
            Completed(this, _result);

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_result == null)
            {
                Completed(this, new ContactPickerTask.PickResult(TaskResult.Cancel));
            }
            base.OnNavigatedFrom(e);
        }
    }
}