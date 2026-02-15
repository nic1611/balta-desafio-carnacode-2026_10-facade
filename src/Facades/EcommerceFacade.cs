public class EcommerceFacade
{
    private InventorySystem _inventory;
    private PaymentGateway _payment;
    private ShippingService _shipping;
    private CouponSystem _coupon;
    private NotificationService _notification;

    public EcommerceFacade()
    {
        _inventory = new InventorySystem();
        _payment = new PaymentGateway();
        _shipping = new ShippingService();
        _coupon = new CouponSystem();
        _notification = new NotificationService();
    }

    public void FinalizeOrder(OrderDTO order)
    {
        // Orquestração de todos os subsistemas
        if (!_inventory.CheckAvailability(order.ProductId))
        {
            throw new Exception("Produto indisponível");
        }

        _inventory.ReserveProduct(order.ProductId, order.Quantity);

        var transactionId = _payment.InitializeTransaction(order.ProductPrice * order.Quantity);
        if (!_payment.ValidateCard(order.CreditCard, order.Cvv))
        {
            throw new Exception("Cartão inválido");
        }

        if (!_payment.ProcessPayment(transactionId, order.CreditCard))
        {
            throw new Exception("Falha no pagamento");
        }

        var shippingLabel = _shipping.CreateShippingLabel(order.OrderId, order.ShippingAddress);
        _shipping.SchedulePickup(shippingLabel, DateTime.Now.AddDays(3));

        if (!_coupon.ValidateCoupon(order.CouponCode))
        {
            throw new Exception("Cupom inválido");
        }

        var discount = _coupon.GetDiscount(order.CouponCode);
        _coupon.MarkCouponAsUsed(order.CouponCode, order.CustomerId);

        _notification.SendOrderConfirmation(order.CustomerEmail, order.OrderId);
        _notification.SendPaymentReceipt(order.CustomerEmail, transactionId);
        _notification.SendShippingNotification(order.CustomerEmail, shippingLabel);
    }
}