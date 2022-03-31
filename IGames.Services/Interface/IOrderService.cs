using GemBox.Document;
using System;
using System.Collections.Generic;
using IGames.Domain.DomainModels;

namespace IGames.Services.Interface
{
    public interface IOrderService
    {
        List<Order> getAllOrders();
        Order getOrderDetails(BaseEntity model);
        DocumentModel CreateOrderInvoice(Guid orderId);
    }
}