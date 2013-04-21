using System;
using System.Windows.Navigation;

namespace RightMyGuide.WindowsPhone
{
    internal class AssociationUriMapper : UriMapperBase
    {

        // Format coming in "rightmyguide:id=11"
        private string _tempUri;
        private const string AssociationUri = "rightmyguide:id=";


        public override Uri MapUri(Uri uri)
        {

            _tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());



            // URI association launch for contoso.

            if (_tempUri.Contains(AssociationUri))
            {

                int categoryIdIndex = _tempUri.IndexOf(AssociationUri) + AssociationUri.Length;

                string id = _tempUri.Substring(categoryIdIndex);



                // Map the show products request to ShowProducts.xaml

                return new Uri("/Views/ShowView.xaml?id=" + id, UriKind.Relative);
            }

            // Otherwise perform normal launch.

            return uri;
        }
    }
}