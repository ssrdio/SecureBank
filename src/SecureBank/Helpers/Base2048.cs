using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureBank.Helpers
{
    /// <summary>
    /// Source reference: https://github.com/qntm/base2048/blob/master/src/index.js
    /// </summary>
    public class Base2048
    {
        public Base2048()
        {
            Decompress();
        }
        private const int BITS_PER_CHAR = 11; // Base2048 is an 11-bit encoding
        private const int BITS_PER_BYTE = 8;
        readonly string[] pairStrings = new string[] 
        {
            "8:A[a{ÆÇÐÑØÙÞàæçðñøùþÿĐĒĦĨıĲĸĹŁŃŊŌŒŔŦŨƀƠƢƯƱǄǝǞǤǦǶǸȜȞȠȦȴʰͰʹͶ͸ͻ;Ϳ΀Α΢ΣΪαϊϏϐϗϰϳϴϷϹϺЀЂЃЄЇЈЌЏЙКйкѐђѓєїјќџѶѸ҂ҊӁӃӐӔӖӘӚӠӢӨӪӶӸӺ԰Ա՗աևא׫װ׳ؠآاـفً٠٪ٮٰٱٵٹۀہۂۃۓەۖۮ۽ۿ܀ܐܑܒܰݍަޱ޲߀߫ࠀࠖࡀ࡙ࡠ࡫ࢠࢵࢶࢾऄऩपऱलऴवऺऽाॐ॑ॠॢ०॰ॲঁঅ঍এ঑ও঩প঱ল঳শ঺ঽাৎ৏ৠৢ০৲৴৺ৼ৽ਅ਋ਏ਑ਓ਩ਪ਱ਲਲ਼ਵਸ਼ਸ਺ੜ੝੦ੰੲੵઅ઎એ઒ઓ઩પ઱લ઴વ઺ઽાૐ૑ૠૢ૦૰ૹૺଅ଍ଏ଑ଓ଩ପ଱ଲ଴ଵ଺ଽାୟୢ୦୰ୱ୸ஃ஄அ஋எ஑ஒஔக஖ங஛ஜ஝ஞ஠ண஥ந஫ம஺ௐ௑௦௳అ఍ఎ఑ఒ఩ప఺ఽాౘ౛ౠౢ౦౰౸౿ಀಁಅ಍ಎ಑ಒ಩ಪ಴ವ಺ಽಾೞ೟ೠೢ೦೰ೱೳഅ഍എ഑ഒ഻ഽാൎ൏ൔൗ൘ൢ൦൹ൺ඀අ඗ක඲ඳ඼ල඾ව෇෦෰กัาำเๆ๐๚ກ຃ຄ຅ງຉຊ຋ຍຎດຘນຠມ຤ລ຦ວຨສຬອັາຳຽ຾ເ໅໐໚ໞ໠ༀ༁༠༴ཀགྷང཈ཉཌྷཎདྷནབྷམཛྷཝཀྵཪ཭ྈྍကဦဧါဿ၊ၐၖ",
            "08" 
        };
        readonly Dictionary<int, char[]> lookupE = new Dictionary<int, char[]>();
        readonly Dictionary<char, int[]> lookupD = new Dictionary<char, int[]>();
        public void Decompress()
        {
            for (int i = 0; i < pairStrings.Length; i++)
            {
                List<char> codes = new List<char>();
                for (int j = 0; j < pairStrings[i].Length; j += 2)
                {
                    int first = pairStrings[i][j];
                    int second = pairStrings[i][j + 1];
                    for (int k = first; k < second; k++)
                    {
                        codes.Add((char)k);
                    }
                }
                int numZBits = BITS_PER_CHAR - BITS_PER_BYTE * i;
                lookupE.Add(numZBits, codes.ToArray());
                for (int j = 0; j < codes.Count; j++)
                {
                    lookupD.Add(codes[j],new int[]{ numZBits, j });
                }
            }
        }

        public string Encode(string text)
        {
            int length = text.Length;

            string str = "";
            int z = 0;
            int numZBits = 0;
            for (int i = 0; i < length; i++)
            {
                byte uint8 = Convert.ToByte(text[i]);

                // Take most significant bit first
                for (int j = BITS_PER_BYTE - 1; j >= 0; j--)
                {

                    int bit = (uint8 >> j) & 1;

                    z = (z << 1) + bit;
                    numZBits++;

                    if (numZBits == BITS_PER_CHAR)
                    {
                        str += lookupE[numZBits][z];
                        z = 0;
                        numZBits = 0;
                    }
                }
            }

            if (numZBits != 0)
            {
                // Final bits require special treatment.

                // => Pad `byte` out to 11 bits using 1s, then encode as normal (repertoire 0)
                // => Pad `byte` out to 3 bits using 1s, then encode specially (repertoire 1)
                while (!lookupE.ContainsKey(numZBits))
                {
                    z = (z << 1) + 1;
                    numZBits++;
                }
                str += lookupE[numZBits][z];
            }

            return str;
        }

        public string Decode(string text)
        {

            int length = text.Length;

            // This length is a guess. There's a chance we allocate one more byte here
            // than we actually need. But we can count and slice it off later
            byte[] uint8Array = new byte[(int)Math.Floor(length * BITS_PER_CHAR / (double)BITS_PER_BYTE)];
            int numUint8s = 0;
            int uint8 = 0;
            int numUint8Bits = 0;
            for (int i = 0; i < length; i++)
            {
                char chr = text[i];
                if (!lookupD.ContainsKey(chr))
                {
                    throw new Exception($"Unrecognised Bas2048 character: {chr}");
                }
                int[] lookup = lookupD[chr];
                int numZBits = lookup[0];
                int z = lookup[1];

                if (numZBits != BITS_PER_CHAR && i != length - 1)
                {
                    throw new Exception("Secondary character found before end of input at position ");
                }
                // Take most significant bit first
                for (int j = numZBits - 1; j >= 0; j--)
                {
                    int bit = (z >> j) & 1;
                    uint8 = (uint8 << 1) + bit;
                    numUint8Bits++;


                    if (numUint8Bits == BITS_PER_BYTE)
                    {
                        uint8Array[numUint8s] = (byte)uint8;
                        numUint8s++;
                        uint8 = 0;
                        numUint8Bits = 0;
                    }
                }
            }

            // Final padding bits! Requires special consideration!
            // Remember how we always pad with 1s?
            // Note: there could be 0 such bits, check still works though
            if (uint8 != ((1 << numUint8Bits) - 1))
            {
                throw new Exception("Padding mismatch");
            }
            return Encoding.UTF8.GetString(uint8Array, 0, numUint8s);
            
        }

    }
}
