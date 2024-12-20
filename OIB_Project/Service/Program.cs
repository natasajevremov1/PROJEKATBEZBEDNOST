﻿using Common;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/ServiceManagement";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(ServiceManagement));

            host.AddServiceEndpoint(typeof(IServiceManagement), binding, address);

            // podesavamo da se koristi MyAuthorizationManager umesto ugradjenog
            host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

            // podesavamo custom polisu, odnosno nas objekat principala
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            // Podesavanje AuditBehavior-a
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();

            Console.WriteLine("Service process run by user: " + WindowsIdentity.GetCurrent().Name);
            Console.WriteLine("Service running...");

            Console.WriteLine("\nTesting BlacklistManager...");

            string blacklistFilePath = "blacklist.txt"; // Putanja do blacklist fajla
            Console.WriteLine($"Blacklist file path: {Path.GetFullPath(blacklistFilePath)}");
            
           
            

            // Ispis svih lista
            Console.WriteLine("\n--- Current Blacklists ---");
            Console.WriteLine("Ports:");
            foreach (var port in Database.ports)
            {
                Console.WriteLine($" - {port}");
            }

            Console.WriteLine("IPs:");
            foreach (var ip in Database.ips)
            {
                Console.WriteLine($" - {ip}");
            }

            Console.WriteLine("Protocols:");
            foreach (var protocol in Database.protocols)
            {
                Console.WriteLine($" - {protocol}");
            }
            Console.ReadLine();
        }
    }
}
