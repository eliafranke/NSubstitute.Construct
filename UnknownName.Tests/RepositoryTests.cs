using System;
using FluentAssertions;
using NSubstitute;
using Tests.Subjects;
using Xunit;

namespace Tests
{
    public class RepositoryTests
    {
        [Theory]
        [InlineData("Hello world")]
        [InlineData("I can do it")]
        [InlineData("A string")]
        public void Construct_ReturnsExpectedInterface_AndUsesGivenParameters(string input)
        {
            var add = Substitute.For<IAddItemToRepository>();
            add
                .AddItem(Arg.Any<Guid>())
                .Returns(input);

            var repository = Construct.For<IAddItemToRepository, Repository>(add);

            var result = repository.AddItem(Guid.NewGuid());

            result
                .Should()
                .Be(input);
        }

        [Theory]
        [InlineData("Hello world")]
        [InlineData("I can do it")]
        [InlineData("A string")]
        public void Construct_ReturnsExpectedType_AndUsesGivenParameters(string input)
        {
            var add = Substitute.For<IAddItemToRepository>();
            add
                .AddItem(Arg.Any<Guid>())
                .Returns(input);

            var doItem = Substitute.For<IDoItemWithRepository>();
            doItem
                .DoItem(Arg.Any<Guid>())
                .Returns(input);

            var repository = Construct.For<Repository>(doItem, add);

            var result = repository.DoItem(Guid.NewGuid());

            result
                .Should()
                .Be(input);
        }
    }
}