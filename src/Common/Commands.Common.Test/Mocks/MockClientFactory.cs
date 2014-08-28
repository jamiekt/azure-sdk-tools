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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.Utilities.HttpRecorder;
using Microsoft.WindowsAzure.Commands.Common.Factories;
using Microsoft.WindowsAzure.Commands.Common.Models;
using Microsoft.WindowsAzure.Common;

namespace Microsoft.WindowsAzure.Commands.Common.Test.Mocks
{
    public class MockClientFactory : IClientFactory
    {
        private IAuthenticationFactory authenticationFactory;

        public MockClientFactory(IAuthenticationFactory authenticationFactory)
        {
            this.authenticationFactory = authenticationFactory;
        }

        private readonly bool throwWhenNotAvailable;

        public List<object> ManagementClients { get; private set; }

        public MockClientFactory(IEnumerable<object> clients, bool throwIfClientNotSpecified = true)
        {
            ManagementClients = clients.ToList();
            throwWhenNotAvailable = throwIfClientNotSpecified;
        }

        public TClient CreateClient<TClient>(AzureSubscription subscription, Uri endpoint, AzureProfile profile) where TClient : ServiceClient<TClient>
        {
            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription.Id.ToString(), "fake_token");
            if (HttpMockServer.GetCurrentMode() != HttpRecorderMode.Playback)
            {
                creds = authenticationFactory.GetSubscriptionCloudCredentials(subscription, profile);
            }

            return CreateClient<TClient>(creds, endpoint);
        }

        public TClient CreateClient<TClient>(AzureSubscription subscription, AzureEnvironment.Endpoint endpointName, AzureProfile profile) where TClient : ServiceClient<TClient>
        {
            return CreateClient<TClient>(subscription, endpointName, profile.Environments[subscription.Environment]);
        }

        public TClient CreateClient<TClient>(AzureSubscription subscription, AzureEnvironment.Endpoint endpointName, AzureEnvironment environment) where TClient : ServiceClient<TClient>
        {
            Uri endpoint = environment.GetEndpointAsUri(endpointName);
            return CreateClient<TClient>(subscription, endpoint);
        }

        public TClient CreateClient<TClient>(AzureSubscription subscription, AzureEnvironment.Endpoint endpoint) where TClient : ServiceClient<TClient>
        {
            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription.Id.ToString(), "fake_token");
            if (HttpMockServer.GetCurrentMode() != HttpRecorderMode.Playback)
            {
                ProfileClient profileClient = new ProfileClient();
                creds = authenticationFactory.GetSubscriptionCloudCredentials(subscription, profileClient.Profile);
            }

            Uri endpointUri = (new ProfileClient()).Profile.Environments[subscription.Environment].GetEndpointAsUri(endpoint);
            return CreateClient<TClient>(creds, endpointUri);
        }

        public TClient CreateClient<TClient>(params object[] parameters) where TClient : ServiceClient<TClient>
        {
            TClient client = ManagementClients.FirstOrDefault(o => o is TClient) as TClient;
            if (client == null)
            {
                if (throwWhenNotAvailable)
                {
                    throw new ArgumentException(
                        string.Format("TestManagementClientHelper class wasn't initialized with the {0} client.",
                            typeof(TClient).Name));
                }
                else
                {
                    IClientFactory realHelper = new ClientFactory();
                    var realClient = realHelper.CreateClient<TClient>(parameters);
                    var newRealClient = realClient.WithHandler(HttpMockServer.CreateInstance());
                    realClient.Dispose();
                    return newRealClient;
                }
            }

            return client;
        }

        public HttpClient CreateHttpClient(string serviceUrl, ICredentials credentials)
        {
            return CreateHttpClient(serviceUrl, ClientFactory.CreateHttpClientHandler(serviceUrl, credentials));
        }

        public HttpClient CreateHttpClient(string serviceUrl, HttpMessageHandler effectiveHandler)
        {
            if (serviceUrl == null)
            {
                throw new ArgumentNullException("serviceUrl");
            }
            if (effectiveHandler == null)
            {
                throw new ArgumentNullException("effectiveHandler");
            }
            var mockHandler = HttpMockServer.CreateInstance();
            mockHandler.InnerHandler = effectiveHandler;

            HttpClient client = new HttpClient(mockHandler)
            {
                BaseAddress = new Uri(serviceUrl),
                MaxResponseContentBufferSize = 30 * 1024 * 1024
            };

            client.DefaultRequestHeaders.Accept.Clear();

            return client;
        }
    }
}
