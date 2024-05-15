namespace wcc.rating.kernel.Helpers
{
    internal static class ScoreHelper
    {
        internal static Tuple<float, float> GetBO3Score(int score1, int score2)
        {
            if (score1 == 2 && score2 == 0)
                return Tuple.Create(1.0f, 0.0f);

            if (score1 == 2 && score2 == 1)
                return Tuple.Create(0.75f, 0.25f);

            if (score1 == 0 && score2 == 2)
                return Tuple.Create(0.0f, 1.0f);

            if (score1 == 1 && score2 == 2)
                return Tuple.Create(0.25f, 0.75f);

            if (score1 == 1 && score2 == 1)
                return Tuple.Create(0.5f, 0.5f);

            var total = score1 + score2;
            return Tuple.Create(
                score1 == total ? 1.0f : score1 / (float)total,
                score2 == total ? 1.0f : score2 / (float)total
            );
        }
    }
}
