namespace Snapdragon.GeneticAlgorithm
{
    /// <summary>
    /// A crossable "gene sequence" that just consists of some cards.
    /// </summary>
    /// <param name="Cards">The cards in this "sequence".</param>
    /// <param name="AllPossibleCards">All possible cards. Used for mutations when crossing.</param>
    /// <param name="MutationPer">Mutation rate (given as the denominator of a fraction, 1 / [this value]).</param>
    /// <param name="OrderBy">If specified, genes are ordered this way before crossing.</param>
    public record CardGeneSequence(
        IReadOnlyList<CardDefinition> Cards,
        IReadOnlyList<CardDefinition> AllPossibleCards,
        int MutationPer = 100,
        Func<CardDefinition, int>? OrderBy = null
    ) : IGeneSequence<CardGeneSequence>
    {
        public CardGeneSequence Cross(CardGeneSequence other)
        {
            if (other.Cards.Count != this.Cards.Count)
            {
                throw new InvalidOperationException(
                    "Cannot cross two CardGeneSequences of different lengths."
                );
            }
            var allPresentCards = this.Cards.Concat(other.Cards).ToList();
            var allPresentCardNames = allPresentCards.Select(c => c.Name).Distinct().ToList();

            // Don't allow duplicates (by name)
            var usedCards = new HashSet<string>();

            var first = this.Cards;
            var second = other.Cards;

            var newDeckCards = new List<CardDefinition>();

            if (OrderBy != null)
            {
                first = first.OrderBy(OrderBy).ToList();
                second = second.OrderBy(OrderBy).ToList();
            }

            for (var i = 0; i < first.Count; i++)
            {
                var f = first[i];
                var s = second[i];

                if (Random.Next(MutationPer) == 0)
                {
                    // "Mutate" - get a random CardDefinition from all cards, instead of the normal logic
                    var mutantGene = Random.Of(
                        AllPossibleCards.Where(c => !usedCards.Contains(c.Name)).ToList()
                    );
                    newDeckCards.Add(mutantGene);
                    usedCards.Add(mutantGene.Name);
                    continue;
                }

                // The logic when one or more cards is already present
                // probably has some implications for the population.
                // Should look into this later.
                if (!usedCards.Contains(f.Name) && !usedCards.Contains(s.Name))
                {
                    var choice = Random.NextBool() ? f : s;

                    usedCards.Add(choice.Name);
                    newDeckCards.Add(choice);
                }
                else if (!usedCards.Contains(f.Name))
                {
                    usedCards.Add(f.Name);
                    newDeckCards.Add(f);
                }
                else if (!usedCards.Contains(s.Name))
                {
                    usedCards.Add(s.Name);
                    newDeckCards.Add(s);
                }
            }

            if (newDeckCards.Count < this.Cards.Count)
            {
                var randomCards = allPresentCardNames
                    .Where(n => !usedCards.Contains(n))
                    .ToList()
                    .OrderBy(n => Random.Next())
                    .Take(this.Cards.Count - newDeckCards.Count)
                    .Select(n => allPresentCards.First(c => string.Equals(c.Name, n)));

                newDeckCards.AddRange(randomCards);
            }

            return this with
            {
                Cards = newDeckCards
            };
        }
    }
}
