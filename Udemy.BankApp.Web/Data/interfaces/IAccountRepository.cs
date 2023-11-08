using Udemy.BankApp.Web.Data.Entities;

namespace Udemy.BankApp.Web.Data.interfaces
{
    public interface IAccountRepository
    {
        public void Create(Account account);
    }
}
