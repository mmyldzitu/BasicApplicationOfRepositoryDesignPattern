using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udemy.BankApp.Web.Data.Context;
using Udemy.BankApp.Web.Data.Entities;
using Udemy.BankApp.Web.Data.interfaces;
using Udemy.BankApp.Web.Data.Repositories;
using Udemy.BankApp.Web.Mappings;
using Udemy.BankApp.Web.Models;

namespace Udemy.BankApp.Web.Controllers
{
    public class AccountController : Controller
    {

        //private readonly IAccountMapper _accountMapper;
        //private readonly IApplicationUserRepository _applicationUserRepository;
        //private readonly IUserMapper _userMapper;
        //private readonly IAccountRepository _accountRepository;

        //public AccountController( IApplicationUserRepository applicationUserRepository, IUserMapper userMapper, IAccountRepository accountRepository, IAccountMapper accountMapper)
        //{


        //    _applicationUserRepository = applicationUserRepository;
        //    _userMapper = userMapper;
        //    _accountRepository = accountRepository;
        //    _accountMapper = accountMapper;
        //}

        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<ApplicationUser> _userRepository;

        public AccountController(IGenericRepository<Account> accountRepository, IGenericRepository<ApplicationUser> userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        public IActionResult Create(int id)
        {
            var userInfo = _userRepository.GetById(id);
            return View(new UserListModel {
                Id=userInfo.Id,
            Name=userInfo.Name,
            Surname=userInfo.Surname
            
            });
        } 
        [HttpPost]
        public IActionResult Create(AccountCreateModel model)
        {
            _accountRepository.Create(new Account { 
            AccountNumber=model.AccountNumber,
            ApplicationUserId=model.ApplicationUserId,
            Balance=model.Balance

            });
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult GetByUserId(int userId)
        {
            var query=_accountRepository.GetQueryable();
            var accountList=query.Where(x => x.ApplicationUserId == userId).ToList();
            var user = _userRepository.GetById(userId);
            var list = new List<AccountListModel>();
            ViewBag.FullName = user.Name + " " + user.Surname;
            foreach ( var account in accountList)
            {
                list.Add(new() { 
                AccountNumber=account.AccountNumber,
                ApplicationUserId=account.ApplicationUserId,
                Balance=account.Balance,
                
                Id=account.Id
                });
            }
            return View(list);
        }
        [HttpGet]
        public IActionResult SendMoney(int accountId)
        {
            
            var query = _accountRepository.GetQueryable();
            var balanceofthis = query.SingleOrDefault(x => x.Id == accountId).Balance;
            ViewBag.balanceofthis = balanceofthis;
            var accounts = query.Where(x => x.Id != accountId).ToList();
            var list = new List<AccountListModel>();
            ViewBag.senderId = accountId;
            foreach ( var item in accounts)
            {
                list.Add(new()
                {
                    AccountNumber = item.AccountNumber,
                    ApplicationUserId=item.ApplicationUserId,
                    Balance = item.Balance,
                    Id=item.Id

                });;
               
            }

            return View(new SelectList(list,"Id","AccountNumber"));
        }
        [HttpPost]
        public IActionResult SendMoney(SendMoneyModel model)
        {
            var senderAccount = _accountRepository.GetById(model.senderId);
            var account=_accountRepository.GetById(model.AccountId);
            senderAccount.Balance -= model.Amount;
            _accountRepository.Update(senderAccount);
            account.Balance += model.Amount;
            _accountRepository.Update(account);


            return RedirectToAction("Index", "Home");
        }
    }
}
