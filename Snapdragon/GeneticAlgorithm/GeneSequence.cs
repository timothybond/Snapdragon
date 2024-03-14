using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record GeneSequence(
        ImmutableList<CardDefinition> FixedCards,
        ImmutableList<CardDefinition> EvolvingCards,
        Genetics? Genetics,
        IPlayerController Controller,
        Guid Id,
        Guid? FirstParentId,
        Guid? SecondParentId
    )
    {
        public GeneSequence Cross(GeneSequence other)
        {
            if (other.EvolvingCards.Count != this.EvolvingCards.Count)
            {
                throw new InvalidOperationException(
                    "Cannot cross two GeneSequences of different lengths."
                );
            }
            var allPresentCards = this.EvolvingCards.Concat(other.EvolvingCards).ToList();
            var allPresentCardNames = allPresentCards.Select(c => c.Name).Distinct().ToList();

            // Don't allow duplicates (by name)
            var usedCards = new HashSet<string>(FixedCards.Select(c => c.Name));

            var first = this.EvolvingCards.ToList();
            var second = other.EvolvingCards.ToList();

            var newEvolvingCards = new List<CardDefinition>();

            if (Genetics.OrderBy != null)
            {
                first = first.OrderBy(Genetics.OrderBy.GetOrder).ToList();
                second = second.OrderBy(Genetics.OrderBy.GetOrder).ToList();
            }

            for (var i = 0; i < first.Count; i++)
            {
                var f = first[i];
                var s = second[i];

                if (Random.Next(Genetics.MutationPer) == 0)
                {
                    // "Mutate" - get a random CardDefinition from all cards, instead of the normal logic
                    var mutantGene = Random.Of(
                        Genetics.AllPossibleCards.Where(c => !usedCards.Contains(c.Name)).ToList()
                    );
                    newEvolvingCards.Add(mutantGene);
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
                    newEvolvingCards.Add(choice);
                }
                else if (!usedCards.Contains(f.Name))
                {
                    usedCards.Add(f.Name);
                    newEvolvingCards.Add(f);
                }
                else if (!usedCards.Contains(s.Name))
                {
                    usedCards.Add(s.Name);
                    newEvolvingCards.Add(s);
                }
            }

            // Due to the restrictions on duplication and the random order, it's possible
            // we can run out of cards to "accept" if we happened to skip some earlier.
            if (newEvolvingCards.Count < this.EvolvingCards.Count)
            {
                var randomCards = allPresentCardNames
                    .Where(n => !usedCards.Contains(n))
                    .ToList()
                    .OrderBy(n => Random.Next())
                    .Take(this.EvolvingCards.Count - newEvolvingCards.Count)
                    .Select(n => allPresentCards.First(c => string.Equals(c.Name, n)));

                newEvolvingCards.AddRange(randomCards);
            }

            // This fallback shouldn't matter in practice, but just in case, we will pull in more random cards.
            if (newEvolvingCards.Count < this.EvolvingCards.Count)
            {
                var randomCardsFromGenePool = Genetics
                    .AllPossibleCards.Where(c => !usedCards.Contains(c.Name))
                    .Take(this.EvolvingCards.Count - newEvolvingCards.Count);
                newEvolvingCards.AddRange(randomCardsFromGenePool);
            }

            return this with
            {
                Id = Guid.NewGuid(),
                FirstParentId = this.Id,
                SecondParentId = other.Id,
                EvolvingCards = newEvolvingCards.ToImmutableList()
            };
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.FixedCards.Concat(this.EvolvingCards).ToList();
        }

        public string? GetControllerString()
        {
            return Controller.ToString();
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            return new PlayerConfiguration(
                Id.ToString(),
                new Deck(FixedCards.AddRange(EvolvingCards), Id),
                Controller
            );
        }
    }
}
