namespace Subnet_Mask_Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            do
            {

                Console.Clear();
                Console.WriteLine("Subnet Mask and Network Address Calculator");
                Console.WriteLine("----------------------");

                Console.WriteLine("Enter an IP address (e.g. 192.168.1.1): ");
                string ipAddress = Console.ReadLine();
                var segments = ipAddress.Split('.');
                while (ipAddress == string.Empty || segments.Length != 4)
                {
                    Console.WriteLine("\nPlease enter 4 numbers (0-255) separated with a dot.");
                    ipAddress = Console.ReadLine();
                    segments = ipAddress.Split('.');
                }
                
                Console.WriteLine("\nEnter the prefix length (e.g. 24): ");
                int prefixLength;
                while (!int.TryParse(Console.ReadLine(), out prefixLength) || prefixLength < 0 || prefixLength > 32)
                {
                    Console.WriteLine("\nInvalid input. Please enter a valid prefix length (0-32): ");
                }
                
                IPAddress ip = new IPAddress(segments, prefixLength);

                Console.WriteLine("\nWould you like to calculate another subnet mask and network address? (y/n): ");
                string response = Console.ReadLine().ToLower();
                if (response != "y")
                    exit = true;
            } while (!exit);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
    public class IPAddress
    {
        public byte ip1 { get; set; }   
        public byte ip2 { get; set; }       
        public byte ip3 { get; set; }   
        public byte ip4 { get; set; }
        public int prefixLength { get; set; }
        

        public IPAddress(string[] segments, int prefixLength)
        {
            ip1 = byte.Parse(segments[0]);
            ip2 = byte.Parse(segments[1]);
            ip3 = byte.Parse(segments[2]);
            ip4 = byte.Parse(segments[3]);
            this.prefixLength = prefixLength;
            Display();
        }

        public void Display()
        {
            Console.WriteLine($"\nIP Address: {ip1}.{ip2}.{ip3}.{ip4}/{prefixLength}");
            Console.WriteLine($"Subnet Mask: {CalculateSubnetMask(prefixLength)}");
            Console.WriteLine($"Network Address: {NetworkAddress(prefixLength)}");
            Console.WriteLine($"First Usable Address: {firstUsableAddress(prefixLength)}"); 
            Console.WriteLine($"Last Usable Address: {lastUsableAddress(prefixLength)}");
            Console.WriteLine($"Broadcast Address: {broadcastAddress(prefixLength)}");  
            Console.WriteLine($"Total Usable Hosts: {totalHosts(prefixLength)}");
        }   

        public static string CalculateSubnetMask(int prefixLength)
        {
            uint mask = uint.MaxValue << (32 - prefixLength); 
                // MaxValue equals 4 294 967 295 in decimal, it's 32 bits of 1s.
                // (32 - prefixLength) equals the number of 0s to add at the end,
                // so shifting left by this amount creates the subnet mask.
            return string.Join(".", 
                (mask >> 24) & 0xFF,
                // Shifts mask right 24 bits.
                // 0xFF equals 255 in decimal, it's 8 bits of 1s.
                // & is bitwise AND, it will turn all 24 leading bits of mask to zero
                // because the mask's total size is 32 bits.
                (mask >> 16) & 0xFF,
                // Shifts mask right 16 bits and turn all 24 leading bits of the mask to zero.
                (mask >> 8) & 0xFF,
                // Shifts mask right 8 bits and turn all 24 leading bits of the mask to zero.
                mask & 0xFF);
                // Just turn all 24 leading bits of mask to zero.
        }
        
        public string NetworkAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4); 
                // Combine the four 8 bit bytes into a single 32 bit uint value by shifting and ORing.
                // Shifting will preserve the leading bits of the 8 bit bytes
                // because of the explicit conversion to a 32 bit uint.
                // Bitwise OR will always return 1 if either of the combined values are 1,
                // so all of the bits of the combined bytes are preserved in the final product.
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
                // Turn the trailing bits of the ip number to zero
                // by combining with the mask by using bitwise AND.
            return string.Join(".",
                (network >> 24) & 0xFF,
                (network >> 16) & 0xFF,
                (network >> 8) & 0xFF,
                network & 0xFF);
        }
        public string firstUsableAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint firstUsable = network + 1;
                // Get the first usable ip address by adding 1 to the network address.
            return string.Join(".",
                (firstUsable >> 24) & 0xFF,
                (firstUsable >> 16) & 0xFF,
                (firstUsable >> 8) & 0xFF,
                firstUsable & 0xFF);
        }
        public string lastUsableAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint broadcast = network | ~mask; 
                // ~ is bitwise NOT, it creates the bitwise opposite of the mask value.
                // Combining the opposite of the mask with the network address
                // will produce it's maximum value, because it will turn all of
                // the trailing bits of the network address to 1.
                // The result equals the broadcast address.
            uint lastUsable = broadcast - 1;
                // Get the last usable ip address by subtracting 1 from the broadcast address.
            return string.Join(".",
                (lastUsable >> 24) & 0xFF,
                (lastUsable >> 16) & 0xFF,
                (lastUsable >> 8) & 0xFF,
                lastUsable & 0xFF);
        }
        public string broadcastAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint broadcast = network | ~mask;
            return string.Join(".",
                (broadcast >> 24) & 0xFF,
                (broadcast >> 16) & 0xFF,
                (broadcast >> 8) & 0xFF,
                broadcast & 0xFF);
        }
        public static int totalHosts(int prefixLength)
        {
            return (int)(Math.Pow(2, 32 - prefixLength) - 2);
                // Total usable hosts equals 2^(32 - prefixLength) - 2.
                // The subtraction of 2 accounts for the network and broadcast addresses,
                // which cannot be assigned to hosts.
        }
    }
}
