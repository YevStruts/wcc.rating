using System;
using wcc.rating.kernel.Helpers;
using Xunit;

namespace wcc.rating.kernel.test.Helpers
{
    public class EloHelperTests
    {
        [Theory]
        [InlineData(1100, 1000,  1.0, 40.0, 40.0, 1114.4,  985.6)]
        [InlineData(1000, 1100,  0.0, 40.0, 40.0,  985.6, 1114.4)]
        [InlineData(1100, 1000, 0.75, 40.0, 40.0, 1104.4,  995.6)]
        [InlineData(1000, 1100, 0.25, 40.0, 40.0,  995.6, 1104.4)]
        [InlineData(1100, 1000,  0.5, 40.0, 40.0, 1094.4, 1005.6)]
        public void Count_ReturnsCorrectRatings(double hplayer, double vplayer, double score, double hFactor, double vFactor, double expectedNewRatingPlayer, double expectedNewRatingOpponent)
        {
            // Arrange

            // Act
            var result = EloHelper.Count(hplayer, vplayer, score, hFactor, vFactor);

            // Assert
            Assert.Equal(expectedNewRatingPlayer, result.Item1, 1); // You can adjust the precision based on your requirement
            Assert.Equal(expectedNewRatingOpponent, result.Item2, 1);
        }

        [Theory]
        [InlineData(5, 10.0)]
        [InlineData(15, 20.0)]
        [InlineData(25, 30.0)]
        [InlineData(35, 40.0)]
        public void GetKFactor_ReturnsCorrectValue(int position, double expectedKFactor)
        {
            // Arrange

            // Act
            var result = EloHelper.GetKFactor(position);

            // Assert
            Assert.Equal(expectedKFactor, result);
        }
    }
}