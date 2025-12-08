namespace Payments.App.Utils
{
    public class DecimalToLong
    {
        public static long Convert(decimal amount)
        {
            if (amount < 0.50m)
                throw new ArgumentException("Minimal value for Stripe is 0.50");

            long cents = (long)(amount * 100m);

            return cents > 99999999 ? throw new ArgumentException("Maximum value for Stripe is 999,999.99") : cents;
        }
        
    }
    
}

   