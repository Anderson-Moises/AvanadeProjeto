using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Messages;

namespace VendasService.Mensagem
{
    public interface IRabbitMQPublisher
    {
        void PublicarPedido(VendaMessage venda);
    }
}