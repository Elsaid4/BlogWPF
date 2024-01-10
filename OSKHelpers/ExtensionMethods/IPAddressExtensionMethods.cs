using OSKHelpers.Common;
using System.Net;
using System.Text.RegularExpressions;

namespace OSKHelpers.ExtensionMethods
{
    public static class IPAddressExtensionMethods
    {
        /// <summary>
        /// Restituisce, se presente, la quartina x.x.x.x all'interno della stringa.
        /// Il risultato non viene verificato, sarà responsabilità del chiamante procedere in tal senso.
        /// </summary>
        /// <param name="address">Stringa da cui estrarre l'indirizzo</param>
        public static Match Match(this IPAddress ipAddress, string address) => OSKIPAddress.Match(address);
    }
}
