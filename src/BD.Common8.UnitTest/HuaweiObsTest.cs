using Microsoft.Extensions.Configuration;
using OBS;
using OBS.Model;

namespace BD.Common8.UnitTest;

public class HuaweiObsTest
{
    private static ObsClient _obsClient = null!;

    private const string bucketName = "common8-testbucket";

    private string? location;

    [SetUp]
    public void SetUp()
    {
        ConfigurationBuilder builder = new();
        builder.AddUserSecrets(typeof(HuaweiObsTest).Assembly);
        IConfigurationRoot configurationRoot = builder.Build();

        _obsClient ??= new ObsClient(
            configurationRoot["access_key"].ThrowIsNull(),
            configurationRoot["secret_key"].ThrowIsNull(),
            configurationRoot["endpoint"].ThrowIsNull());

        location ??= new Uri(_obsClient.ObsConfig.Endpoint!).Host?.Replace("obs.", string.Empty).Split('.')[0] ?? string.Empty;
    }

    [Ignore("Bucket_Test ignore")]
    public void Bucket_Test()
    {
        // check is exists
        var exists = _obsClient.HeadBucket(new() { BucketName = bucketName });

        // get buckes list
        var list_buckets = _obsClient.ListBuckets(new());
        Assert.That(list_buckets.Buckets, Is.Not.Null);

        // if is not exists create bucket
        if (!exists)
            CreateBucket();

        // delete previous created bucket
        _obsClient.DeleteBucket(new() { BucketName = bucketName });

        // against check delete success
        exists = _obsClient.HeadBucket(new() { BucketName = bucketName });
        Assert.That(exists, Is.EqualTo(false));
    }

    [Ignore("UpLoad_Test ignore")]
    public void UpLoad_Test()
    {
        var filePath = @"C:\xxx\xxx\hello.png";
        var file = new FileInfo(filePath);

        using var stream = file.OpenRead();
        var fileName = Hashs.String.SHA384(stream) + Path.GetExtension(file.FullName);

        // check if file exists, if not then upload
        if (!_obsClient.HeadObject(new()
        {
            BucketName = bucketName,
            ObjectKey = fileName,
        }))
        {
            goto doPutObject;
        doPutObject: try
            {
                var put_result = _obsClient.PutObject(new()
                {
                    BucketName = bucketName,
                    ObjectKey = fileName,
                    InputStream = stream,
                });
                Assert.That(put_result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
            catch (ObsException obs_ex)
            {
                if (obs_ex.StatusCode == HttpStatusCode.NotFound)
                {
                    TestContext.WriteLine("current bucket such notfound");
                    CreateBucket();
                    goto doPutObject;
                }
                throw;
            }
        }

        // delete previous upload file
        var delete_req = new DeleteObjectRequest() { BucketName = bucketName, ObjectKey = fileName };
        var delete_rsp = _obsClient.DeleteObject(delete_req);

        // check delete if success
        var exists = _obsClient.HeadObject(new HeadObjectRequest() { BucketName = bucketName, ObjectKey = fileName });
        Assert.That(exists, Is.EqualTo(false));
    }

    private void CreateBucket()
    {
        var createBucketRequest = new CreateBucketRequest()
        {
            BucketName = bucketName,
            StorageClass = StorageClassEnum.Standard,
            CannedAcl = CannedAclEnum.Private,
            Location = location,
        };
        var create_bucket = _obsClient.CreateBucket(createBucketRequest);
        Assert.Multiple(() =>
        {
            Assert.That(create_bucket, Is.Not.Null);
            Assert.That(create_bucket.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}
