using System;

namespace VirtualCardSystem
{
    public class CardBuilder
    {
        private string name;

        private string number;

        private string pin;

        private DateTime expiry;

        private string cvv;

        public CardBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public CardBuilder WithNumber(string number)
        {
            this.number = number;
            return this;
        }

        public CardBuilder WithPin(string pin)
        {
            this.pin = pin;
            return this;
        }

        public CardBuilder WithExpiry(DateTime expiry)
        {
            this.expiry = expiry;
            return this;
        }

        public CardBuilder WithCvv(string cvv)
        {
            this.cvv = cvv;
            return this;
        }
        public Card Build()
        {
            return new Card(name, number, cvv, expiry, pin);
        }
    }
}
