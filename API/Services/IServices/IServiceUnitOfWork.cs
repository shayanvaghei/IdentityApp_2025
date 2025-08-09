namespace API.Services.IServices
{
    public interface IServiceUnitOfWork
    {
        IEmailService EmailService { get; }
        ITokenService TokenService { get; }
    }
}
