namespace StorefrontApi.Channels;

using System.Threading.Channels;

public class PaymentChannel
{
    private readonly Channel<PaymentMessage> _channel = Channel.CreateUnbounded<PaymentMessage>();

    public ChannelWriter<PaymentMessage> Writer => _channel.Writer;
    public ChannelReader<PaymentMessage> Reader => _channel.Reader;
}

public class PaymentMessage
{
    public Guid OrderId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<PaymentMessageItem> Items { get; set; } = [];
}

public class PaymentMessageItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
