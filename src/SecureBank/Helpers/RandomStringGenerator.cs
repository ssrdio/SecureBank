using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Helpers
{
    public class RandomStringGenerator
    {
        private const string CHARACTERS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private readonly Random _random;

        public RandomStringGenerator(Random random)
        {
            _random = random;
        }

        public RandomStringGenerator(string seed)
        {
            _random = new Random(GetDeterministicHashCode(seed));
        }

        public string Generate(int lenght = 10)
        {
            return new string(Enumerable
                .Repeat(CHARACTERS, lenght)
                .Select(x => x[_random.Next(CHARACTERS.Length)])
                .ToArray());
        }

        //https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        private static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = (hash1 << 5) + hash1 ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = (hash2 << 5) + hash2 ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
    }
}
