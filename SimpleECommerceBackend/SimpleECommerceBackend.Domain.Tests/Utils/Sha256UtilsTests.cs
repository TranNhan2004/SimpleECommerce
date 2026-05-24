using FluentAssertions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Utils;

public class Sha256UtilsTests
{
    [Fact]
    public void ComputeHexHash_ShouldReturnExpectedSha256Digest()
    {
        Sha256Utils.ComputeHexHash("abc")
            .Should()
            .Be("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
    }
}