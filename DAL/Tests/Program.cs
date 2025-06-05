namespace DAL.Tests
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await DatabaseConnectionTest.TestConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
} 