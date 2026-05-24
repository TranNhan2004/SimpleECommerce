using SimpleECommerceBackend.Domain.Attributes;

namespace SimpleECommerceBackend.Domain.Enums;

public enum PaymentMethod
{
    [DisplayValue("credit-card")]
    CreditCard = 1,

    [DisplayValue("debit-card")]
    DebitCard = 2,

    [DisplayValue("cash")]
    Cash = 3,

    [DisplayValue("bank-transfer")]
    BankTransfer = 4,

    [DisplayValue("e-wallet")]
    EWallet = 5
}
