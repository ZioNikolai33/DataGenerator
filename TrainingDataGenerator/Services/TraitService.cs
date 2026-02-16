using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Entities.PartyEntities;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class TraitService : ITraitService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public TraitService(ILogger logger, IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void ManageTraitSpecifics(PartyMember member, RaceMapper raceMapper, SubraceMapper? subraceMapper)
    {
        _logger.Verbose($"Managing trait specifics for {member.Name}");

        if (raceMapper.Traits.Count == 0)
        {
            _logger.Verbose($"No traits found for race {raceMapper.Name}");
            return;
        }

        // Get all traits (race + subrace)
        var raceTraits = GetRaceTraits(raceMapper, subraceMapper);
        
        if (raceTraits.Count == 0)
        {
            _logger.Verbose("No valid traits found after filtering");
            return;
        }

        _logger.Verbose($"Found {raceTraits.Count} base traits");

        // Select subtraits if available
        var subtraits = SelectSubtraits(raceTraits);
        
        if (subtraits.Count > 0)
        {
            _logger.Verbose($"Selected {subtraits.Count} subtraits");
            
            // Add subtraits to the trait list
            var subtraitMappers = subtraits
                .Select(subtrait => EntitiesFinder.GetEntityByIndex(
                    Lists.traits,
                    new BaseEntity(raceMapper.Index, raceMapper.Name),
                    new BaseEntity(subraceMapper?.Index ?? string.Empty, subraceMapper?.Name ?? string.Empty),
                    subtrait))
                .Where(t => t != null)
                .ToList();

            raceTraits.AddRange(subtraitMappers);
        }

        // Set trait indices on the member
        if (raceTraits.Count > 0)
        {
            member.Traits = raceTraits.Select(t => t.Index).ToList();
            _logger.Verbose($"Applied {member.Traits.Count} total traits: {string.Join(", ", member.Traits)}");
        }
    }

    public List<TraitMapper> GetRaceTraits(RaceMapper raceMapper, SubraceMapper? subraceMapper)
    {
        var allTraitEntities = raceMapper.Traits.ToList();

        // Add subrace traits if applicable
        if (subraceMapper != null)
        {
            allTraitEntities.AddRange(subraceMapper.RacialTraits);
            _logger.Verbose($"Added {subraceMapper.RacialTraits.Count} subrace traits");
        }

        // Get trait mappers from the data
        var raceTraits = allTraitEntities
            .Select(traitEntity => EntitiesFinder.GetEntityByIndex(
                Lists.traits,
                new BaseEntity(raceMapper.Index, raceMapper.Name),
                new BaseEntity(subraceMapper?.Index ?? string.Empty, subraceMapper?.Name ?? string.Empty),
                traitEntity))
            .Where(trait => trait != null && trait.Parent == null) // Only parent traits, not subtraits
            .ToList();

        return raceTraits;
    }

    public List<BaseEntity> SelectSubtraits(List<TraitMapper> traits)
    {
        var allSubtraits = new List<BaseEntity>();

        foreach (var trait in traits)
        {
            // Check if trait has subtrait options
            if (trait.TraitSpec?.SubtraitOptions == null)
                continue;

            var selectedSubtraits = trait.TraitSpec.SubtraitOptions.GetRandomChoice(_random);
            
            if (selectedSubtraits.Count > 0)
            {
                allSubtraits.AddRange(selectedSubtraits);
                _logger.Verbose($"Selected {selectedSubtraits.Count} subtraits from {trait.Name}: " +
                              $"{string.Join(", ", selectedSubtraits.Select(s => s.Name))}");
            }
        }

        return allSubtraits;
    }

    public bool HasTrait(PartyMember member, string traitIndex)
    {
        return member.Traits.Contains(traitIndex);
    }
}