namespace wcc.rating.kernel.Helpers
{
    public static class EloHelper
    {
        public static Tuple<double, double> Count(double hplayer, double vplayer, double score, double hFactor, double vFactor)
        {
            // Calculate expected scores
            double expectedScorePlayer = 1 / (1 + Math.Pow(10, (vplayer - hplayer) / 400));
            double expectedScoreOpponent = 1 / (1 + Math.Pow(10, (hplayer - vplayer) / 400));

            // Calculate new ratings
            double newRatingPlayer = hplayer + hFactor * (score - expectedScorePlayer);
            double newRatingOpponent = vplayer + vFactor * ((1 - score) - expectedScoreOpponent);

            return Tuple.Create(newRatingPlayer, newRatingOpponent);
        }

        public static double GetKFactor(int position)
        {
            if (position < 10)
                return 10.0;
            if (position < 20)
                return 20.0;
            if (position < 30)
                return 30.0;
            return 40.0;
        }
    }
}
