# Snapdragon

For posterity - this is a demonstration project that I wrote while applying for a job at Second Dinner to work on Marvel Snap.

It consists of:

- Logic to process games of Marvel Snap
- Monte Carlo game search to automatically play a side in games of Marvel Snap
- A genetic algorithm to search for optimal decks for Marvel Snap

Because I'm working on this primarily while applying for a job, it's pretty likely that I won't implement every bit of the logic for Marvel Snap (see "Future Work", below). My goals instead are to:

- Show that I can build a fairly complex project that implements the core game, and reasonably suggests I could implement the rest given enough time
- Demonstrate that it's possible to write a system that self-discovers good Marvel Snap decks

Toward the latter point, it's worth noting that [Monte Carlo game search](https://en.wikipedia.org/wiki/Monte_Carlo_tree_search#Pure_Monte_Carlo_game_search) produces quality play regardless of the rules of the game - assuming it knows all of the possible moves, and has a valid implementation of the game logic, it will play the game well - all it needs to be able to do is examine the hypothetical future game states, play them randomly to the end, and know whether or not it wins in each case. Since all of these pieces are in place, there's no reason it wouldn't be able to do the same thing if I was able to swap out my partial implementation for the actual, canonical Marvel Snap engine.

Similarly, the genetic algorithm is implemented broadly without respect to Marvel Snap specifically - the actual "gene sequences" are Marvel Snap cards, but the logic of the algorithm is independent of that - it just will tend to increase the prevelance of a gene in the gene pool if that card is associated with winning the game more than the alternatives.

In practice, if I had the real Marvel Snap engine to work with, I think this would be a useful tool for examining balance questions of actual or hypothetical Marvel Snap cards, but in its current form it doesn't intend to be, because I have only implemented a small subset of the cards and there are portions of the game logic that don't match reality (again, see "Future Work", below).

# Solution Overview

Most of the logic is contained within the main `Snapdragon` project, which contains all primitive types, the genetic-algorithm and Monte Carlo logic, and implementations of the actual Marvel Snap cards. The other projects are `Snapdragon.Tests` which unsurprisingly houses all of the automated tests, and `Snapdragon.Runner`, which is just an executable for running stuff (typically, long evolutions of populations of decks).

Some notes about the items within the main "Snapdragon" project:
- Virtually everything is implemented as immutable record types with various transformations. This is important for the typical reason of avoiding unexpected mutations and allowing me to run everything safely on multiple threads, but also because the Monte Carlo game search logic requires me to easily be able to make derivitive game states and run them forward.
- `SnapCards` contains all of the implemented Marvel Snap cards. It's also a reasonable place to look to get a sense for what ability logic has been written.
- The `Snapdragon.Effects` namespace has types derive from `IEffect`, which have enough information (on instantiation) to transform an instance of `Game`. Most (but not all) other things that transform `Game` do so by creating an `IEffect` and then applying them.
- Some cards (e.g., Hawkeye, Jessica Jones) have on-reveal abilities that create some kind of trigger associated with the space they were played in (regardless of whether the card is then moved). I have chosen to refer to these as "Sensors", most of the logic for which is in the `Snapdragon.Sensors` namespace.
- Outside the context of the game, I use the `CardDefinition` type to refer to a card in the abstract (cost, abilities, etc.). Once a card is used in a game, and then has a Side (i.e., which Player owns it), it gets transformed into the `Card` type.
- Pretty much all of the Monte Carlo game search logic is implemented in either the `MonteCarloSearchController` or in the `ControllerUtilities` class. `ControllerUtilities` mostly consists of logic to get exhaustive sets of all of the possible actions a given player can take on a given turn.

# Future Work

## Things I Just Haven't Done Yet, And Maybe Won't

As noted above, this project isn't attempting to implement everything, so there's some things that aren't done.

### Locations

I'm only partway through implementing `Location`s, with many categories of `Location` ability not addressed yet.

### Hidden Information

The Monte Carlo game search is actually given the full state of the game, including the order of remaining cards in each player's library and hand, and all of the Location information. This probably changes the balance of the game somewhat, although my intuition is that it's probably not SUPER important because both players have this advantage. However, any strategy that pays off heavily by virtue of surprise will probably not be selected for.

This is definitely fixable with effort, but I would need to make a transform to `Game` that basically replaces all "unknown" things (opponent's hand, both player's decks, `Location`s) with random things, and then continue doing so each simulated turn for the Monte Carlo game search. (Also, I would have to track any effects that modify a player's library in some way, such as shuffling in a "Rock" card.)

### Various Card Abilities

At time of writing I have implemented 47 cards, out of what I believe are a few hundred. I've tried to cover a variety of effects - on-reveal, ongoing, movement, etc. - to make it obvious I can do so.

There are a few categories of abilities I still haven't tackled, such as restrictions on when a card can be played (e.g. Ebony Maw, Infinaut).

### Genetics

The current implementation of the genetic algorithm has some advanced features, but the basic version of it just treats a list of cards as a gene sequence. Generally when I "cross" two members of a population I'm randomly re-ordering them first, so the crossing is sort-of-order-agnostic. This is not necessarily optimal. It might be better to order them by cost, for example, so that cards of similar cost would be trading off against each other more commonly.

Also, the only mutation type currently is "instead of either of these cards, pull a random card from the whole possible set". It would probably be good to experiment with some other types of mutations (particularly if card order ends up being important, where swapping the orders / inserting / deleting would be distinct from just picking a random card).

I suspect there might also be some value in choosing to cross decks in non-random ways, like by similarity, so they could "automatically" form populations around particular strategies.

### Database and Viewer For Populations

There's basic database code that can log everything for an expereiment. One obvious thing it's missing is that I can't programmatically delete old experiments (which is somewhat important because they're pretty big, with full game logs).

There's also a basic UI where I can look through what happened in an experiment. It's very much the bare minimum, though - other than being able to click through to see decks and games, there's one graph of card distributions over generations, and everything is very clunky and ugly.


## Technical Debt / Weird Implementation Details

### Effects vs. Abilities

For the most part I've tried to make sure card abilities produce implementations of `IEffect` which then transform the `Game` object. However, I don't think this is the case everywhere - probably some of the abilities I implemented first just directly transform `Game`.

Also, there are certain conditions around what's allowed that get checked on different levels - I've tried to move in the direction of making `IEffect` implementations 
check whether an effect can be applied and silently returning an un-transformed `Game` if not, but in general I *also* throw errors if an `IPlayerController` tries to specify some `IPlayerAction`s that can't be performed.

Maybe this is a reasonable decision. In some cases I rely on it at the `IEffect` level - for example, Heimdall's ability just tries to move everything to the left and I rely on `Effects.MoveCardLeft` to understand when no more cards fit into a particular `Location`. However, that same check appears in several places, which suggests to me that it could be improved.

In general I think also there's some name confusion around similar things that exist at multiple levels, such as reveal abilities and the `IEffect`s that they produce (for example there's a bunch of types that start with `AddPower` in the `Snapdragon.Effects`, `Snapdragon.RevealAbilities`, and `Snapdragon.TriggeredEffects` namespaces).

### Abilities That Get Called With "Source" vs. Sensor Abilities

A lot of the Ongoing/Reveal abilities have logic that gets called with the state of the game and a reference to the card that has the ability on it:  `IRevealAbility<T>.Activate(Game game, T source)` or `OngoingAdjustPower<T>.Apply(Card target, T source, Game game)`. This makes some sense, because a lot of abilities apply or don't apply to other cards based on their relationship to the card with that ability (e.g., cards on the same side, cards in the same `Location`, etc.).

On the flip side, the `Sensor` implementations generally invoke `Builder` types at the time the `Sensor` is created (typically, when some card like `Hawkeye` or 
`Jessica Jones` is played), and these `Builder`s take whatever information they need and store it in the `Sensor` or its members at that point, and then are later 
invoked *without* passing in the `source` object (which would theoretically be the `Sensor` but could arguably instead be the original `Card`). In some ways this could be considered a better pattern - it brings them more in line with how the `IEffect` type works - but there are already a lot more things implemented that pass in a `source` argument, and I could probably reduce the number of interfaces overall by being consistent with that.

### Card.Column

I fixed the previous problem that was here, where the nullable `Card.Column` property was getting checked a lot and I had to constantly look for nulls - now in-play cards are of type `Card` and not-in-play cards are of type `CardInstance` and I always know that `Card.Column` is non-null.

However, this introduced a lot of places where I needed to specify types where I didn't before, so I think there's more cleanup to be done there.

Also, I still have the issue that a `Card.Column` value could mismatch with the `Location.Column` value of its containing `Location`, and it would be nice to address that.

### ControllerUtilities

The `ControllerUtilities` class has some methods for enumerating all of the possible things a player could do, and it broadly works, but I haven't convinced myself that the logic for enumerating all possible moves is provably correct. I did double-check a complex case by hand, though.

### Genetics

The genetics code works, but isn't very well-organized. I think much of the logic in the abstract base "Genetics" type just needs to be moved to a static class, and there's some general cleanup to make everything more comprehensible.

At the moment I think the "Population" and "Genetics" classes have similar roles, and should probably be considered for merging, although I'm not 100% sure.

I recently merged all of the different "IGeneSequence" types into a single class, now called GeneSequence, but there's still some cleanup to be done from that.

### Abilities That Modify Power

There's some logic (particularly Luke Cage's ability) that means I need to keep track of what power modifications have been made to each card, I think as a sequence. (At the moment I just keep the end result.)

### Event Order Of Operations 

At the moment when I resolve an event, if it creates other events, they get resolved later. That doesn't match the actual game, where at least events caused by an on-reveal ability trigger other abilities before the on-reveal ability finishes.

A good example of this is if Carnage eats Nova, which immediately gives its power increase to all cards, even the ones Carnage is about to eat - this will cause a different outcome if e.g. Carnage also is about to eat Bucky Barnes, who will get the boost; in my logic the resulting Winter Soldier would incorrectly get it instead.

### Fluent API for Abilities

I recently rewrote the logic to construct most of the abilities (particularly Ongoing and On-Reveal ones) as a fluent API (at the moment, still nested under a `Snapdragon.Fluent` namespace). I think in general it's an improvement on the prior implementation for abilities, particularly in terms of how easy it is to follow things in `SnapCards.cs` but also in terms of how flexible and extensible the abilities are, but there's definitely some cleanup and refactoring that would be good:

- A lot of the old ability logic is still in place and needs to be deleted
- Once this is the only way to make abilities, there's no point in keeping it in a `Fluent` namespace (which was more so I could keep track of the in-progress work)
- Some of the builder types are hard to follow and not well-documented - in practice they end up *mostly* being intuitive, but there's no justification for not making them cleaner and documenting them
- The types *mostly* work as desired when chaining together stuff, but occasionally I need to explicitly specify one
- I'm not always consistent with where I define the extension methods that get invoked in `SnapCards.cs`
- There are a lot of places where I define things that take a context object, and copies that take the context object *and* an event, and I should try to do away with this
- There are two orders in which some things are created, where either I invoke an extension method on a builder or I call `.Build(...)` on something invoking an extension method on a selector. For example, from `SnapCards.cs`:

```
new(
    "Agent 13",
    1,
    2,
    OnReveal.AddToHand(new RandomSingleItem<CardDefinition, Card>(), My.Self)
),

// ...

new(
    "Spectrum",
    6,
    7,
    OnReveal.Build(My.OtherCards.WithOngoingAbilities().ModifyPower(2))
),
```