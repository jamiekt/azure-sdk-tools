// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Commands.ServiceManagement
{
    using AutoMapper;
    using Extensions;
    using Helpers;
    using IaaS.Extensions;
    using Management.Compute.Models;
    using Management.Models;
    using Management.Network.Models;
    using Management.Storage.Models;
    using Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using Utilities.Common;
    using NSM = Management.Compute.Models;
    using NVM = Management.Network.Models;
    using PVM = Model;

    public static class ServiceManagementMapperExtension
    {
        public static IMappingExpression<TSource, TDestination> ForItems<TSource, TDestination, T>(
                 this IMappingExpression<TSource, TDestination> mapper)
            where TSource : IEnumerable
            where TDestination : ICollection<T>
        {
            mapper.AfterMap((c, s) =>
            {
                if (c != null && s != null)
                {
                    foreach (var t in c)
                    {
                        s.Add(Mapper.Map<T>(t));
                    }
                }
            });

            return mapper;
        }
    }

    public class ServiceManagementProfile : Profile
    {
        private static readonly Lazy<bool> initialize;

        static ServiceManagementProfile()
        {
            initialize = new Lazy<bool>(() =>
            {
                Mapper.AddProfile<ServiceManagementProfile>();
                return true;
            });
        }

        public override string ProfileName
        {
            get { return "ServiceManagementProfile"; }
        }

        public static bool Initialize()
        {
            return initialize.Value;
        }

        protected override void Configure()
        {
            // Service Extension Image
            Mapper.CreateMap<OperationStatusResponse, ExtensionImageContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<HostedServiceListAvailableExtensionsResponse.ExtensionImage, ExtensionImageContext>()
                  .ForMember(c => c.ExtensionName, o => o.MapFrom(r => r.Type));

            // VM Extension Image
            Mapper.CreateMap<OperationStatusResponse, VirtualMachineExtensionImageContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<VirtualMachineExtensionListResponse.ResourceExtension, VirtualMachineExtensionImageContext>()
                  .ForMember(c => c.ExtensionName, o => o.MapFrom(r => r.Name));

            //Image mapping
            Mapper.CreateMap<VirtualMachineOSImageListResponse.VirtualMachineOSImage, OSImageContext>()
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => new DateTime?(r.PublishedDate)))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageGetResponse, OSImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => new DateTime?(r.PublishedDate)))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageCreateResponse, OSImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => r.PublishedDate))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageUpdateResponse, OSImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => r.PublishedDate))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<OperationStatusResponse, OSImageContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<VirtualMachineDiskCreateResponse, OSImageContext>()
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystem));

            // VM Image mapping
            Mapper.CreateMap<VirtualMachineOSImageListResponse.VirtualMachineOSImage, VMImageContext>()
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => new DateTime?(r.PublishedDate)))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageGetResponse, VMImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => new DateTime?(r.PublishedDate)))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageCreateResponse, VMImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => r.PublishedDate))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<VirtualMachineOSImageUpdateResponse, VMImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.IconUri, o => o.MapFrom(r => r.SmallIconUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.PublishedDate, o => o.MapFrom(r => r.PublishedDate))
                  .ForMember(c => c.LogicalSizeInGB, o => o.MapFrom(r => (int)r.LogicalSizeInGB));

            Mapper.CreateMap<OperationStatusResponse, VMImageContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<VirtualMachineDiskCreateResponse, VMImageContext>()
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystem));

            // VM Image Disk Mapping
            Mapper.CreateMap<VirtualMachineVMImageListResponse.OSDiskConfiguration, PVM.OSDiskConfiguration>()
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystem));
            Mapper.CreateMap<VirtualMachineVMImageListResponse.DataDiskConfiguration, PVM.DataDiskConfiguration>()
                  .ForMember(c => c.Lun, o => o.MapFrom(r => r.LogicalUnitNumber));

            Mapper.CreateMap<IList<NSM.DataDiskConfigurationUpdateParameters>, List<PVM.DataDiskConfiguration>>();
            Mapper.CreateMap<List<NSM.DataDiskConfigurationUpdateParameters>, List<PVM.DataDiskConfiguration>>();
            Mapper.CreateMap<IList<NSM.DataDiskConfigurationUpdateParameters>, PVM.DataDiskConfigurationList>();

            Mapper.CreateMap<PVM.OSDiskConfiguration, NSM.OSDiskConfigurationUpdateParameters>();
            Mapper.CreateMap<PVM.DataDiskConfiguration, NSM.DataDiskConfigurationUpdateParameters>()
                  .ForMember(c => c.LogicalUnitNumber, o => o.MapFrom(r => r.Lun));

            Mapper.CreateMap<IList<PVM.DataDiskConfiguration>, IList<NSM.DataDiskConfigurationUpdateParameters>>();
            Mapper.CreateMap<List<PVM.DataDiskConfiguration>, List<NSM.DataDiskConfigurationUpdateParameters>>();
            Mapper.CreateMap<PVM.DataDiskConfigurationList, Collection<PVM.DataDiskConfiguration>>();
            Mapper.CreateMap<Collection<PVM.DataDiskConfiguration>, IList<NSM.DataDiskConfigurationUpdateParameters>>();
            Mapper.CreateMap<Collection<PVM.DataDiskConfiguration>, List<NSM.DataDiskConfigurationUpdateParameters>>();
            Mapper.CreateMap<PVM.DataDiskConfigurationList, IList<NSM.DataDiskConfigurationUpdateParameters>>();
            Mapper.CreateMap<PVM.DataDiskConfigurationList, List<NSM.DataDiskConfigurationUpdateParameters>>();

            Mapper.CreateMap<VirtualMachineVMImageListResponse.VirtualMachineVMImage, VMImageContext>()
                  .ForMember(c => c.ImageName, o => o.MapFrom(r => r.Name));

            Mapper.CreateMap<OperationStatusResponse, VMImageContext>()
                  .ForMember(c => c.OS, o => o.Ignore())
                  .ForMember(c => c.LogicalSizeInGB, o => o.Ignore())
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            // VM Resource Extensions
            Mapper.CreateMap<NSM.GuestAgentMessage, PVM.GuestAgentMessage>();
            Mapper.CreateMap<NSM.GuestAgentFormattedMessage, PVM.GuestAgentFormattedMessage>();
            Mapper.CreateMap<NSM.GuestAgentStatus, PVM.GuestAgentStatus>()
                  .ForMember(c => c.TimestampUtc, o => o.MapFrom(r => r.Timestamp));

            Mapper.CreateMap<NSM.ResourceExtensionConfigurationStatus, PVM.ResourceExtensionConfigurationStatus>()
                  .ForMember(c => c.TimestampUtc, o => o.MapFrom(r => r.Timestamp))
                  .ForMember(c => c.ConfigurationAppliedTimeUtc, o => o.MapFrom(r => r.ConfigurationAppliedTime));

            Mapper.CreateMap<NSM.ResourceExtensionSubStatus, PVM.ResourceExtensionSubStatus>();
            Mapper.CreateMap<IList<NSM.ResourceExtensionSubStatus>, PVM.ResourceExtensionStatusList>();
            Mapper.CreateMap<IEnumerable<NSM.ResourceExtensionSubStatus>, PVM.ResourceExtensionStatusList>();
            Mapper.CreateMap<List<NSM.ResourceExtensionSubStatus>, PVM.ResourceExtensionStatusList>();

            Mapper.CreateMap<NSM.ResourceExtensionStatus, PVM.ResourceExtensionStatus>();
            Mapper.CreateMap<IList<NSM.ResourceExtensionStatus>, PVM.ResourceExtensionStatusList>();
            Mapper.CreateMap<IEnumerable<NSM.ResourceExtensionStatus>, PVM.ResourceExtensionStatusList>();
            Mapper.CreateMap<List<NSM.ResourceExtensionStatus>, PVM.ResourceExtensionStatusList>();

            //SM to NewSM mapping
            Mapper.CreateMap<PVM.LoadBalancerProbe, NSM.LoadBalancerProbe>()
                  .ForMember(c => c.Protocol, o => o.MapFrom(r => r.Protocol));
            Mapper.CreateMap<PVM.AccessControlListRule, NSM.AccessControlListRule>();
            Mapper.CreateMap<PVM.EndpointAccessControlList, NSM.EndpointAcl>()
                  .ForMember(c => c.Rules, o => o.MapFrom(r => r.Rules.ToList()));
            Mapper.CreateMap<PVM.InputEndpoint, NSM.InputEndpoint>()
                  .ForMember(c => c.VirtualIPAddress, o => o.MapFrom(r => r.Vip != null ? IPAddress.Parse(r.Vip) : null))
                  .ForMember(c => c.EndpointAcl, o => o.MapFrom(r => r.EndpointAccessControlList))
                  .ForMember(c => c.LoadBalancerName, o => o.MapFrom(r => r.LoadBalancerName));
            Mapper.CreateMap<PVM.DataVirtualHardDisk, NSM.DataVirtualHardDisk>()
                  .ForMember(c => c.Name, o => o.MapFrom(r => r.DiskName))
                  .ForMember(c => c.Label, o => o.MapFrom(r => r.DiskLabel))
                  .ForMember(c => c.LogicalUnitNumber, o => o.MapFrom(r => r.Lun));
            Mapper.CreateMap<PVM.OSVirtualHardDisk, NSM.OSVirtualHardDisk>()
                  .ForMember(c => c.Name, o => o.MapFrom(r => r.DiskName))
                  .ForMember(c => c.Label, o => o.MapFrom(r => r.DiskLabel))
                  .ForMember(c => c.OperatingSystem, o => o.MapFrom(r => r.OS));
            Mapper.CreateMap<PVM.NetworkConfigurationSet, NSM.ConfigurationSet>()
                  .ForMember(c => c.InputEndpoints, o => o.MapFrom(r => r.InputEndpoints != null ? r.InputEndpoints.ToList() : null))
                  .ForMember(c => c.SubnetNames, o => o.MapFrom(r => r.SubnetNames != null ? r.SubnetNames.ToList() : null))
                  .ForMember(c => c.PublicIPs, o => o.MapFrom(r => r.PublicIPs != null ? r.PublicIPs.ToList() : null));

            Mapper.CreateMap<PVM.LinuxProvisioningConfigurationSet.SSHKeyPair, NSM.SshSettingKeyPair>();
            Mapper.CreateMap<PVM.LinuxProvisioningConfigurationSet.SSHPublicKey, NSM.SshSettingPublicKey>();
            Mapper.CreateMap<PVM.LinuxProvisioningConfigurationSet.SSHSettings, NSM.SshSettings>();
            Mapper.CreateMap<PVM.LinuxProvisioningConfigurationSet, NSM.ConfigurationSet>()
                  .ForMember(c => c.PublicIPs, o => o.Ignore())
                  .ForMember(c => c.UserPassword, o => o.MapFrom(r => r.UserPassword == null ? null : r.UserPassword.ConvertToUnsecureString()))
                  .ForMember(c => c.SshSettings, o => o.MapFrom(r => r.SSH));
            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet, NSM.ConfigurationSet>()
                  .ForMember(c => c.PublicIPs, o => o.Ignore())
                  .ForMember(c => c.AdminPassword, o => o.MapFrom(r => r.AdminPassword == null ? null : r.AdminPassword.ConvertToUnsecureString()));
            Mapper.CreateMap<PVM.ProvisioningConfigurationSet, NSM.ConfigurationSet>()
                  .ForMember(c => c.PublicIPs, o => o.Ignore());
            Mapper.CreateMap<PVM.ConfigurationSet, NSM.ConfigurationSet>()
                  .ForMember(c => c.PublicIPs, o => o.Ignore());
            Mapper.CreateMap<PVM.InstanceEndpoint, NSM.InstanceEndpoint>()
                  .ForMember(c => c.VirtualIPAddress, o => o.MapFrom(r => r.Vip != null ? IPAddress.Parse(r.Vip) : null))
                  .ForMember(c => c.Port, o => o.MapFrom(r => r.PublicPort));

            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.WinRmConfiguration, NSM.WindowsRemoteManagementSettings>();
            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.WinRmListenerProperties, NSM.WindowsRemoteManagementListener>()
                  .ForMember(c => c.ListenerType, o => o.MapFrom(r => r.Protocol));
            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.WinRmListenerCollection, IList<NSM.WindowsRemoteManagementListener>>();

            //NewSM to SM mapping
            Mapper.CreateMap<NSM.LoadBalancerProbe, PVM.LoadBalancerProbe>()
                  .ForMember(c => c.Protocol, o => o.MapFrom(r => r.Protocol.ToString().ToLower()));
            Mapper.CreateMap<NSM.AccessControlListRule, PVM.AccessControlListRule>();
            Mapper.CreateMap<NSM.EndpointAcl, PVM.EndpointAccessControlList>()
                  .ForMember(c => c.Rules, o => o.MapFrom(r => r.Rules));
            Mapper.CreateMap<NSM.InputEndpoint, PVM.InputEndpoint>()
                  .ForMember(c => c.LoadBalancerName, o => o.MapFrom(r => r.LoadBalancerName))
                  .ForMember(c => c.Vip, o => o.MapFrom(r => r.VirtualIPAddress != null ? r.VirtualIPAddress.ToString() : null))
                  .ForMember(c => c.EndpointAccessControlList, o => o.MapFrom(r => r.EndpointAcl));
            Mapper.CreateMap<NSM.DataVirtualHardDisk, PVM.DataVirtualHardDisk>()
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.DiskLabel, o => o.MapFrom(r => r.Label))
                  .ForMember(c => c.Lun, o => o.MapFrom(r => r.LogicalUnitNumber));
            Mapper.CreateMap<NSM.OSVirtualHardDisk, PVM.OSVirtualHardDisk>()
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.DiskLabel, o => o.MapFrom(r => r.Label))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystem));
            Mapper.CreateMap<NSM.ConfigurationSet, PVM.ConfigurationSet>();
            Mapper.CreateMap<NSM.ConfigurationSet, PVM.NetworkConfigurationSet>();

            Mapper.CreateMap<NSM.SshSettingKeyPair, PVM.LinuxProvisioningConfigurationSet.SSHKeyPair>();
            Mapper.CreateMap<NSM.SshSettingPublicKey, PVM.LinuxProvisioningConfigurationSet.SSHPublicKey>();
            Mapper.CreateMap<NSM.SshSettings, PVM.LinuxProvisioningConfigurationSet.SSHSettings>();
            Mapper.CreateMap<NSM.ConfigurationSet, PVM.LinuxProvisioningConfigurationSet>()
                  .ForMember(c => c.UserPassword, o => o.MapFrom(r => SecureStringHelper.GetSecureString(r.UserPassword)))
                  .ForMember(c => c.SSH, o => o.MapFrom(r => r.SshSettings));
            Mapper.CreateMap<NSM.ConfigurationSet, PVM.WindowsProvisioningConfigurationSet>()
                  .ForMember(c => c.AdminPassword, o => o.MapFrom(r => SecureStringHelper.GetSecureString(r.AdminPassword)));
            Mapper.CreateMap<NSM.InstanceEndpoint, PVM.InstanceEndpoint>()
                  .ForMember(c => c.Vip, o => o.MapFrom(r => r.VirtualIPAddress != null ? r.VirtualIPAddress.ToString() : null))
                  .ForMember(c => c.PublicPort, o => o.MapFrom(r => r.Port));

            Mapper.CreateMap<NSM.WindowsRemoteManagementSettings, PVM.WindowsProvisioningConfigurationSet.WinRmConfiguration>();
            Mapper.CreateMap<NSM.WindowsRemoteManagementListener, PVM.WindowsProvisioningConfigurationSet.WinRmListenerProperties>()
                  .ForMember(c => c.Protocol, o => o.MapFrom(r => r.ListenerType.ToString()));
            Mapper.CreateMap<IList<NSM.WindowsRemoteManagementListener>, PVM.WindowsProvisioningConfigurationSet.WinRmListenerCollection>();

            // LoadBalancedEndpointList mapping
            Mapper.CreateMap<PVM.AccessControlListRule, NSM.AccessControlListRule>();
            Mapper.CreateMap<PVM.EndpointAccessControlList, NSM.EndpointAcl>();
            Mapper.CreateMap<PVM.InputEndpoint, VirtualMachineUpdateLoadBalancedSetParameters.InputEndpoint>()
                  .ForMember(c => c.Rules, o => o.MapFrom(r => r.EndpointAccessControlList == null ? null : r.EndpointAccessControlList.Rules))
                  .ForMember(c => c.VirtualIPAddress, o => o.MapFrom(r => r.Vip));

            Mapper.CreateMap<NSM.AccessControlListRule, PVM.AccessControlListRule>();
            Mapper.CreateMap<NSM.EndpointAcl, PVM.EndpointAccessControlList>();
            Mapper.CreateMap<NSM.VirtualMachineUpdateLoadBalancedSetParameters.InputEndpoint, PVM.InputEndpoint>()
                  .ForMember(c => c.EndpointAccessControlList, o => o.MapFrom(r => r.Rules == null ? null : r.Rules))
                  .ForMember(c => c.Vip, o => o.MapFrom(r => r.VirtualIPAddress));

            //Common mapping
            Mapper.CreateMap<OperationResponse, ManagementOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.RequestId))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.StatusCode.ToString()));

            Mapper.CreateMap<OperationStatusResponse, ManagementOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //AffinityGroup mapping
            Mapper.CreateMap<AffinityGroupGetResponse, AffinityGroupContext>()
                  .ForMember(c => c.VirtualMachineRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.VirtualMachinesRoleSizes))
                  .ForMember(c => c.WebWorkerRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.WebWorkerRoleSizes));
            Mapper.CreateMap<AffinityGroupListResponse.AffinityGroup, AffinityGroupContext>()
                  .ForMember(c => c.VirtualMachineRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.VirtualMachinesRoleSizes))
                  .ForMember(c => c.WebWorkerRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.WebWorkerRoleSizes));
            Mapper.CreateMap<AffinityGroupGetResponse.HostedServiceReference, AffinityGroupContext.Service>()
                  .ForMember(c => c.Url, o => o.MapFrom(r => r.Uri));
            Mapper.CreateMap<AffinityGroupGetResponse.StorageServiceReference, AffinityGroupContext.Service>()
                  .ForMember(c => c.Url, o => o.MapFrom(r => r.Uri));
            Mapper.CreateMap<OperationStatusResponse, AffinityGroupContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //Location mapping
            Mapper.CreateMap<LocationsListResponse.Location, LocationsContext>()
                  .ForMember(c => c.VirtualMachineRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.VirtualMachinesRoleSizes))
                  .ForMember(c => c.WebWorkerRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.WebWorkerRoleSizes));
            Mapper.CreateMap<OperationStatusResponse, LocationsContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //Role sizes mapping
            Mapper.CreateMap<RoleSizeListResponse.RoleSize, RoleSizeContext>()
                  .ForMember(c => c.InstanceSize, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.RoleSizeLabel, o => o.MapFrom(r => r.Label));
            Mapper.CreateMap<OperationStatusResponse, RoleSizeContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));
            
            //ServiceCertificate mapping
            Mapper.CreateMap<ServiceCertificateGetResponse, CertificateContext>()
                  .ForMember(c => c.Data, o => o.MapFrom(r => r.Data != null ? Convert.ToBase64String(r.Data) : null));
            Mapper.CreateMap<ServiceCertificateListResponse.Certificate, CertificateContext>()
                  .ForMember(c => c.Url, o => o.MapFrom(r => r.CertificateUri))
                  .ForMember(c => c.Data, o => o.MapFrom(r => r.Data != null ? Convert.ToBase64String(r.Data) : null));
            Mapper.CreateMap<OperationStatusResponse, CertificateContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));
            Mapper.CreateMap<OperationStatusResponse, ManagementOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //OperatingSystems mapping
            Mapper.CreateMap<OperatingSystemListResponse.OperatingSystem, OSVersionsContext>();
            Mapper.CreateMap<OperationStatusResponse, OSVersionsContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //Service mapping
            Mapper.CreateMap<NSM.HostedServiceProperties, HostedServiceDetailedContext>()
                  .ForMember(c => c.Description, o => o.MapFrom(r => string.IsNullOrEmpty(r.Description) ? null : r.Description))
                  .ForMember(c => c.DateModified, o => o.MapFrom(r => r.DateLastModified));
            Mapper.CreateMap<HostedServiceGetResponse, HostedServiceDetailedContext>()
                  .ForMember(c => c.ExtendedProperties, o => o.MapFrom(r => r.Properties == null ? null : r.Properties.ExtendedProperties))
                  .ForMember(c => c.VirtualMachineRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.VirtualMachinesRoleSizes))
                  .ForMember(c => c.WebWorkerRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.WebWorkerRoleSizes))
                  .ForMember(c => c.Url, o => o.MapFrom(r => r.Uri));
            Mapper.CreateMap<HostedServiceListResponse.HostedService, HostedServiceDetailedContext>()
                  .ForMember(c => c.ExtendedProperties, o => o.MapFrom(r => r.Properties == null ? null : r.Properties.ExtendedProperties))
                  .ForMember(c => c.VirtualMachineRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.VirtualMachinesRoleSizes))
                  .ForMember(c => c.WebWorkerRoleSizes, o => o.MapFrom(r => r.ComputeCapabilities == null ? null : r.ComputeCapabilities.WebWorkerRoleSizes))
                  .ForMember(c => c.Url, o => o.MapFrom(r => r.Uri));
            Mapper.CreateMap<OperationStatusResponse, HostedServiceDetailedContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //Disk mapping
            Mapper.CreateMap<VirtualMachineDiskListResponse.VirtualMachineDisk, DiskContext>()
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.DiskSizeInGB, o => o.MapFrom(r => r.LogicalSizeInGB))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType))
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.AttachedTo, o => o.MapFrom(r => r.UsageDetails));
            Mapper.CreateMap<VirtualMachineDiskListResponse.VirtualMachineDiskUsageDetails, DiskContext.RoleReference>();

            Mapper.CreateMap<VirtualMachineDiskGetResponse, DiskContext>()
                  .ForMember(c => c.AttachedTo, o => o.MapFrom(r => r.UsageDetails))
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.DiskSizeInGB, o => o.MapFrom(r => r.LogicalSizeInGB))
                  .ForMember(c => c.IsCorrupted, o => o.MapFrom(r => r.IsCorrupted))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystemType));
            Mapper.CreateMap<VirtualMachineDiskGetResponse.VirtualMachineDiskUsageDetails, DiskContext.RoleReference>();

            Mapper.CreateMap<VirtualMachineDiskCreateResponse, DiskContext>()
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.OS, o => o.MapFrom(r => r.OperatingSystem))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.DiskSizeInGB, o => o.MapFrom(r => r.LogicalSizeInGB))
                  .ForMember(c => c.AttachedTo, o => o.MapFrom(r => r.UsageDetails));
            Mapper.CreateMap<VirtualMachineDiskCreateResponse.VirtualMachineDiskUsageDetails, DiskContext.RoleReference>();

            Mapper.CreateMap<VirtualMachineDiskUpdateResponse, DiskContext>()
                  .ForMember(c => c.DiskName, o => o.MapFrom(r => r.Name))
                  .ForMember(c => c.MediaLink, o => o.MapFrom(r => r.MediaLinkUri))
                  .ForMember(c => c.DiskSizeInGB, o => o.MapFrom(r => r.LogicalSizeInGB));

            Mapper.CreateMap<OperationStatusResponse, DiskContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            //Storage mapping
            Mapper.CreateMap<StorageAccountGetResponse, StorageServicePropertiesOperationContext>()
                  .ForMember(c => c.StorageAccountDescription, o => o.MapFrom(r => r.StorageAccount.Properties == null ? null : r.StorageAccount.Properties.Description))
                  .ForMember(c => c.StorageAccountName, o => o.MapFrom(r => r.StorageAccount.Name));
            Mapper.CreateMap<StorageAccountProperties, StorageServicePropertiesOperationContext>()
                  .ForMember(c => c.StorageAccountDescription, o => o.MapFrom(r => r.Description))
                  .ForMember(c => c.GeoPrimaryLocation, o => o.MapFrom(r => r.GeoPrimaryRegion))
                  .ForMember(c => c.GeoSecondaryLocation, o => o.MapFrom(r => r.GeoSecondaryRegion))
                  .ForMember(c => c.StorageAccountStatus, o => o.MapFrom(r => r.Status))
                  .ForMember(c => c.StatusOfPrimary, o => o.MapFrom(r => r.StatusOfGeoPrimaryRegion))
                  .ForMember(c => c.StatusOfSecondary, o => o.MapFrom(r => r.StatusOfGeoSecondaryRegion));
            Mapper.CreateMap<StorageAccount, StorageServicePropertiesOperationContext>()
                  .ForMember(c => c.StorageAccountDescription, o => o.MapFrom(r => r.Properties == null ? null : r.Properties.Description))
                  .ForMember(c => c.StorageAccountName, o => o.MapFrom(r => r.Name));
            Mapper.CreateMap<OperationStatusResponse, StorageServicePropertiesOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<StorageAccountGetKeysResponse, StorageServiceKeyOperationContext>()
                  .ForMember(c => c.Primary, o => o.MapFrom(r => r.PrimaryKey))
                  .ForMember(c => c.Secondary, o => o.MapFrom(r => r.SecondaryKey));
            Mapper.CreateMap<OperationStatusResponse, StorageServiceKeyOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            Mapper.CreateMap<OperationStatusResponse, ManagementOperationContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            // DomainJoinSettings mapping for IaaS
            Mapper.CreateMap<NSM.DomainJoinCredentials, PVM.WindowsProvisioningConfigurationSet.DomainJoinCredentials>()
                  .ForMember(c => c.Domain, o => o.MapFrom(r => r.Domain))
                  .ForMember(c => c.Username, o => o.MapFrom(r => r.UserName))
                  .ForMember(c => c.Password, o => o.MapFrom(r => SecureStringHelper.GetSecureString(r.Password)));
            Mapper.CreateMap<NSM.DomainJoinProvisioning, PVM.WindowsProvisioningConfigurationSet.DomainJoinProvisioning>()
                  .ForMember(c => c.AccountData, o => o.MapFrom(r => r.AccountData));
            Mapper.CreateMap<NSM.DomainJoinSettings, PVM.WindowsProvisioningConfigurationSet.DomainJoinSettings>()
                  .ForMember(c => c.Credentials, o => o.MapFrom(r => r.Credentials))
                  .ForMember(c => c.JoinDomain, o => o.MapFrom(r => r.DomainToJoin))
                  .ForMember(c => c.MachineObjectOU, o => o.MapFrom(r => r.LdapMachineObjectOU))
                  .ForMember(c => c.Provisioning, o => o.MapFrom(r => r.Provisioning));

            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.DomainJoinCredentials, NSM.DomainJoinCredentials>()
                  .ForMember(c => c.Domain, o => o.MapFrom(r => r.Domain))
                  .ForMember(c => c.UserName, o => o.MapFrom(r => r.Username))
                  .ForMember(c => c.Password, o => o.MapFrom(r => r.Password.ConvertToUnsecureString()));
            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.DomainJoinProvisioning, NSM.DomainJoinProvisioning>()
                  .ForMember(c => c.AccountData, o => o.MapFrom(r => r.AccountData));
            Mapper.CreateMap<PVM.WindowsProvisioningConfigurationSet.DomainJoinSettings, NSM.DomainJoinSettings>()
                  .ForMember(c => c.Credentials, o => o.MapFrom(r => r.Credentials))
                  .ForMember(c => c.DomainToJoin, o => o.MapFrom(r => r.JoinDomain))
                  .ForMember(c => c.LdapMachineObjectOU, o => o.MapFrom(r => r.MachineObjectOU))
                  .ForMember(c => c.Provisioning, o => o.MapFrom(r => r.Provisioning));

            // Networks mapping
            Mapper.CreateMap<IList<string>, PVM.AddressPrefixList>()
                  .ForItems<IList<string>, PVM.AddressPrefixList, string>();
            Mapper.CreateMap<NVM.NetworkListResponse.AddressSpace, PVM.AddressSpace>();
            Mapper.CreateMap<NVM.NetworkListResponse.Connection, PVM.Connection>();
            Mapper.CreateMap<NVM.NetworkListResponse.LocalNetworkSite, PVM.LocalNetworkSite>();
            Mapper.CreateMap<IList<NVM.NetworkListResponse.LocalNetworkSite>, PVM.LocalNetworkSiteList>()
                  .ForItems<IList<NVM.NetworkListResponse.LocalNetworkSite>, PVM.LocalNetworkSiteList, PVM.LocalNetworkSite>();
            Mapper.CreateMap<NVM.NetworkListResponse.DnsServer, PVM.DnsServer>();
            Mapper.CreateMap<IList<NVM.NetworkListResponse.DnsServer>, PVM.DnsServerList>()
                  .ForItems<IList<NVM.NetworkListResponse.DnsServer>, PVM.DnsServerList, PVM.DnsServer>();
            Mapper.CreateMap<NVM.NetworkListResponse.Subnet, PVM.Subnet>();
            Mapper.CreateMap<IList<NVM.NetworkListResponse.Subnet>, PVM.SubnetList>()
                  .ForItems<IList<NVM.NetworkListResponse.Subnet>, PVM.SubnetList, PVM.Subnet>();
            Mapper.CreateMap<IList<NVM.NetworkListResponse.DnsServer>, PVM.DnsSettings>()
                  .ForMember(c => c.DnsServers, o => o.MapFrom(r => r));
            Mapper.CreateMap<IList<NVM.NetworkListResponse.Gateway>, PVM.Gateway>();
            Mapper.CreateMap<NVM.NetworkListResponse.VirtualNetworkSite, PVM.VirtualNetworkSite>();
            Mapper.CreateMap<IList<NVM.NetworkListResponse.VirtualNetworkSite>, PVM.VirtualNetworkSiteList>()
                  .ForItems<IList<NVM.NetworkListResponse.VirtualNetworkSite>, PVM.VirtualNetworkSiteList, PVM.VirtualNetworkSite>();
            Mapper.CreateMap<NVM.NetworkListResponse.VirtualNetworkSite, VirtualNetworkSiteContext>()
                  .ForMember(c => c.AddressSpacePrefixes, o => o.MapFrom(r => r.AddressSpace == null ? null : r.AddressSpace.AddressPrefixes == null ? null :
                                                                              r.AddressSpace.AddressPrefixes.Select(p => p)))
                  .ForMember(c => c.DnsServers, o => o.MapFrom(r => r.DnsServers.AsEnumerable()))
                  .ForMember(c => c.GatewayProfile, o => o.MapFrom(r => r.Gateway.Profile))
                  .ForMember(c => c.GatewaySites, o => o.MapFrom(r => r.Gateway.Sites));
            Mapper.CreateMap<OperationStatusResponse, VirtualNetworkSiteContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()))
                  .ForMember(c => c.Id, o => o.Ignore());

            // Check Static IP Availability Response Mapping
            Mapper.CreateMap<NVM.NetworkStaticIPAvailabilityResponse, VirtualNetworkStaticIPAvailabilityContext>();
            Mapper.CreateMap<OperationStatusResponse, VirtualNetworkStaticIPAvailabilityContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()));

            // New SM to Model
            Mapper.CreateMap<NSM.StoredCertificateSettings, PVM.CertificateSetting>();
            Mapper.CreateMap<IList<NSM.StoredCertificateSettings>, PVM.CertificateSettingList>()
                  .ForItems<IList<NSM.StoredCertificateSettings>, PVM.CertificateSettingList, PVM.CertificateSetting>();

            // Model to New SM
            Mapper.CreateMap<PVM.CertificateSetting, NSM.StoredCertificateSettings>();
            Mapper.CreateMap<PVM.CertificateSettingList, IList<NSM.StoredCertificateSettings>>()
                  .ForItems<PVM.CertificateSettingList, IList<NSM.StoredCertificateSettings>, NSM.StoredCertificateSettings>();

            // Resource Extensions
            Mapper.CreateMap<NSM.ResourceExtensionParameterValue, PVM.ResourceExtensionParameterValue>()
                  .ForMember(c => c.SecureValue, o => o.MapFrom(r => SecureStringHelper.GetSecureString(r)))
                  .ForMember(c => c.Value, o => o.MapFrom(r => SecureStringHelper.GetPlainString(r)));
            Mapper.CreateMap<NSM.ResourceExtensionReference, PVM.ResourceExtensionReference>();

            Mapper.CreateMap<PVM.ResourceExtensionParameterValue, NSM.ResourceExtensionParameterValue>()
                  .ForMember(c => c.Value, o => o.MapFrom(r => SecureStringHelper.GetPlainString(r)));
            Mapper.CreateMap<PVM.ResourceExtensionReference, NSM.ResourceExtensionReference>();

            // Reserved IP
            Mapper.CreateMap<OperationStatusResponse, ReservedIPContext>()
                  .ForMember(c => c.OperationId, o => o.MapFrom(r => r.Id))
                  .ForMember(c => c.OperationStatus, o => o.MapFrom(r => r.Status.ToString()))
                  .ForMember(c => c.Id, o => o.Ignore());
            Mapper.CreateMap<NetworkReservedIPGetResponse, ReservedIPContext>()
                  .ForMember(c => c.ReservedIPName, o => o.MapFrom(r => r.Name));
            Mapper.CreateMap<NetworkReservedIPListResponse.ReservedIP, ReservedIPContext>()
                  .ForMember(c => c.ReservedIPName, o => o.MapFrom(r => r.Name));

            // Public IP
            Mapper.CreateMap<PVM.PublicIP, NSM.RoleInstance.PublicIP>();
            Mapper.CreateMap<PVM.AssignPublicIP, NSM.ConfigurationSet.PublicIP>();

            Mapper.CreateMap<NSM.RoleInstance.PublicIP, PVM.PublicIP>();
            Mapper.CreateMap<NSM.ConfigurationSet.PublicIP, PVM.AssignPublicIP>();
        }
    }
}