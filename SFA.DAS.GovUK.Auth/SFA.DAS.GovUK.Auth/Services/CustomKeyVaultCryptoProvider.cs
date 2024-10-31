using System;
using System.Linq;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.GovUK.Auth.Services;
/// <summary>
/// This is a copy of the KeyVaultCryptoProvider to fix an issue in Microsoft.IdentityModel.JsonWebTokens
/// </summary>
public class CustomKeyVaultCryptoProvider : ICryptoProvider
{
    private readonly CryptoProviderCache _cache;

    public CustomKeyVaultCryptoProvider()
    {
        _cache = new InMemoryCryptoProviderCache();
    }

    internal CryptoProviderCache CryptoProviderCache => _cache;

    public object Create(string algorithm, params object[] args)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw LogHelper.LogArgumentNullException(nameof(algorithm));

        if (args == null)
            throw LogHelper.LogArgumentNullException(nameof(args));

        if (args.FirstOrDefault() is KeyVaultSecurityKey key)
        {
            if (JsonWebKeyEncryptionAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal))
                return new KeyVaultKeyWrapProvider(key, algorithm);
            else if (JsonWebKeySignatureAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal))
            {
                var willCreateSignatures = (bool)(args.Skip(1).FirstOrDefault() ?? false);

                if (_cache.TryGetSignatureProvider(key, algorithm, typeofProvider: key.GetType().ToString(), willCreateSignatures, out var cachedProvider))
                    return cachedProvider;

                var signatureProvider = new KeyVaultSignatureProvider(key, algorithm, willCreateSignatures);
                //if (CryptoProviderFactory.ShouldCacheSignatureProvider(signatureProvider))
                _cache.TryAdd(signatureProvider);

                return signatureProvider;
            }
        }

        throw LogHelper.LogExceptionMessage(new NotSupportedException(LogHelper.FormatInvariant(/*LogMessages.IDX10652*/"", LogHelper.MarkAsNonPII(algorithm))));
    }

    public bool IsSupportedAlgorithm(string algorithm, params object[] args)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw LogHelper.LogArgumentNullException(nameof(algorithm));

        if (args == null)
            throw LogHelper.LogArgumentNullException(nameof(args));

        return args.FirstOrDefault() is KeyVaultSecurityKey
               && (JsonWebKeyEncryptionAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal) || JsonWebKeySignatureAlgorithm.AllAlgorithms.Contains(algorithm, StringComparer.Ordinal));
    }

    public void Release(object cryptoInstance)
    {
        if (cryptoInstance is SignatureProvider signatureProvider)
            _cache.TryRemove(signatureProvider);

        if (cryptoInstance is IDisposable obj)
            obj.Dispose();
    }
}