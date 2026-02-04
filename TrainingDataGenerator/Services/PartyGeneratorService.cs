using Microsoft.Extensions.DependencyInjection;
using TrainingDataGenerator.DataBase;
using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class PartyGeneratorService : IPartyGenerator
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;
    private readonly IServiceProvider _serviceProvider;

    public PartyGeneratorService(
        ILogger logger,
        IRandomProvider random,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public List<PartyMember> GenerateRandomParty(Database database)
    {
        var partyLevels = new List<byte>();
        var party = new List<PartyMember>();
        var numMembers = _random.Next(1, 8);
        var section = _random.Next(1, 5);

        for (var i = 0; i < numMembers; i++)
            partyLevels.Add((byte)_random.Next((5 * section) - 4, (5 * section) + 1));
        for (var i = 0; i < numMembers; i++)
            party.Add(CreatePartyMember(i, partyLevels[i], _random.SelectRandom(Lists.races), _random.SelectRandom(Lists.classes)));

        _logger.Information($"Generated {numMembers} party members of level {string.Join(", ", partyLevels)}. Levels were in sector {section}");

        return party;
    }

    private PartyMember CreatePartyMember(int id, byte level, RaceMapper race, ClassMapper classMapper)
    {
        // Get services from DI
        var attributeService = _serviceProvider.GetRequiredService<IAttributeService>();
        var equipmentService = _serviceProvider.GetRequiredService<IEquipmentService>();
        var spellService = _serviceProvider.GetRequiredService<ISpellService>();
        var featureService = _serviceProvider.GetRequiredService<IFeatureService>();
        var proficiencyService = _serviceProvider.GetRequiredService<IProficiencyService>();
        var traitService = _serviceProvider.GetRequiredService<ITraitService>();
        var resistanceService = _serviceProvider.GetRequiredService<IResistanceService>();

        return new PartyMember(
            id,
            level,
            race,
            classMapper,
            _logger,
            _random,
            attributeService,
            equipmentService,
            spellService,
            featureService,
            proficiencyService,
            traitService,
            resistanceService);
    }
}