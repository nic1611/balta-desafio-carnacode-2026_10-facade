public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Sistema de E-commerce ===\n");

        var facade = new EcommerceFacade();
        facade.FinalizeOrder(new OrderDTO
        {
            ProductId = "PROD001",
            Quantity = 2,
            CustomerEmail = "cliente@email.com",
            CreditCard = "1234567890123456",
            Cvv = "123",
            ShippingAddress = "Rua Exemplo, 123",
            ZipCode = "12345-678",
            CouponCode = "PROMO10",
            ProductPrice = 100.00m
        });

        Console.WriteLine("Pedido finalizado com sucesso!");
    }
}