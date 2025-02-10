using luizalabs.desafio_tecnico.Interfaces;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using luizalabs.desafio_tecnico.Models.Token;

namespace luizalabs.desafio_tecnico.Logics
{
    public class TokenLogic : ITokenLogic
    {
        private ILogger<TokenLogic> Logger;
        private IConfiguration Configuration;

        public TokenLogic(ILogger<TokenLogic> logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        public string CertificatePrivate { get { return Configuration["Certificate:Private"]; } }
        public string CertificatePublic { get { return Configuration["Certificate:Public"]; } }

        public string CreateToken(string user, DateTime expires)
        {
            var sr = new StreamReader(CertificatePrivate);
            PemReader pr = new PemReader(sr);
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
            //RSAParameters rsaParams = (RSAParameters)pr.ReadObject();

            sr.Close();

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParams);

            var securityKey = new RsaSecurityKey(rsa);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                //iat attribute -- when token was created
                new Claim("iat", ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString()),
                //Scope is the account's claim
                new Claim("issid", user),
                new Claim("name", user),
                new Claim("cert", "/api/v1/Login")
            }, "Custom");



            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                //aud attribute
                Audience = "dani.luizalab.com",
                //iss attribute
                Issuer = user,
                //Subject and scope attribute
                Subject = claimsIdentity,
                //exp attribute -- sempre manda como UTC e verifica em UTC
                Expires = expires,
                //Inform private key to sign
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            //JSON que necessita enviar para o end-point da V3 -- consegue ver o payload
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            //Signed token with private key
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodedToken;
        }

        public string getPublicKey()
        {
            StreamReader sr = new StreamReader(CertificatePublic);
            string retorno = sr.ReadToEnd();
            sr.Close();
            return retorno;
        }

        public Token ValidateToken(string token)
        {
            Token returnValue = new Token();
            if (!string.IsNullOrEmpty(token))
            {
                returnValue.ok = true;
                var handler = new JwtSecurityTokenHandler();
                var tokenData = handler.ReadJwtToken(token);
                var tokenJWT = tokenData.Payload;
                if (tokenJWT != null)
                {
                    returnValue.Expiration = tokenJWT.ValidTo;
                    if (tokenJWT.ValidTo > DateTime.Now)
                    {
                        returnValue.User = tokenJWT["iss"].ToString();
                        returnValue.Id = tokenJWT["issid"].ToString();
                        returnValue.Name = tokenJWT["name"].ToString();

                        string certificado = getPublicKey().Replace("-----BEGIN CERTIFICATE-----\n", string.Empty).Replace("\n-----END CERTIFICATE-----\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

                        if (!string.IsNullOrEmpty(certificado))
                        {
                            returnValue.valid = ValidateWithKeys(token, tokenJWT["aud"].ToString(), tokenJWT["iss"].ToString(), certificado);
                        }
                        else
                        {
                            returnValue.valid = false;
                        }
                    }
                    else
                    {
                        returnValue.valid = false;
                    }

                }
                else
                {
                    returnValue.ok = false;
                }
            }
            else
            {
                returnValue.ok = false;
            }
            return returnValue;
        }

        private bool ValidateWithKeys(string signedAndEncodedToken, string audience, string issuer, string key)
        {

            bool isValid = false;
            try
            {
                if (!isValid)
                {
                    var certificate = new X509Certificate2(Convert.FromBase64String(key));
                    SecurityToken validatedToken = null;

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var rsa = certificate.GetRSAPublicKey();

                    var tokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = audience, //inform tenant
                        ValidIssuer = issuer, //inform identity plataform issuer
                        IssuerSigningKey = new RsaSecurityKey(rsa) //inform public key
                    };

                    try
                    {
                        tokenHandler.ValidateToken(signedAndEncodedToken, tokenValidationParameters, out validatedToken);
                        isValid = true;
                    }
                    catch (SecurityTokenException ex)
                    {
                        //Util.LogError("Erro ao validar o token de acesso do usuário, tenant-id: " + audience + " na plataforma: " + issuer, ex);
                        isValid = false;
                    }
                    catch (Exception ex)
                    {
                        //Util.LogError("Erro ao validar o token de acesso do usuário, tenant-id: " + audience + " na plataforma: " + issuer, ex);
                        isValid = false;
                    }
                }
            }
            catch (SecurityTokenException ex)
            {
                //Util.LogError("Erro ao validar o token de acesso do usuário, tenant-id: " + audience + " na plataforma: " + issuer, ex);
            }
            return isValid;
        }

        private string Salt { get { return Configuration["Certificate:Chave"]; } }

        private byte[] GetHashKey(string hashKey)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            string salt = !string.IsNullOrEmpty(Salt) ? Salt : "I am a nice little salt";
            byte[] saltBytes = encoder.GetBytes(salt);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(hashKey, saltBytes);
            return rfc.GetBytes(16);
        }

        public string Encrypt(string keyString, string dataToEncrypt)
        {
            byte[] key = GetHashKey(keyString);
            // Initialize
            AesManaged encryptor = new AesManaged();
            // Set the key
            encryptor.Key = key;
            encryptor.IV = key;
            // create a memory stream
            using (MemoryStream encryptionStream = new MemoryStream())
            {
                // Create the crypto stream
                using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // Encrypt
                    byte[] utfD1 = Encoding.UTF8.GetBytes(dataToEncrypt);
                    encrypt.Write(utfD1, 0, utfD1.Length);
                    encrypt.FlushFinalBlock();
                    encrypt.Close();
                    // Return the encrypted data
                    return Convert.ToBase64String(encryptionStream.ToArray());
                }
            }
        }
    }
}
