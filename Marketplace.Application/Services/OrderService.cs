
using AutoMapper;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;


namespace Marketplace.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);

                if (order == null)
                    throw new ArgumentException("Order does not found");
                

                if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                    throw new InvalidOperationException($"Order cannot be cancelled when status is {order.Status}");

                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                await RestoreStockForOrder(order);

                _orderRepository.Update(order);

                _logger.LogInformation($"order with Id = {orderId} cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", orderId);
                throw;

            }
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, int userId)
        {
            using var transaction = await _orderRepository.BeginTransactionAsync()
            try
            {
                var cart = await _shoppingCartRepository.GetCartWithItemsAsync(userId);

                if (cart == null || !cart.Items.Any())
                    throw new InvalidOperationException("Cart is empty");


                foreach (var item in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);

                    if(product == null)
                        throw new ArgumentException($"product with id = {item.ProductId} does not exist");

                    if (product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product with id {item.ProductId}");

                }

                var order = _mapper.Map<Order>(createDto);

                order.IsActive = true;
                order.OrderDate = DateTime.UtcNow;
                order.CustomerId = userId;
                

                decimal subtotal = 0;
                decimal discountAmount = 0;

                foreach (var item in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);

                    var productFinalPrice = item.Product.FinalPrice;
                    var totalPrice = item.TotalPrice;

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        TotalPrice = totalPrice,
                        UnitPrice = productFinalPrice,
                        Quantity = item.Quantity,
                        DiscountApplied = product.DiscountedPrice.HasValue ?
                                   ((product.Price - product.DiscountedPrice.Value) * item.Quantity) : 0,

                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    order.OrderItems.Add(orderItem);
                    subtotal += totalPrice;
                    discountAmount += orderItem.DiscountApplied ?? 0;

                    product.StockQuantity -= item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;

                    _productRepository.Update(product);


                }

                order.Subtotal = subtotal;
                order.DiscountAmount = discountAmount;
                order.TaxAmount = subtotal * 0.14m;
                order.ShippingCost = CalculateShippingCost(order);
                order.TotalAmount = order.Subtotal + order.ShippingCost + order.TaxAmount - order.DiscountAmount;

                await _orderRepository.AddAsync(order);


                await _shoppingCartRepository.ClearCartAsync(userId);

               
                await transaction.

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error");
            }
        }

        private decimal CalculateShippingCost(Order order)
        {
            // Simple shipping calculation based on total amount
            // Free shipping for orders over $100
            if (order.Subtotal >= 1000)
                return 0;

            // Base shipping cost
            return 30.00m; 
        }



        public Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetOrderCountAsync(int? sellerId = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalSalesAsync(int? sellerId = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OrderExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task RestoreStockForOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}
