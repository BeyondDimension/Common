namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="RSA"/> 的单元测试
/// </summary>
public sealed class RSATest
{
    const int keySizeInBits = 4096;
    const string text = "a你好，Здравствыйте，こんにちは 안녕하세요.😋✅☺️📄💜😂❌😁🔫🛀";

    static void TestRSA(RSA rsa, string ciphertext)
    {
        var plaintext = rsa.Decrypt(ciphertext);
        Assert.That(plaintext, Is.EqualTo(text));
    }

    /// <summary>
    /// 测试 RSA 密钥 Json 字符串的格式
    /// </summary>
    [Test]
    public void RSA_Json()
    {
        string privateKey = null!;
        string ciphertext = null!;
        {
            using var rsa = RSA.Create(keySizeInBits);
            privateKey = rsa.ToJsonString(true);
            ciphertext = rsa.Encrypt(text);
        }
        TestContext.WriteLine($"ByteCount: {Encoding.UTF8.GetByteCount(privateKey)}");
        {
            using var rsa = RSAUtils.GetRSAParametersFromJsonString(privateKey).Create();
            TestRSA(rsa, ciphertext);
        }
    }

    /// <summary>
    /// 测试 RSA 密钥 MemoryPack byte[] 的格式
    /// </summary>
    [Test]
    public void RSA_MP2()
    {
        MemoryPackFormatterProvider.Register(RSAParametersFormatterAttribute.Formatter.Default);
        byte[] privateKey = null!;
        string ciphertext = null!;
        {
            using var rsa = RSA.Create(keySizeInBits);
            privateKey = Serializable.SMP2(rsa.ExportParameters(true));
            ciphertext = rsa.Encrypt(text);
        }
        TestContext.WriteLine($"ByteCount: {privateKey.Length}");
        {
            using var rsa = Serializable.DMP2<RSAParameters>(privateKey).Create();
            TestRSA(rsa, ciphertext);
        }
    }

    /// <summary>
    /// 测试 RSA 密钥 JsonWebKey 的格式
    /// </summary>
    [Test]
    public void JSONWebKey()
    {
        //using var rsa2 = RSAUtils.CreateFromJsonString(RSAPrivateKey);
        //var result = rsa2.Decrypt("YJ_2m2vk4N1uhZDRkwdMPTq1ttz7pmh4Vov_mVbO1Cft19SdtTFRSAGe3WHFhrltswQJmX3wXUuVyTDLb7QUlHnw0K407ufvooP81HLQwlHNFT_UNYLHpYQAZkYyYvPOKJJEKIu4xhwKffDdgJNr5InaDiemPelNevG0doDtEx6ok6ffEi3mS6495q5KhhECs3i1reLXhHGrLTgs6xeJymlXLqEWSA4srhtEQ1o6wx-MgWsvlr9UVWzRgWuklXRem6jyb7jz5KDneWjBvrL-VTKPo7Fuzmdumage_9pSYDSbiRzYLx5NiLYRFKYS2KVTSvcFGPyW0vMySe_5t9IJA44Hoj-90-hBd0kBl9fBq3H_JsxZDF52Xc1NSkV4arpPnz-SuvoBwYsvu5C0oNPtmJIig715lCa6MPKzzw6ge8xf0FagdONqWoKpMW0NDuP-SJAMZ90THi4QpTkoCuthCJaifU3mhFKMRzyctVtHQ2J8St1t1s9AcB7-tMKy-gtdOSb_8UJs07rdCYL9jjpHZQF9xFhv7VKpI20BFO-f56qSNsf-D_SDvNNfSDYecMKD5kgJkU_SDcrl9bktP1LRwm2pwRLV199wLFWWOgMlOQXjylOcG8KnZWI-Nb0kRCZW6OcIlqtIgKLE0JcYgNpGcZ--KE179gX4O89mKdgjIM4", RSAEncryptionPadding.OaepSHA256);

        // https://developer.mozilla.org/en-US/docs/Web/API/SubtleCrypto/importKey#json_web_key

        const bool includePrivateParameters = false; // 是否导出私钥，在 WebAPI SubtleCrypto 中私钥仅用于解密，公钥仅用于加密
        using var rsa = RSA.Create(4096); // 4096-bit
        var parameters = rsa.ExportParameters(includePrivateParameters);

        var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(
            new RsaSecurityKey(parameters));
        var json = SystemTextJsonSerializer.Serialize(jsonWebKey, jsonWebKey.GetType(), new SystemTextJsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = SystemTextJsonIgnoreCondition.WhenWritingNull,
        });
        TestContext.WriteLine(json);
    }

    const string RSAPrivateKey = """
        {"z":"kqbE_cdiGOWdSvvkWF1QitSRBiAZuKse2Gb252P_pc8nVVTHg7o0gIXt8RIWiTbWHSeLdxuJZcqNOTbC74K45IugCc6goDdl6eswz2podPF7ONqlxZfPP_5gq79KLuQmCZ4rjIPiG9eQUWrhu3rzKCx_Nn0haa5I8hI9W5RB_jhFA52sA-m_ZgJSMYh4s3HDe4a_6qJM038aQ7Rjf0lRmfgDqAJLdOj58EBIcU2XrBYQlliE7-LoyjSiQrFtoSa1uXM98dLkJWlZKRtGegNA5P0jiDqp-Zv7Cx9vUA37FCbCKLyOCR2iXlj-v8YcLfq87f68OrCUCqFlX0tJBqxv1zAsAqajIc8HKYA0g58t8ZndjSxtPbhxXttv0vMV0_r2V8g4y-WqRFfpmtCw75uBb9-1CjEw1eTN6OrUwWzTpNh6zulR9_XdX9YtoQ8z7S9whk4L4SeGOBF7XvMD_eFnDVAg1EodkZODFAlWDih9Tlf_CZCN1Tt-ibRZr-wCcpu8sSCOesWKI9U9wjnjNvMdLlUXeIsEC7mKw5yvw9vhUpwCDHvJxyq2HGhoUAIWV91lzfDF4N0A_oD-QP7xFNYxKAXbPA3ff1XyLCzpdjT5debhDJllskWtdCPlkYZRRAjasjQNbXWMvyCS_vgwqYSRV-f0kWufFFtcMabkakGth5k","x":"WIXkOsVVp6Kd7CPjhNNDfuY1nXFxvR3Oks9AcQtAT-fZQpdo5BymWN9HGrepxjp1G9NVHUiXr3jK0u6V6hBTkEpRK8HIoMFrvcoUlSZ4ToLJAk0Q9TSqxFEhx_g1vnDsioyx_XU-7St7r8v3R7qVh0mDkGLEZqdzWHw31mTcqKvGTWJdmkaYJy2OCqlpZQTnoOsbSpexyhy2YOW9-D61HH7AZTkApTlDnOFSGVKlZ1lp6qccVYnQsbEc7NKSYGEuCEXxLrLql4U9s1cQsQkILxBVly6vXeizeOMjYEEmnDFK5LKebdPK3cWMh0l9-9sFuk2FQrLj5KZZonJWxKivnw","c":"h79HdkyT9jdY9tpOvfKFbeMRKT9ZqTK_H2ZSPi6rhJXW4Ltl7G4dd5rEEjqDPucfwpwdmyiWpBoRpwI5fy8B9j4bORFLC6k26HYQK5rFs_iQrxWrqNsJ3-jETmUvCnDt6C2GBK5tkyKtb0LDa_fAVrHAYCCLScWR9ljCDS4PSMW6VwVPwVwaOdOM3xnWXf-D4JDvaspQnso9s6OUPz2Nrro6Ti5sHH48TbgrcPwr045Uj6t0jR8OIIrUJzdXk3s9kH20rPUxK95fJ8YRyfzAuTd5kky57fGie_DMMrfT6xyM-EhKA8PRO1XesgAQOqFFRUpVr6U2rgO2wubu45DbQw","v":"AQAB","b":"qz7R5aajzYy-94NaNtqIIovY8MtHDx-a1woStiZgFc03Y_AZRJV2OToT1ZryJeYLh7uY4Ddx3pAPYtWzYaO_7WASC67W53Ay8a-L30FIl9weknb9Rt5h_2P-8l266_cjxrD78tA8J2dPjD6gstZa2LRYxWM0Je7iWrM3EroybHnUMX29pgpIqd11cBGKmWapsZwbwBV7GyaC9Z2ynhaNbFMYLknYkDVEYBMO8b_qmzLTl_qbgOKt7P3semOfXLKHwsUao79UOtZhnz2HSYLjiXKLt_1ptazM7Xl33mgCePTgo8xt02qjyxUPdAdSzIh8ZGh2fCXk6hC5gxVjnWG6xQ","n":"vTeWydeo1IYYImYQ5S-IDK-m8oI2NrfV1-QYYpiqcMnTfU3fKYH7W1i0qupbPXGwKrojnZ1mkeq0yt5dMLmDWQ8NYh8UH7EBFc72_dlaV9imFrWc_wKTGwMoisfA7kf5sujkgWIUs-o_kak6SLMsie6ZVK7oDd5Te9kWrc5qJ3bZPfmRMHFlT-mK_c5RNBhtp-7NAzoHBlHboCc6IRtKdPSbmAuhKfwvGYDa8a_J8JD5E8CNpS3WPAC_jjyAWcYI5CyXLyXmf9IwiYM6gBWV8LBSHjXl_sKLw4hsicJEIYPTr6LlRDBmtuMWEZ_Br7KkIAhG2VPO6CC9WEseBJBBqmA33AykfHSNwX2sJW_5wdYqsLWSBhgUn9rbd6gdulM3TPtxQbig9c-XibeV9BTLMZkeEH95N3zLSJiPF-ElzlW8P5MMa9HR4Kp5AXqrxvoidT3g6sX81zJ9nqJBheNOZd26L9guHemwZ38TMOJAJlVMFBWY_02kyoYNn64tbE1B-vBaeWurHKDG1Wq9n_t2YZrlgGbN6BqySd1KBazC4cGGIcQxvOANlZJgD49BVNC0S5g1I_j0taP15VQfPBVWUwwMs-sd1dhIP8QDtjGDpZA9QS4F07VKCeUDBK5ApXDubZO2o-4qAJckKPBKk6a_EfZ5LmYAkaYNROXMxwyAWo0","m":"xq0C1mXJWysnuqccNYl-LqwG8LYP7Otec0y6vGV2KR4oyYCmChhpLh50qzkIUZqG6AbPsQIjipBW7e0oo9GnwjFeueZr-hdyZC-Jdz8tf-ikFuHPME-ISfS-h77TLlIr2Nc2c7ufRUCyqVq8vz0AdqjJTYui1rK87KpTHWa655IXESChR7SgadAgVX1AiU8A5sRKf3F0bjQPy110AuGnAn9R0g57TG_oNUaAquOyw6XIb7vACv6yHreeJOdm7A002bKRD7zAk2vi7y7k3hilPOhckstKN2Oun2vXzrwtvVGVtgD06fQQz5odV1OV0_tZlQ5Cgc2LGWHQXjm_uvuXvw","a":"88_sDt-xqsDGVqPi9NGSbg2zxLS4PTV0qr84chyQ-DNvqSHT7OrRi9yjVclUP7EwYt2v6ppnES_MCM-dkGHHyLdDaL4hG3f-1UYD5gGJ4xGURtkhbx1EoNlZ9ThapESd-c7c1MdZXxKzffv94ugZahY0niSTi3WXFb8_MOTb7GtAY62gMl9eu-jzcHKJU4zs_qbSZv2BWCG1GSy5miEF3DRmvhBALEze7yTyRzUxTFcUwoGq9VcYlmz9D4lXEKPlkAIrRzdyZnhCGqBoDxZ6BNByAWCPbAT4Xu7VUcT8yU0OJIh82pHIE_mIGU3phrUFdIlyVikWPeHZ-oaIpLrAsw"}
        """;
}
