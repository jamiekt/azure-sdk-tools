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

namespace Microsoft.WindowsAzure.Management.ServiceManagement.IaaS.PersistentVMs
{
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.WindowsAzure.ServiceManagement;

    [Cmdlet(VerbsCommon.Get, "AzureSubnet"), OutputType(typeof(SubnetNamesCollection))]
    public class GetAzureSubnetCommand : VirtualMachineConfigurationCmdletBase
    {
        internal void ExecuteCommand()
        {
            var role = VM.GetInstance();

            var networkConfiguration = role.ConfigurationSets
                                        .OfType<NetworkConfigurationSet>()
                                        .SingleOrDefault();

            if (networkConfiguration == null)
            {
                WriteObject(null);
            }
            else
            {
                WriteObject(networkConfiguration.SubnetNames, true);
            }
        }

        protected override void ProcessRecord()
        {
            ExecuteCommand();
        }
    }
}