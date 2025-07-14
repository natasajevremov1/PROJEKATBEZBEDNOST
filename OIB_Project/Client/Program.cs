using Common;
using System;
using System.Security.Principal;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NetTcpBinding binding = new NetTcpBinding();
                string address = "net.tcp://localhost:9999/ServiceManagement";

                binding.Security.Mode = SecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                // Kreiraj instancu AES enkripcije
                AESAlgorithm aes = new AESAlgorithm();

                Console.WriteLine("Client process run by user: " + WindowsIdentity.GetCurrent().Name);

                EndpointAddress endpointAddress = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("wcfService"));

                using (ClientProxy proxy = new ClientProxy(binding, endpointAddress))
                {
                    while (true)
                    {
                        // Ispisivanje menija
                        Console.Clear();
                        Console.WriteLine("===== Meni =====");
                        Console.WriteLine("1. Connect");
                        Console.WriteLine("2. Run Service");
                        Console.WriteLine("3. Stop Service");
                        Console.WriteLine("4. Add to BlackList");
                        Console.WriteLine("5. Read from BlackList");
                        Console.WriteLine("6. Exit");
                        Console.Write("Izaberite opciju: ");
                        string choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                Connect(proxy, aes);
                                break;
                            case "2":
                                RunService(proxy, aes, binding);
                                break;
                            case "3":
                                StopService(proxy, aes);
                                break;
                            case "4":
                                 AddToBlacklist(proxy);
                                break;
                            case "5":
                                 ReadFromBlackList(proxy);
                                break;
                            case "6":
                                Console.WriteLine("Exiting...");
                                return; // izlazak iz petlje i programa
                            default:
                                Console.WriteLine("Nepoznata opcija, pokušajte ponovo.");
                                break;
                        }

                        // Pauza pre nego što se ponovo prikaže meni
                        Console.WriteLine("\nPritisnite bilo koji taster za povratak u meni...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void ReadFromBlackList(ClientProxy proxy)
        {
           string result =  proxy.ReadFromBlacklist();
            Console.WriteLine(result);

        }

        static void Connect(ClientProxy proxy, AESAlgorithm aes)
        {
            //Console.Write("IP :\t");
            //string ip = Console.ReadLine();
            //Console.Write("PORT :\t");
            //string port = Console.ReadLine();
            //Console.Write("PROTOCOL :\t");
            //string protocol = Console.ReadLine();

            string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            // Uspostavljanje sesije
            string sessionId = proxy.Connect(userName);

            Console.WriteLine(sessionId);
            if (string.IsNullOrEmpty(sessionId))
            {
                Console.WriteLine("Failed to establish session. Exiting...");
                return;
            }

            // Enkriptuj podatke
            //string encryptedIp = aes.Encrypt(ip.Trim(), sessionId);
            //string encryptedPort = aes.Encrypt(port.Trim(), sessionId);
            //string encryptedProtocol = aes.Encrypt(protocol.Trim(), sessionId);

            //Console.WriteLine("\nEncrypted Data:");
            //Console.WriteLine("Encrypted IP: " + encryptedIp);
            //Console.WriteLine("Encrypted PORT: " + encryptedPort);
            //Console.WriteLine("Encrypted PROTOCOL: " + encryptedProtocol);
        }

        static void RunService(ClientProxy proxy, AESAlgorithm aes, NetTcpBinding binding)
        {
            Console.Write("Enter IP for the service: ");
            string ip = Console.ReadLine();
            Console.Write("Enter Port: ");
            string port = Console.ReadLine();
            Console.Write("Enter Protocol: ");
            string protocol = Console.ReadLine();

            // Validacija unosa
            if (!IsValidIP(ip) || !IsValidPort(port) || string.IsNullOrWhiteSpace(protocol))
            {
                Console.WriteLine("Invalid input. Please try again.");
                return;
            }

            string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string sessionId = proxy.Connect(userName);

            if (string.IsNullOrEmpty(sessionId))
            {
                Console.WriteLine("Failed to establish session. Exiting...");
                return;
            }

            // Enkriptovanje podataka
            string encryptedIp = aes.Encrypt(ip.Trim(), sessionId);
            string encryptedPort = aes.Encrypt(port.Trim(), sessionId);
            string encryptedProtocol = aes.Encrypt(protocol.Trim(), sessionId);

            // Pokretanje servisa
            bool flag = proxy.RunService(encryptedIp, encryptedPort, encryptedProtocol, userName);

            if (flag)
            {

                // Testiranje veze
                string testAddress = $"net.{protocol}://{ip}:{port}/TestService";
                EndpointAddress testEndPointAddress = new EndpointAddress(new Uri(testAddress), EndpointIdentity.CreateUpnIdentity("TestService"));
                ChannelFactory<ITest> testFactory = new ChannelFactory<ITest>(binding);
                ITest testProxy = testFactory.CreateChannel(testEndPointAddress);

                Console.WriteLine("CLIENT: Service run successfully!");
            }
            else
            {
                Console.WriteLine("CLIENT: Service is either in use or on blakclist!");
            }
        }

        static void StopService(ClientProxy proxy, AESAlgorithm aes)
        {
            Console.Write("Enter IP to stop the service: ");
            string ip = Console.ReadLine();
            Console.Write("Enter Port: ");
            string port = Console.ReadLine();
            Console.Write("Enter Protocol: ");
            string protocol = Console.ReadLine();

            //Validacija unosa
            //if (!IsValidIP(ip) || !IsValidPort(port) || string.IsNullOrWhiteSpace(protocol))
            //{
            //    Console.WriteLine("Invalid input. Please try again.");
            //    return;
            //}

            string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string sessionId = proxy.Connect(userName);

            if (string.IsNullOrEmpty(sessionId))
            {
                Console.WriteLine("Failed to establish session. Exiting...");
                return;
            }

            // Enkriptovanje podataka
            string encryptedIp = aes.Encrypt(ip.Trim(), sessionId);
            string encryptedPort = aes.Encrypt(port.Trim(), sessionId);
            string encryptedProtocol = aes.Encrypt(protocol.Trim(), sessionId);

            try
            {
                // Pozivanje metode za zaustavljanje servisa
                if(proxy.StopService(encryptedIp, encryptedPort, encryptedProtocol))
                {
                    Console.WriteLine("CLIENT: Service stopped successfully!");
                }
                else
                {
                    Console.WriteLine("CLIENT: Service either not existing or error have occured!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

           
        }

        static bool IsValidIP(string ip)
        {
            //Provera validnosti IP adrese(osnovna provera)
          return System.Net.IPAddress.TryParse(ip, out _);
        }

        static bool IsValidPort(string port)
        {
            //Provera validnosti porta(0 - 65535)
          return int.TryParse(port, out int portNumber) && portNumber > 0 && portNumber <= 65535;
        }
        // Funkcija za dodavanje u blacklist
        //static void AddToBlackList(ClientProxy proxy, AESAlgorithm aes)
        //{
        //    Console.Write("Unesite IP adresu za dodavanje u BlackList: ");
        //    string ip = Console.ReadLine();

        //    // Ako je IP "localhost", pretvori u odgovarajući IP
        //    if (ip.Trim().ToLower() == "localhost")
        //    {
        //        ip = "127.0.0.1";
        //    }

        //    // Validacija IP adrese
        //    if (string.IsNullOrWhiteSpace(ip) || !System.Net.IPAddress.TryParse(ip, out _))
        //    {
        //        Console.WriteLine("Nevalidna IP adresa. Pokušajte ponovo.");
        //        return;
        //    }

        //    string userName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
        //    string sessionId = proxy.Connect(userName);

        //    if (string.IsNullOrEmpty(sessionId))
        //    {
        //        Console.WriteLine("Neuspelo uspostavljanje sesije. Izlazim...");
        //        return;
        //    }

        //    // Enkriptuj IP adresu
        //    string encryptedIp = aes.Encrypt(ip.Trim(), sessionId);

        //    // Dodaj u BlackList na serveru
        //    try
        //    {
        //        proxy.AddItemToBlacklist("ip", encryptedIp);
        //        Console.WriteLine($"IP adresa {ip} uspešno dodata u BlackList.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Greška prilikom dodavanja IP adrese u BlackList: {ex.Message}");
        //    }
        //}

        static void AddToBlacklist(ClientProxy proxy)
        {
            string ipAddress = string.Empty;
            int port = 0;
            string protocol = string.Empty;
            Console.WriteLine("\n1. Dodaj IP adresu");
            Console.WriteLine("2. Dodaj port");
            Console.WriteLine("3. Dodaj protokol");
            Console.WriteLine("4. Izlaz");
            Console.Write("Izaberite opciju: ");

            string izbor = Console.ReadLine();

            switch (izbor)
            {
                case "1":
                    Console.Write("Unesite IP adresu(npr 155.10.0.0): ");
                     ipAddress = Console.ReadLine();
                    proxy.AddItemToBlacklist("ip", ipAddress);
                    Console.WriteLine("IP adresa je uspesno dodata.");
                    break;

                case "2":
                    Console.Write("Unesite port: ");
                    if (int.TryParse(Console.ReadLine(), out port))
                    {
                        proxy.AddItemToBlacklist("port", port.ToString());
                        Console.WriteLine("Port je uspesno dodat.");
                    }
                    else
                    {
                        Console.WriteLine("Neispravan unos. Molimo unesite broj.");
                    }
                    break;

                case "3":
                    Console.Write("Unesite protokol (npr. HTTP, HTTPS, FTP): ");
                    protocol = Console.ReadLine();
                    proxy.AddItemToBlacklist("protokol", protocol);
                    Console.WriteLine("Protokol je uspesno dodat.");
                    break;

        

                case "4":
                    
                    return;

                default:
                    Console.WriteLine("Nepostojeca opcija. Pokusajte ponovo.");
                    break;
            }

            
        }

    }

}
