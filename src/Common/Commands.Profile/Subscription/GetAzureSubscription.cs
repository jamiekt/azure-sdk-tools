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
using System.Management.Automation;
using Microsoft.WindowsAzure.Commands.Common;
using Microsoft.WindowsAzure.Commands.Common.Models;
using Microsoft.WindowsAzure.Commands.Common.Properties;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.Commands.Utilities.Common.Authentication;
using Microsoft.WindowsAzure.Commands.Utilities.Profile;
using Microsoft.WindowsAzure.Management;

namespace Microsoft.WindowsAzure.Commands.Profile
{
    /// <summary>
    /// Implementation of the get-azuresubscription cmdlet that works against
    /// the AzureProfile layer.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSubscription", DefaultParameterSetName = "ByName")]
    [OutputType(typeof(AzureSubscription))]
    public class GetAzureSubscriptionCommand : SubscriptionCmdletBase
    {
        public GetAzureSubscriptionCommand() : base(true)
        {

        }

        [Parameter(Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Name of the subscription", ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        [Alias("Name")]
        public string SubscriptionName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "ID of the subscription", ParameterSetName = "ById")]
        [ValidateNotNullOrEmpty]
        [Alias("Id")]
        public string SubscriptionId { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Retrieves the default subscription", ParameterSetName = "Default")]
        public SwitchParameter Default { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Retrieves the current subscription", ParameterSetName = "Current")]
        public SwitchParameter Current { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Retrieves extended details about subscription such as quota and usage")]
        public SwitchParameter ExtendedDetails { get; set; }

        public override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case "ByName":
                    WriteSubscriptions(ProfileClient.RefreshSubscriptions(AzureSession.CurrentEnvironment)
                        .Where(s => SubscriptionName == null || s.Name.Equals(SubscriptionName, StringComparison.InvariantCultureIgnoreCase)));
                    break;
                case "ById":
                    WriteSubscriptions(ProfileClient.GetSubscriptionById(new Guid(SubscriptionId)));
                    break;
                case "Default":
                    GetDefault();
                    break;
                case "Current":
                    GetCurrent();
                    break;
            }
        }

        public void GetDefault()
        {
            var defaultSubscription = ProfileClient.Profile.DefaultSubscription;

            if (defaultSubscription == null)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException(Resources.InvalidDefaultSubscription), 
                    string.Empty,
                    ErrorCategory.InvalidData, null));
            }
            else
            {
                WriteSubscriptions(defaultSubscription);
            }
        }

        public void GetCurrent()
        {
            //
            // Explicitly ignore the SubscriptionDataFile property here,
            // since current is strictly in-memory and we want the real
            // current subscription.
            //
            if (AzureSession.CurrentSubscription == null)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException(Resources.InvalidSelectedSubscription),
                    string.Empty,
                    ErrorCategory.InvalidData, null));
            }
            else
            {
                WriteSubscriptions(AzureSession.CurrentSubscription);
            }
        }

        private void WriteSubscriptions(params AzureSubscription[] subscriptions)
        {
            WriteSubscriptions((IEnumerable<AzureSubscription>) subscriptions);
        }

        private void WriteSubscriptions(IEnumerable<AzureSubscription> subscriptions)
        {
            IEnumerable<AzureSubscription> subscriptionOutput;

            if (ExtendedDetails.IsPresent)
            {
                subscriptionOutput = subscriptions.Select(s => s.ToExtendedData(AzureSession.ClientFactory, ProfileClient.Profile));
            }
            else
            {
                subscriptionOutput = subscriptions;
            }

            if (subscriptionOutput.Count() == 1)
            {
                AzureSubscription subscription = subscriptionOutput.First();
                PSObject psObject = base.ConstructPSObject(
                    null,
                    "SubscriptionId", subscription.Id,
                    "SubscriptionName", subscription.Name,
                    "Environment", subscription.Environment,
                    "SupportedModes", subscription.GetProperty(AzureSubscription.Property.SupportedModes),
                    "IsDefault", (subscription.GetProperty(AzureSubscription.Property.Default) != null ? "Yes" : ""),
                    "IsCurrent", (AzureSession.CurrentSubscription != null && AzureSession.CurrentSubscription.Id == subscription.Id
                        ? "Yes"
                        : ""));

                WriteObject(psObject);
            }
            else
            {
                List<PSObject> output = new List<PSObject>();
                subscriptionOutput.ForEach(s => output.Add(base.ConstructPSObject(
                    null,
                    "SubscriptionId", s.Id,
                    "SubscriptionName", s.Name,
                    "Environment", s.Environment,
                    "IsCurrent", (AzureSession.CurrentSubscription != null && AzureSession.CurrentSubscription.Id == s.Id ? "Yes" : "")
                    )));

                WriteObject(output, true);
            }
        }
    }

    static class SubscriptionConversions
    {
        internal static SubscriptionDataExtended ToExtendedData(this AzureSubscription subscription, IClientFactory clientFactory, AzureProfile azureProfile)
        {
            using (var client = clientFactory.CreateClient<ManagementClient>(subscription, AzureEnvironment.Endpoint.ServiceEndpoint))
            {
                var response = client.Subscriptions.Get();
                var environment = azureProfile.Environments[subscription.Environment];

                SubscriptionDataExtended result = new SubscriptionDataExtended
                {
                    AccountAdminLiveEmailId = response.AccountAdminLiveEmailId,
                    ActiveDirectoryUserId = subscription.GetProperty(AzureSubscription.Property.DefaultPrincipalName),
                    CurrentCoreCount = response.CurrentCoreCount,
                    CurrentHostedServices = response.CurrentHostedServices,
                    CurrentDnsServers = 0, // TODO: Add to spec
                    CurrentLocalNetworkSites = 0, // TODO: Add to spec
                    MaxCoreCount = response.MaximumCoreCount,
                    MaxDnsServers = response.MaximumDnsServers,
                    MaxHostedServices = response.MaximumHostedServices,
                    MaxVirtualNetworkSites = response.MaximumVirtualNetworkSites,
                    MaxStorageAccounts = response.MaximumStorageAccounts,
                    ServiceAdminLiveEmailId = response.ServiceAdminLiveEmailId,
                    SubscriptionRealName = response.SubscriptionName,
                    SubscriptionStatus = response.SubscriptionStatus.ToString(),
                    Name = subscription.Name,
                    Id = subscription.Id,
                    ServiceEndpoint = environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ServiceEndpoint).ToString(),
                    ResourceManagerEndpoint = environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ResourceManagerEndpoint).ToString(),
                    IsDefault = subscription.GetProperty(AzureSubscription.Property.Default) != null,
                    Certificate = ProfileClient.DataStore.GetCertificate(subscription.GetProperty(AzureSubscription.Property.Thumbprint)),
                    CurrentStorageAccountName = subscription.GetProperty(AzureSubscription.Property.CloudStorageAccount)
                };
                
                return result;
            }
        }
    }
}
