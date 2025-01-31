﻿using Microsoft.AspNetCore.Identity;
using WebApi.Helpers.Repositories;
using WebApi.Models.Dtos;
using WebApi.Models.Entities;
using WebApi.Models.Schemas;
using WebApi.Models.Interfaces;

namespace WebApi.Helpers.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly CreditCardRepository _creditCardRepo;
        private readonly UserProfileCreditCardRepository _userProfileCreditCardRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public PaymentService(CreditCardRepository creditCardRepo, UserManager<IdentityUser> userManager, UserProfileCreditCardRepository userProfileCreditCardRepo)
        {
            _creditCardRepo = creditCardRepo;
            _userManager = userManager;
            _userProfileCreditCardRepo = userProfileCreditCardRepo;
        }

        public async Task<IEnumerable<CreditCardDto>> GetUserCreditCardsAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userName);
                var result = await _userProfileCreditCardRepo.GetListAsync(x => x.UserProfileId == user!.Id);
                if (result != null)
                {
                    List<CreditCardDto> creditCards = new List<CreditCardDto>();
                    foreach (var item in result)
                    {
                        var card = await _creditCardRepo.GetAsync(x => x.Id == item.CreditCardId);
                        creditCards.Add(card);
                    }
                    return creditCards;
                }
            }
            catch { }
            return null!;
        }
        public async Task<bool> RegisterCreditCardsAsync(RegisterCreditCardSchema schema, string userName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userName);
                var existingCard = await _creditCardRepo.GetAsync(x => x.CardNo == schema.CardNo && x.NameOnCard == schema.NameOnCard && x.CVV == schema.CVV && x.Expires.Month == schema.ExpireMonth && x.Expires.Year == schema.ExpireYear);

                if (existingCard != null)
                {
                    var result = await _userProfileCreditCardRepo.AddAsync(new UserProfileCreditCardEntity { CreditCardId = existingCard.Id, CreditCard = existingCard, UserProfileId = user!.Id });
                    if (result != null)
                        return true;
                }
                else
                {
                    CreditCardEntity entity = schema;
                    var newCard = await _creditCardRepo.AddAsync(entity);
                    if (newCard != null)
                    {
                        await _userProfileCreditCardRepo.AddAsync(new UserProfileCreditCardEntity { CreditCardId = newCard.Id, CreditCard = newCard, UserProfileId = user!.Id });
                        return true;
                    }
                }
            }
            catch { }
            return false!;
        }
        public async Task<bool> DeleteCreditCardsAsync(int id, string userName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userName);
                var result = await _userProfileCreditCardRepo.GetAsync(x => x.UserProfileId == user!.Id && x.CreditCardId == id);
                await _userProfileCreditCardRepo.DeleteAsync(result);
                return true;
            }
            catch { }
            return false!;
        }
    }
}
