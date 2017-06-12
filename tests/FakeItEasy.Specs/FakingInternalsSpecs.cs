﻿namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    internal interface IInternal
    {
    }

    public static class FakingInternalsSpecs
    {
        [Scenario]
        public static void InvisibleInternalInterface(
            Exception exception)
        {
            "Given an internal interface not visible to DynamicProxyGenAssembly2"
                .See<IInternal>();

            "When I try to fake the interface"
                .x(() => exception = Record.Exception(() => A.Fake<IInternal>()));

            "Then it throws an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                    .Contain("Make it public, or internal and mark your assembly with")
                    .And.Match("*[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2*\")]*"));
        }

        [Scenario]
        public static void GenericTypeWithInternalTypeParameters(
            Exception exception)
        {
            "Given a public generic interface with a type parameter that is not visible to DynamicProxyGenAssembly2"
                .See<IList<IInternal>>();

            "When I try to fake the public interface"
                .x(() => exception = Record.Exception(() => A.Fake<IList<IInternal>>()));

            "Then it throws an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                             .Contain(
                                 "No usable default constructor was found on the type System.Collections.Generic.IList`1[FakeItEasy.Specs.IInternal]")
                             .And.Match(
                                 "*because type FakeItEasy.Specs.IInternal is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2*\")]*"));
        }

        [Scenario]
        public static void OverrideInternalMethod(
            TypeWithInternalMethod fake,
            Exception exception)
        {
            "Given a public type"
                .See<TypeWithInternalMethod>();

            "And the type has an internal method not visible to DynamicProxyGenAssembly2"
                .See<TypeWithInternalMethod>();

            "And I create a fake of the type"
            .x(() => fake = A.Fake<TypeWithInternalMethod>());

            "When I override the internal"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.InternalMethod()).Returns(17)));

            "Then it throws an exception with a message complaining about accessibility"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().Contain("not accessible to DynamicProxyGenAssembly2"));
        }
    }

    public class TypeWithInternalMethod
    {
        internal virtual int InternalMethod()
        {
            return 8;
        }
    }
}
