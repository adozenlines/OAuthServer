using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.Mentalis.Security.Tools;

namespace OAuthServer
{
    public class AuthServerHost : IAuthorizationServerHost
    {
        public AutomatedAuthorizationCheckResponse CheckAuthorizeClientCredentialsGrant(DotNetOpenAuth.OAuth2.Messages.IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }

        public AutomatedUserAuthorizationCheckResponse CheckAuthorizeResourceOwnerCredentialGrant(string userName, string password, DotNetOpenAuth.OAuth2.Messages.IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }

        public AccessTokenResult CreateAccessToken(DotNetOpenAuth.OAuth2.Messages.IAccessTokenRequest accessTokenRequestMessage)
        {
            //RSA Key Gen since we don't have certificates.
            RSA rsakey = RSAExponentOfOne.Create();
            RSAParameters privateParam = rsakey.ExportParameters(true);
            RSAParameters publicParam = rsakey.ExportParameters(false);

            RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider();
            RSACryptoServiceProvider publicKey = new RSACryptoServiceProvider();

            privateKey.ImportParameters(privateParam);
            publicKey.ImportParameters(publicParam);

            //-----------------------------------------------------

            var token = new AuthorizationServerAccessToken();

            token.Lifetime = TimeSpan.FromMinutes(10);

            /*
            var signCert = LoadCert(Config.STS_CERT);
            token.AccessTokenSigningKey =
                     (RSACryptoServiceProvider)signCert.PrivateKey;

            var encryptCert = LoadCert(Config.SERVICE_CERT);
            token.ResourceServerEncryptionKey =
                     (RSACryptoServiceProvider)encryptCert.PublicKey.Key;
            */
            token.AccessTokenSigningKey = privateKey;
            token.ResourceServerEncryptionKey = publicKey;

            var result = new AccessTokenResult(token);
            return result;
        }

        private static X509Certificate2 LoadCert(string thumbprint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(
                              X509FindType.FindByThumbprint,
                              thumbprint,
                              validOnly: false);

            if (certs.Count == 0) throw new Exception("Could not find cert");
            var cert = certs[0];
            return cert;
        }


        public DotNetOpenAuth.Messaging.Bindings.ICryptoKeyStore CryptoKeyStore
        {
            get
            {
                return new CryptoKeyStore();
            }
        }

        public IClientDescription GetClient(string clientIdentifier)
        {
            switch (clientIdentifier)
            {

                case "RP":
                    var allowedCallback = "https://localhost/RP/Secure4ownAuthSvr/OAuth";
                    return new ClientDescription(
                                 "data!",
                                 new Uri(allowedCallback),
                                 ClientType.Confidential);
            }

            return null;
        }

        public bool IsAuthorizationValid(DotNetOpenAuth.OAuth2.ChannelElements.IAuthorizationDescription authorization)
        {
            if (authorization.ClientIdentifier == "RP"
                  && authorization.Scope.Count == 1
                  && authorization.Scope.First() == "http://localhost/demo"
                  && authorization.User == "Max Muster")
            {
                return true;
            }

            return false;
        }

        public DotNetOpenAuth.Messaging.Bindings.INonceStore NonceStore
        {
            get { return new DummyNonceStore(); }
        }
    }
}
