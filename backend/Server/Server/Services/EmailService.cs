using Server.Mappers;
using Server.Models;
using System.Net;
using System.Net.Mail;

namespace Server.Services
{
    public class EmailService
    {
        UnitOfWork _unitOfWork;
        ProductMapper _productMapper;
        public EmailService(UnitOfWork unitOfWork,ProductMapper productMapper) 
        {
            _unitOfWork = unitOfWork;
            _productMapper = productMapper;
        }

        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;
        private const string EMAIL_FROM = "farminhouse447@gmail.com";
        private const string PASSWORD_EMAIL_FROM = "weiqxfzteoxaysbu";
        public  async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            using SmtpClient client = new SmtpClient(SMTP_HOST, SMTP_PORT)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EMAIL_FROM, PASSWORD_EMAIL_FROM)
            };

            MailMessage mail = new MailMessage(EMAIL_FROM, to, subject, body)
            {
                IsBodyHtml = isHtml,
            };

            await client.SendMailAsync(mail);
        }
        public async Task CreateEmailUser(User user, Wishlist productsorder,int paymetId)
        {
            decimal totalprice = 0;
            string to = user.Email;
            string subject = "Envio de la compra realizada";
            string body = "<html> <h1>QUE BISHO GRACIAS POR COMPRAR</h1> <table><tr><td>Nombre</td><td>Imagen</td><td>Precio</td><td>Cantidad</td><td>Suma</td></tr>";
            foreach (ProductsToBuy products in productsorder.Products)
            {
                decimal price = 0;
                decimal totalpricequantity = 0;
                Product oneproduct = await _unitOfWork.ProductRepository.GetFullProductById(products.ProductId);
                price = oneproduct.Price / 100m;
                totalpricequantity = (products.Quantity * oneproduct.Price) / 100m;
                body += $"<tr><td>{oneproduct.Name}</td><td><img src='{_productMapper.AddCorrectPath(oneproduct)}'></td><td>{price}€</td><td>{products.Quantity}</td><td>{totalpricequantity}€</td></tr>";
                totalprice += products.Quantity * oneproduct.Price;
            }
            body += "</table></html>";
            body += $"<h2>Su pedido ha costado: {totalprice / 100}€</h2>";
            if (paymetId == 1)
            {
                body += $"<h3>Metodo de pago: Tarjeta</h3>";
            }
            else
            {
                body += $"<h3>Metodo de pago: Etherium</h3>";
            }
            body += $"<h4>Direccion de envio: {user.Address}</h4>";
            await SendEmailAsync(to, subject, body, true);
        }
    }
}
