using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Threading.Tasks;
using VirtualCard;
using VirtualCardSystem;

namespace Tests
{
    [TestFixture]
    public class CardTests
    {
        const string validNumber = "1234123412341234";
        const string validName = "Rajnikant Dankhara";
        const string validPin = "1234";
        DateTime validExpiry = DateTime.Now.AddDays(2);
        const string validCvv = "123";
        const decimal validTopupAmount = 100;
        const decimal validWithdrawalAmount = 10;

        [SetUp]
        public void Setup()
        {
        }

        [TestCase(null)]
        [TestCase(" ")]
        public void GivenNameIsNullOrEmptyString_ItThrowsArgumentNullException(string name)
        {
            Assert.Throws<ArgumentException>(() => new Card(name, validNumber, validCvv, validExpiry, validPin));
        }

        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("123")]
        [TestCase("12341234123412341")]
        public void GivenCardNumberIsInvalidString_ItThrowsArgumentNullException(string number)
        {
            Assert.Throws<ArgumentException>(() => new Card(validName, number, validCvv, validExpiry, validPin));
        }

        [Test]
        public void GivenAllValidCardInput_ItShouldNotThrowAnyException()
        {
            Assert.DoesNotThrow(() => new Card(validName, validNumber, validCvv, validExpiry, validPin));
        }

        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("12")]
        [TestCase("12345")]
        public void Given_InvalidPin_ItShouldThrowArgumentException(string pin)
        {
            Assert.Throws<ArgumentException>(() => new Card(validName, validNumber, validCvv, validExpiry, pin));
        }

        [Test]
        public void Given_InputSuppliedByRandomise_BuilderShould_AbleToCreateCardSuccessfuly()
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithName(validName)
                .WithNumber(CardNumberProvider.GetNewCardNumber())
                .WithCvv(CardNumberProvider.GetNewCvv())
                .WithPin(CardNumberProvider.GetNewPin())
                .WithExpiry(DateTime.Now.AddDays(2));

            Assert.DoesNotThrow(() => cardBuilder.Build());
        }

        //Not adding all input tests due to time constraint and jumping to Transaction tests
        private Card CreateValidCard()
        {
            CardBuilder cardBuilder = new CardBuilder()
                .WithName(validName)
                .WithNumber(CardNumberProvider.GetNewCardNumber())
                .WithCvv(CardNumberProvider.GetNewCvv())
                .WithPin(validPin)
                .WithExpiry(DateTime.Now.AddDays(2));

            return cardBuilder.Build();
        }

        [Test]
        public void Given_ValidCard_It_Should_TopUp_Balance()
        {
            Card card = CreateValidCard();

            Assert.That(card.GetBalance() == 0);

            Assert.That(card.TopupBy(100, validPin).GetBalance(), Is.EqualTo(100));
        }

        [Test]
        public void Given_CardHasEnoughBalance_ItShouldBeUsedByManyPlaces_AtSameTime()
        {
            Card card = CreateValidCard();
            card.TopupBy(1000, validPin);

            Parallel.Invoke(() => card.WithdrawBy(100, validPin),
                () => card.WithdrawBy(100, validPin),
                () => card.WithdrawBy(100, validPin),
                () => card.WithdrawBy(100, validPin),
                () => card.WithdrawBy(100, validPin),
                () => card.WithdrawBy(100, validPin));

            Assert.That(card.GetBalance(), Is.EqualTo(400));
        }

        [Test]
        public void WhenTry_ToWithdrawMoreThanAvailableBalance_ItThrowsInvalidOperationException()
        {
            Card card = CreateValidCard();
            card.TopupBy(1000, validPin);
            Assert.Throws<InvalidOperationException>(() => card.WithdrawBy(2000, validPin));
        }
    }
}