
INotifier notifier = new MailManager();
var submiter = new ProductSubmitterBuilder()
    .WithNotify(notifier)
    .SkipValidate()
    .Build();

var product = new Product()
{
    Name = "tag",
    Length = 3
};
try
{
    submiter.Submit(product);
}
catch (Exception ex)
{

    Console.WriteLine(ex.Message); 
}

interface INotifier
{
    void Notify(string msg);
}
class MailManager : INotifier
{
    public void Notify(string msg)
    {
        Console.WriteLine($"send mail, msg : {msg}");
    }
}

class Product 
{
    public string Name { get; set; }
    public double Length { get; set; }
}

interface IProductValidator
{
    void Validate(Product product);
}
class ProdcutValidator : IProductValidator
{

    public void Validate(Product product)
    {
        if (string.IsNullOrEmpty(product.Name))
        {
            throw new Exception("product name error");
        }
        if(product.Length < 10)
        {
            throw new Exception("product length must over 10");
        }
    }
}
class SkippingProductValidator : IProductValidator
{
    public void Validate(Product product)
    {
    }
}


interface IProductSubmiter
{
    void Submit(Product product);
}

class ProductSubmitterBuilder
{
    private bool _skipValidate;
    private bool _withNotify;
    private INotifier _notifier;

    public ProductSubmitterBuilder SkipValidate()
    {
        _skipValidate = true;
        return this;
    }

    public ProductSubmitterBuilder WithNotify(INotifier notifier)
    {
        _notifier = notifier;
        _withNotify = true; 
        return this;
    }


    public IProductSubmiter Build()
    {
        IProductValidator validator = CreateProductValidator();
        IProductSubmiter submiter = new ProductSubmiter(validator);
        if (_withNotify)
        {
            submiter = new NotifyProdutSubmitter(submiter, _notifier);
        }
        return submiter;
    }

    private IProductValidator CreateProductValidator()
    {
        if(_skipValidate)
        {
            return new SkippingProductValidator();
        }
        return new ProdcutValidator();
    }
}

class NotifyProdutSubmitter : IProductSubmiter
{
    private readonly IProductSubmiter _sumitter;
    private readonly INotifier _notifier;

    public NotifyProdutSubmitter(IProductSubmiter sumitter, INotifier notifier)
    {
        _sumitter = sumitter;
        _notifier = notifier;
    }

    public void Submit(Product product)
    {
        _sumitter.Submit(product);
        _notifier.Notify($"product {product.Name} is finsih, length is {product.Length}");
        
    }
}
class ProductSubmiter : IProductSubmiter
{
    private readonly IProductValidator _validator;


    public ProductSubmiter(IProductValidator validator)
    {
        _validator = validator;
    }

    public void Submit(Product product)
    {
        _validator.Validate(product);
        Console.WriteLine("submitting");
        Thread.Sleep(500);
        Console.WriteLine("sucessfully");
    }
}