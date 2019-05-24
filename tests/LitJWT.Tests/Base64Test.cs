using FluentAssertions;
using RandomFixtureKit;
using System;
using Xunit;

namespace LitJWT.Tests
{
    public class Base64Test
    {
        [Fact]
        public void Base64Encode()
        {
            Span<char> writeTo = new char[1024];
            foreach (var item in FixtureFactory.CreateMany<byte[]>(1000, resolver: RandomByteArrayResolver.Default))
            {
                var reference = Convert.ToBase64String(item);

                Base64.TryToBase64Chars(item, writeTo, out var written);
                var implResult = new string(writeTo.Slice(0, written));

                implResult.Should().Be(reference);
            }
        }

        [Fact]
        public void Base64Decode()
        {
            Span<byte> writeTo = new byte[1024];
            foreach (var item in FixtureFactory.CreateMany<byte[]>(1000, resolver: RandomByteArrayResolver.Default))
            {
                var referenceString = Convert.ToBase64String(item);

                Base64.TryFromBase64Chars(referenceString, writeTo, out var written);
                var implResult = writeTo.Slice(0, written);

                implResult.SequenceEqual(item).Should().BeTrue("Str:{0} Reference:{1} Actual:{2}", referenceString, string.Join(",", item), string.Join(",", implResult.ToArray()));
            }
        }

        [Fact]
        public void EdgeCaseEncode()
        {
            Span<char> writeTo = new char[1024];
            foreach (var item in new byte[][] { new byte[0] })
            {
                var reference = Convert.ToBase64String(item);

                Base64.TryToBase64Chars(item, writeTo, out var written);
                var implResult = new string(writeTo.Slice(0, written));

                implResult.Should().Be(reference);
            }
        }

        [Fact]
        public void InvalidDecodeCases()
        {
            Span<byte> writeTo = new byte[1024];
            Base64.TryFromBase64Chars("AAB!DEFG", writeTo, out var written).Should().BeFalse();
            Base64.TryFromBase64Chars("AAB=DEFG", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("AABCDEF!", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("AABCDEF", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("=", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("A", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("A===", writeTo, out written).Should().BeFalse();
            Base64.TryFromBase64Chars("A!!!", writeTo, out written).Should().BeFalse();
        }

        [Fact]
        public void MaxLength()
        {
            for (int i = 0; i < 99; i++)
            {
                Span<char> chars = new char[1000];
                Base64.TryToBase64Chars(new byte[i], chars, out var actual);

                actual.Should().BeLessOrEqualTo(Base64.GetMaxBase64Length(i));
            }
        }

    }
}