using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKHelpers.Common
{
    public class StringUtils
    {

        /// <summary>
        /// Restituisce la stringa convertita in ASCII.
        /// </summary>
        /// <param name="text">Testo da convertire in ASCII</param>
        /// <param name="trim">Indica se si desidera il Trim della stringa (default: false)</param>
        /// <param name="length">Lunghezza massima della stringa (default: nessuna)</param>
        /// <param name="padRight">Se la stringa è più breve della lunghezza indicata aggiunge spazi in coda fino alla corretta lunghezza</param>
        /// <returns></returns>
        public static string AsASCII(string text, bool toUpper = false, bool trim = false, int? length = null, bool padRight = false)
        {
            var sb = new StringBuilder();
            if (text != null)
            {
                if (trim)
                {
                    text = text.Trim();
                }
                if (toUpper)
                {
                    text = text.ToUpper();
                }
                foreach (char c in text)
                {
                    int unicode = c;
                    if (unicode < 128)
                    {
                        sb.Append(c);
                    }
                }
            }
            var t = sb.ToString();
            // Verifica della lunghezza della stringa
            int l = t.Length; // lunghezza della stringa convertita ASCII
            if (length != null && length >= 0)
            {
                if (length < l)
                {
                    l = (int)length;
                }
                else if (padRight)
                {
                    l = (int)length;
                    t = t.PadRight(l, ' ');
                }
            }

            t = l > 0 ? t.Substring(0, l) : string.Empty;

            return t;
        }

    }
}
