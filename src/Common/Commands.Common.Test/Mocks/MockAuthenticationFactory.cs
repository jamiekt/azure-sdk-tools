﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.WindowsAzure.Commands.Common.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common.Authentication;

namespace Microsoft.WindowsAzure.Commands.Common.Test.Mocks
{
    public class MockAuthenticationFactory : IAuthenticationFactory
    {
        private const string CommonAdTenant = "Common";

        public IAccessToken Token { get; set; }

        public MockAuthenticationFactory()
        {
            Token = new MockAccessToken
            {
                UserId = "Test",
                LoginType = LoginType.OrgId,
                AccessToken = "abc"
            };
        }

        public IAccessToken Authenticate(AzureEnvironment environment, ref UserCredentials credentials)
        {
            return Authenticate(environment, CommonAdTenant, ref credentials);
        }

        public IAccessToken Authenticate(AzureEnvironment environment, string tenant, ref UserCredentials credentials)
        {
            if (credentials.UserName == null)
            {
                credentials.UserName = "test";
            }

            Token = new MockAccessToken
            {
                UserId = credentials.UserName,
                LoginType = LoginType.OrgId,
                AccessToken = "abc"
            };
            
            return Token;
        }

        public SubscriptionCloudCredentials GetSubscriptionCloudCredentials(AzureSubscription subscription, AzureProfile profile)
        {
            return new AccessTokenCredential(subscription.Id, Token);
        }
    }
}
