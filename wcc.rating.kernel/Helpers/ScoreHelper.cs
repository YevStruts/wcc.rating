namespace wcc.rating.kernel.Helpers
{
    internal static class ScoreHelper
    {
        internal static Tuple<float, float> GetBOScore(int score1, int score2)
        {
            var total = score1 + score2;
            return Tuple.Create(
                score1 == total ? 1.0f : score1 / (float)total,
                score2 == total ? 1.0f : score2 / (float)total
            );
        }
    }
}
