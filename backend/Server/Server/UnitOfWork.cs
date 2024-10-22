using Server;
using Server.Repositories;


public class UnitOfWork
{
    private readonly FarminhouseContext _context;

    private OrderRepository _orderRepository;
    private PaymentRepository _paymentRepository;
    private PaymentsTypeRepository _paymentsTypeRepository;
    private ProductRepository _productRepository;
    private ReviewRepository _reviewRepository;
    private UserRepository _userRepository;

    public OrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);
    public PaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);
    public PaymentsTypeRepository PaymentsTypeRepository => _paymentsTypeRepository ??= new PaymentsTypeRepository(_context);
    public ProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
    public ReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_context);
    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    public UnitOfWork(FarminhouseContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
