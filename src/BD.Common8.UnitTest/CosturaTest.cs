extern alias SG_Repo;

namespace BD.Common8.UnitTest;

public sealed class CosturaTest
{
    [Test]
    public void SourceGenerator_Repositories()
    {
        SG_Repo::BD.Common8.SourceGenerator.Repositories.PackageReferenceTest.Assemblies();
    }
}
