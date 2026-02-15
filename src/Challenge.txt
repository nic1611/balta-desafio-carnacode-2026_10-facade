// DESAFIO: Sistema de E-commerce Complexo
// PROBLEMA: O processo de finalização de pedido envolve múltiplos subsistemas (estoque, pagamento, 
// envio, notificação, cupons) cada um com interfaces complexas. O cliente precisa conhecer e 
// orquestrar todos esses sistemas, resultando em código complexo e acoplado

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    // Contexto: E-commerce onde finalizar um pedido requer interagir com vários subsistemas
    // Cada subsistema tem sua própria API complexa com múltiplos passos
    
    // ========== SUBSISTEMA DE ESTOQUE ==========
    public class InventorySystem
    {
        private Dictionary<string, int> _stock = new Dictionary<string, int>
        {
            ["PROD001"] = 10,
            ["PROD002"] = 5,
            ["PROD003"] = 0
        };

        public bool CheckAvailability(string productId)
        {
            Console.WriteLine($"[Estoque] Verificando disponibilidade de {productId}...");
            return _stock.ContainsKey(productId) && _stock[productId] > 0;
        }

        public void ReserveProduct(string productId, int quantity)
        {
            Console.WriteLine($"[Estoque] Reservando {quantity}x {productId}");
            if (_stock.ContainsKey(productId))
                _stock[productId] -= quantity;
        }

        public void ReleaseReservation(string productId, int quantity)
        {
            Console.WriteLine($"[Estoque] Liberando reserva de {quantity}x {productId}");
            if (_stock.ContainsKey(productId))
                _stock[productId] += quantity;
        }
    }

    // ========== SUBSISTEMA DE PAGAMENTO ==========
    public class PaymentGateway
    {
        public string InitializeTransaction(decimal amount)
        {
            Console.WriteLine($"[Pagamento] Inicializando transação de R$ {amount:N2}");
            return $"TXN{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        public bool ValidateCard(string cardNumber, string cvv)
        {
            Console.WriteLine($"[Pagamento] Validando cartão {cardNumber}");
            return cardNumber.Length == 16;
        }

        public bool ProcessPayment(string transactionId, string cardNumber)
        {
            Console.WriteLine($"[Pagamento] Processando pagamento {transactionId}");
            return true;
        }

        public void RollbackTransaction(string transactionId)
        {
            Console.WriteLine($"[Pagamento] Revertendo transação {transactionId}");
        }
    }

    // ========== SUBSISTEMA DE ENVIO ==========
    public class ShippingService
    {
        public decimal CalculateShipping(string zipCode, decimal weight)
        {
            Console.WriteLine($"[Envio] Calculando frete para CEP {zipCode}");
            return 15.00m;
        }

        public string CreateShippingLabel(string orderId, string address)
        {
            Console.WriteLine($"[Envio] Criando etiqueta para pedido {orderId}");
            return $"LABEL{orderId}";
        }

        public void SchedulePickup(string labelId, DateTime date)
        {
            Console.WriteLine($"[Envio] Agendando coleta {labelId} para {date:dd/MM/yyyy}");
        }
    }

    // ========== SUBSISTEMA DE CUPONS ==========
    public class CouponSystem
    {
        private Dictionary<string, decimal> _coupons = new Dictionary<string, decimal>
        {
            ["PROMO10"] = 0.10m,
            ["SAVE20"] = 0.20m
        };

        public bool ValidateCoupon(string code)
        {
            Console.WriteLine($"[Cupom] Validando cupom {code}");
            return _coupons.ContainsKey(code);
        }

        public decimal GetDiscount(string code)
        {
            return _coupons.ContainsKey(code) ? _coupons[code] : 0;
        }

        public void MarkCouponAsUsed(string code, string customerId)
        {
            Console.WriteLine($"[Cupom] Marcando cupom {code} como usado por {customerId}");
        }
    }

    // ========== SUBSISTEMA DE NOTIFICAÇÕES ==========
    public class NotificationService
    {
        public void SendOrderConfirmation(string email, string orderId)
        {
            Console.WriteLine($"[Notificação] Enviando confirmação de pedido {orderId} para {email}");
        }

        public void SendPaymentReceipt(string email, string transactionId)
        {
            Console.WriteLine($"[Notificação] Enviando recibo de pagamento {transactionId}");
        }

        public void SendShippingNotification(string email, string trackingCode)
        {
            Console.WriteLine($"[Notificação] Enviando código de rastreamento {trackingCode}");
        }
    }

    // Problema: Cliente precisa orquestrar todos os subsistemas
    public class OrderDTO
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string CustomerEmail { get; set; }
        public string CreditCard { get; set; }
        public string Cvv { get; set; }
        public string ShippingAddress { get; set; }
        public string ZipCode { get; set; }
        public string CouponCode { get; set; }
        public decimal ProductPrice { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de E-commerce ===\n");

            // Problema: Cliente precisa conhecer e usar todos os subsistemas
            var inventory = new InventorySystem();
            var payment = new PaymentGateway();
            var shipping = new ShippingService();
            var coupon = new CouponSystem();
            var notification = new NotificationService();

            var order = new OrderDTO
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
            };

            Console.WriteLine("=== Processando Pedido (Código Complexo) ===\n");

            try
            {
                // Problema: Cliente precisa conhecer ordem correta de execução
                // e gerenciar estado de cada subsistema
                
                // Passo 1: Verificar estoque
                if (!inventory.CheckAvailability(order.ProductId))
                {
                    Console.WriteLine("❌ Produto indisponível");
                    return;
                }

                // Passo 2: Reservar produto
                inventory.ReserveProduct(order.ProductId, order.Quantity);

                // Passo 3: Validar e aplicar cupom
                decimal discount = 0;
                if (!string.IsNullOrEmpty(order.CouponCode))
                {
                    if (coupon.ValidateCoupon(order.CouponCode))
                    {
                        discount = coupon.GetDiscount(order.CouponCode);
                    }
                }

                // Passo 4: Calcular valores
                decimal subtotal = order.ProductPrice * order.Quantity;
                decimal discountAmount = subtotal * discount;
                decimal shippingCost = shipping.CalculateShipping(order.ZipCode, order.Quantity * 0.5m);
                decimal total = subtotal - discountAmount + shippingCost;

                // Passo 5: Processar pagamento
                string transactionId = payment.InitializeTransaction(total);
                
                if (!payment.ValidateCard(order.CreditCard, order.Cvv))
                {
                    inventory.ReleaseReservation(order.ProductId, order.Quantity);
                    Console.WriteLine("❌ Cartão inválido");
                    return;
                }

                if (!payment.ProcessPayment(transactionId, order.CreditCard))
                {
                    inventory.ReleaseReservation(order.ProductId, order.Quantity);
                    Console.WriteLine("❌ Pagamento recusado");
                    return;
                }

                // Passo 6: Criar envio
                string orderId = $"ORD{DateTime.Now.Ticks}";
                string labelId = shipping.CreateShippingLabel(orderId, order.ShippingAddress);
                shipping.SchedulePickup(labelId, DateTime.Now.AddDays(1));

                // Passo 7: Marcar cupom como usado
                if (!string.IsNullOrEmpty(order.CouponCode))
                {
                    coupon.MarkCouponAsUsed(order.CouponCode, order.CustomerEmail);
                }

                // Passo 8: Enviar notificações
                notification.SendOrderConfirmation(order.CustomerEmail, orderId);
                notification.SendPaymentReceipt(order.CustomerEmail, transactionId);
                notification.SendShippingNotification(order.CustomerEmail, labelId);

                Console.WriteLine($"\n✅ Pedido {orderId} finalizado com sucesso!");
                Console.WriteLine($"   Total: R$ {total:N2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao processar pedido: {ex.Message}");
            }

            Console.WriteLine("\n=== PROBLEMAS ===");
            Console.WriteLine("✗ Cliente precisa conhecer 5 subsistemas diferentes");
            Console.WriteLine("✗ Código complexo com muitos passos interdependentes");
            Console.WriteLine("✗ Alto acoplamento entre cliente e subsistemas");
            Console.WriteLine("✗ Lógica de negócio espalhada no código cliente");
            Console.WriteLine("✗ Difícil garantir consistência e tratamento de erros");
            Console.WriteLine("✗ Código repetido em diferentes pontos da aplicação");

            // Perguntas para reflexão:
            // - Como simplificar a interface para o cliente?
            // - Como encapsular a complexidade dos subsistemas?
            // - Como criar uma interface unificada e simples?
            // - Como reduzir o acoplamento entre cliente e subsistemas?
        }
    }
}
