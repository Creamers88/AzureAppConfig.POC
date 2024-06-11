using Microsoft.FeatureManagement;

namespace AzureAppConfig.POC.WebApp.FeatureManagement.Filters
{
    [FilterAlias("DiceRoll")]
    public class DiceRollFilter : IFeatureFilter
    {
        private readonly Random _random = new();

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            int chosenNumber = context.Parameters.GetSection("Number").Get<int>();

            int diceRoll = _random.Next(1, 7);

            return Task.FromResult(chosenNumber == diceRoll);
        }
    }
}
