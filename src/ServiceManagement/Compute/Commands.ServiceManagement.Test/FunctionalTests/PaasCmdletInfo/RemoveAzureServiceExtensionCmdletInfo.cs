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

namespace Microsoft.WindowsAzure.Commands.ServiceManagement.Test.FunctionalTests.PaasCmdletInfo
{
    using PowershellCore;

    public class RemoveAzureServiceExtensionCmdletInfo : CmdletsInfo
    {
        public RemoveAzureServiceExtensionCmdletInfo(string serviceName, bool uninstall, string[] roles, string slot,
            string extensionName, string providerNamespace)
        {
            this.cmdletName = Utilities.RemoveAzureServiceExtensionCmdletName;
            this.cmdletParams.Add(new CmdletParam("ServiceName", serviceName));
            if (!string.IsNullOrEmpty(slot))
            {
                this.cmdletParams.Add(new CmdletParam("Slot", slot));
            }
            if (roles != null)
            {
                this.cmdletParams.Add(new CmdletParam("Role", roles));
            }
            if (uninstall)
            {
                this.cmdletParams.Add(new CmdletParam("UninstallConfiguration"));
            }
            this.cmdletParams.Add(new CmdletParam("ExtensionName", extensionName));
            this.cmdletParams.Add(new CmdletParam("ProviderNamespace", providerNamespace));
        }
    }
}
