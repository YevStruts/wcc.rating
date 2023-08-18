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
            if (position > 40)
                return 32.0;

            double positionFactor = position / 40.0 * 100.0;

            return /* koef */ 20.0 / 100.0 * positionFactor + 10.0;
        }
    }
}
