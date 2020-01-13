﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.Bot.Connector.Tests.Authentication
{
    public class AppCredentialsTests
    {
        [Fact]
        public void ConstructorTests()
        {
            var shouldDefaultToChannelScope = new TestAppCredentials("irrelevant", null, null);
            Assert.Equal(AuthenticationConstants.ToChannelFromBotOAuthScope, shouldDefaultToChannelScope.OAuthScope);

            var shouldDefaultToCustomScope = new TestAppCredentials("irrelevant", null, null, "customScope");
            Assert.Equal("customScope", shouldDefaultToCustomScope.OAuthScope);
        }

        private class TestAppCredentials : AppCredentials
        {
            public TestAppCredentials(string channelAuthTenant = null, HttpClient customHttpClient = null, ILogger logger = null)
                : base(channelAuthTenant, customHttpClient, logger)
            {
            }

            public TestAppCredentials(string channelAuthTenant = null, HttpClient customHttpClient = null, ILogger logger = null, string oAuthScope = null)
                : base(channelAuthTenant, customHttpClient, logger, oAuthScope)
            {
            }

            protected override Lazy<IAuthenticator> BuildAuthenticator()
            {
                return new Mock<Lazy<IAuthenticator>>().Object;
            }
        }
    }
}
