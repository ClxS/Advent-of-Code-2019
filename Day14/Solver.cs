namespace Day14
{
    using System;

    internal class Solver : ISolver
    {
        public long Solve(Data inputData)
        {
            var refinery = Refinery.Create(inputData.ReactionRecipes);

            var oreRequiredForOne = refinery.GetOreRequirement(Product.Get("FUEL")).Count;
            if (inputData.OreBudget <= 0)
            {
                return oreRequiredForOne;
            }

            var minGuess = (long)(inputData.OreBudget / (decimal)oreRequiredForOne);
            var hitMin = false;

            // Increase by an assumed difference until we get just over, then move backwards until we're in range.
            while (true)
            {
                var count = refinery.GetOreRequirement(Product.Get("FUEL"), minGuess).Count;
                if (count > inputData.OreBudget)
                {
                    minGuess--;
                    hitMin = true;
                }
                else if (count < inputData.OreBudget)
                {
                    if (hitMin)
                    {
                        break;
                    }

                    minGuess += (long)Math.Ceiling((inputData.OreBudget - count) / (decimal)oreRequiredForOne);
                }
                else if (count == inputData.OreBudget)
                {
                    break;
                }
            }

            return minGuess;
        }
    }
}
