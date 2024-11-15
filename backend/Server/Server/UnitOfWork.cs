using Server;
using Server.Models;
using Server.Repositories;


public class UnitOfWork
{
    private readonly FarminhouseContext _context;

    private OrderRepository _orderRepository;
    private PaymentsTypeRepository _paymentsTypeRepository;
    private ProductRepository _productRepository;
    private ReviewRepository _reviewRepository;
    private UserRepository _userRepository;
    private CategoryRepository _categoryRepository;

    //Nuevas tablas
    private ShoppingCartRepository _shoppingCartRepository;
    private TemporalOrderRepository _temporalOrderRepository;
    private CartContentRepository _cartContentRepository;


    public OrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);
    public PaymentsTypeRepository PaymentsTypeRepository => _paymentsTypeRepository ??= new PaymentsTypeRepository(_context);
    public ProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context);
    public ReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_context);
    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
    public CategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_context);

    //Nuevas tablas
    public ShoppingCartRepository ShoppingCartRepository => _shoppingCartRepository ??= new ShoppingCartRepository(_context);
    public TemporalOrderRepository TemporalOrderRepository => _temporalOrderRepository ??= new TemporalOrderRepository(_context);
    public CartContentRepository CartContentRepository => _cartContentRepository ??= new CartContentRepository(_context);


    public UnitOfWork(FarminhouseContext context)
    {
        _context = context;
    }

    public FarminhouseContext Context => _context;

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
