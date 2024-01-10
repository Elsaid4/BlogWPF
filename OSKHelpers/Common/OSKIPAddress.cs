using System.Net;
using System.Text.RegularExpressions;

namespace OSKHelpers.Common
{
    public class OSKIPAddress : IPAddress
    {
        #region Costanti

        private const string IPMASK = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";

        #endregion

        #region Costruttori

        public OSKIPAddress(byte[] address) : base(address)
        {
        }

        public OSKIPAddress(long newAddress) : base(newAddress)
        {
        }

        public OSKIPAddress(byte[] address, long scopeid) : base(address, scopeid)
        {
        }

        #endregion

        #region Metodi

        /// <summary>
        /// Restituisce, se presente, la quartina x.x.x.x all'interno della stringa.
        /// Il risultato non viene verificato, sarà responsabilità del chiamante procedere in tal senso.
        /// </summary>
        /// <param name="address">Stringa da cui estrarre l'indirizzo</param>
        public static Match Match(string address) => Regex.Match(address ?? string.Empty, IPMASK);

        #endregion
    }
}