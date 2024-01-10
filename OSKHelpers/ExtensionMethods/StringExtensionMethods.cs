using OSKHelpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKHelpers.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Restituisce la stringa convertita in ASCII.
        /// </summary>
        /// <param name="text">Testo da convertire in ASCII</param>
        /// <param name="trim">Indica se si desidera il Trim della stringa (default: false)</param>
        /// <param name="length">Lunghezza massima della stringa (default: nessuna)</param>
        /// <param name="padRight">Se la stringa è più breve della lunghezza indicata aggiunge spazi in coda fino alla corretta lunghezza</param>
        /// <returns></returns>
        public static string AsASCII(this string text, bool toUpper = false, bool trim = false, int? length = null, bool padRight = false) => StringUtils.AsASCII(text, toUpper, trim, length, padRight);

    }
}
