namespace BD.Common8.Security.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

public abstract class EmbeddedAesDataProtectionProviderBase : IEmbeddedAesDataProtectionProvider
{
    public abstract Aes[]? Aes { get; }

    static byte[] Encrypt(Aes[] aes, byte[] value)
    {
        if (value.Length == 0)
            return value;
        var len = aes.Length - 1;
        var data_e = aes[len].Encrypt(value);
        using var stream = new MemoryStream();
        stream.Write(BitConverter.GetBytes(len));
        stream.Write(data_e);
        var data_r = stream.ToArray();
        return data_r;
    }

    /// <inheritdoc/>
    public byte[]? EncryptStringToBytes(string? value)
    {
        if (value == null)
            return null;
        var aes = Aes;
        if (aes != null && aes.Length != 0)
        {
            var value2 = Encoding.UTF8.GetBytes(value);
            return Encrypt(aes, value2);
        }
        else
            return Encoding.UTF8.GetBytes(value);
    }

    /// <inheritdoc/>
    public byte[]? EncryptBytesToBytes(byte[]? value)
    {
        if (value == null)
            return null;
        var aes = Aes;
        if (aes != null && aes.Length != 0)
        {
            if (value == null)
                return null;
            return Encrypt(aes, value);
        }
        else
            return value;
    }

    static byte[]? Decrypt(Aes[] aes, byte[]? value)
    {
        if (value == null)
            return null;
        if (value.Length == 0)
            return value;
        if (value.Length <= sizeof(int))
            return null;
        var len = BitConverter.ToInt32(value, 0);
        using var transform = aes[len].CreateDecryptor();
        var data_r = transform.TransformFinalBlock(value, sizeof(int), value.Length - sizeof(int));
        return data_r;
    }

    /// <inheritdoc/>
    public string? DecryptBytesToString(byte[]? value)
    {
        if (value == null)
            return null;
        var aes = Aes;
        if (aes != null && aes.Length != 0)
            try
            {
                var data_r = Decrypt(aes, value);
                if (data_r != null)
                    return Encoding.UTF8.GetString(data_r);
                else
                    return null;
            }
            catch
            {
                return null;
            }
        else
            return Encoding.UTF8.GetString(value);
    }

    /// <inheritdoc/>
    public byte[]? DecryptBytesToBytes(byte[]? value)
    {
        if (value == null) return null;
        var aes = Aes;
        if (aes != null && aes.Length != 0)
            try
            {
                var data_r = Decrypt(aes, value);
                return data_r;
            }
            catch
            {
                return null;
            }
        else
            return value;
    }
}