using System;
using System.Linq;
using System.Threading;

namespace VirtualCardSystem
{
    public class Card
    {
        private decimal balance;

        public Card(string name, string number, string cvv, DateTime expiry, string pin)
        {
            //name reg ex validation should be used
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name must not be null or empty", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(number) || number.Length != 16 || !number.All(char.IsDigit))
            {
                throw new ArgumentException("number must be 16 digit", nameof(number));
            }

            if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
            {
                throw new ArgumentException("cvv must be 3 digit number", nameof(cvv));
            }

            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4 || !pin.All(char.IsDigit))
            {
                throw new ArgumentException("pin must be 4 digit number", nameof(pin));
            }

            Name = name;
            Number = number;
            CVC = cvv;
            if (expiry < DateTime.Now) throw new ArgumentException("Invalid expiry date");
            ExpiryDate = expiry;
            Pin = pin;
        }

        public string Number { get; private set; }
        public string CVC { get; private set; }
        public DateTime ExpiryDate { get; private set; }

        public string Name { get; private set; }
        private string Pin { get; set; }

        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public decimal GetBalance()
        {
            decimal availableBalance;
            locker.EnterReadLock();
            availableBalance = balance;
            locker.ExitReadLock();
            return availableBalance;
        }

        private void VerifyPin(string pin)
        {
            if (pin != Pin)
            {
                throw new InvalidOperationException("wrong pin");
            }
        }

        private void VerifyExpiry()
        {
            if (ExpiryDate < DateTime.Now)
            {
                throw new InvalidOperationException("card is expired");
            }
        }

        public Card WithdrawBy(decimal amount, string pin)
        {
            //can be promoted to decorator method
            VerifyPin(pin);
            VerifyExpiry();

            if (GetBalance() < amount)
            {
                throw new InvalidOperationException("can't withdraw more than available balance");
            }
            locker.EnterWriteLock();
            balance -= amount;
            locker.ExitWriteLock();
            return this;
        }

        public Card TopupBy(decimal amount, string pin)
        {
            VerifyPin(pin);
            VerifyExpiry();
            if (amount < 0)
            {
                throw new InvalidOperationException("can't top up by negative amount");
            }

            // if any complex than addition then should be wrapped in try catch
            locker.EnterWriteLock();
            balance += amount;
            locker.ExitWriteLock();
            return this;
        }
    }
}
