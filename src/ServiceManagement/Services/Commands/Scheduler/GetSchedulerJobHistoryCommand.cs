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

namespace Microsoft.WindowsAzure.Commands.Scheduler
{
    using Microsoft.WindowsAzure.Commands.Utilities.Scheduler;
    using System.Collections.Generic;
    using System.Management.Automation;

    /// <summary>
    /// Cmdlet to get history of job executions
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureSchedulerJobHistory", SupportsPaging=true), OutputType(typeof(ICollection<string>))]
    public class GetSchedulerJobHistoryCommand : SchedulerBaseCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The location name.")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The job collection name.")]
        [ValidateNotNullOrEmpty]
        public string JobCollectionName { get; set; }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The job name.")]
        [ValidateNotNullOrEmpty]
        public string JobName { get; set; }

        [Parameter(Position = 3, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The job state.")]
        [ValidateSet("Completed", "Failed", IgnoreCase = true)]
        public string JobStatus { get; set; }

        public override void ExecuteCmdlet()
        {            
            WriteObject(SMClient.GetJobHistory(jobCollection: JobCollectionName, job: JobName, region: Location, jobStatus: JobStatus), true);
        }
    }
}
