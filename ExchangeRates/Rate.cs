namespace ExchangeRates
{
    class Rate
    {
        public string Date
        {
            get;
        }
        public double Mid
        {
            get;
        }

        public Rate(string date, double mid)
        {
            Date = date;
            Mid = mid;
        }

        public override string ToString()
        {
            return Date + ", " + Mid;
        }
    }
}