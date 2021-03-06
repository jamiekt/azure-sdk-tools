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

using Microsoft.WindowsAzure.Commands.Common.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.WindowsAzure.Commands.Common.Models
{
    public class VirtualDiskDataStore : IDataStore
    {
        Dictionary<string, object> virtualDisk;

        public VirtualDiskDataStore()
        {
            virtualDisk = new Dictionary<string, object>();
        }

        public void WriteFile(string path, string contents)
        {
            virtualDisk[path] = contents;
        }

        public string ReadFile(string path)
        {
            return (string)virtualDisk[path];
        }

        public bool FileExists(string path)
        {
            return virtualDisk.ContainsKey(path);
        }

        public X509Certificate2 GetCertificate(string thumbprint)
        {
            return (X509Certificate2)virtualDisk[thumbprint];
        }

        public void AddCertificate(X509Certificate2 cert)
        {
            virtualDisk.Add(cert.Thumbprint, cert);
        }
    }
}
