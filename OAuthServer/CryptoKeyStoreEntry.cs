using DotNetOpenAuth.Messaging.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuthServer
{
    public class CryptoKeyStoreEntry
    {
        public string Bucket { get; set; }
        public string Handle { get; set; }
        public CryptoKey Key { get; set; }
    }
}