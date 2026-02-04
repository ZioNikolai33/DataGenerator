using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Enums;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;

namespace TrainingDataGenerator.Services;

public class FeatureService : IFeatureService
{
    private readonly ILogger _logger;

    public FeatureService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ApplyFeatureEffects(PartyMember member)
    {
        _logger.Verbose($"Applying feature effects for {member.Name}");

        var featureIndices = member.Features.Select(f => f.Index).ToList();

        // Fast Movement (Barbarian/Monk)
        if (featureIndices.Contains("fast-movement"))
        {
            member.Speed += 10;
            _logger.Verbose($"Applied Fast Movement: Speed +10 -> {member.Speed}");
        }

        // Monk-specific features
        if (member.Class == "monk")
        {
            ApplyMonkFeatures(member, featureIndices);
        }
    }

    public void ApplyFeatureSpecifics(PartyMember member)
    {
        _logger.Verbose($"Applying feature specifics for {member.Name}");

        var newFeatures = new List<Feature>();

        foreach (var feature in member.Features)
        {
            if (feature.FeatureType == null)
                continue;

            switch (feature.FeatureType)
            {
                case FeatureSpecificTypes.Expertise:
                    ApplyExpertise(member, feature);
                    break;

                case FeatureSpecificTypes.Enemy:
                    member.FeatureSpecifics.AddRange(feature.FeatureSpec ?? new List<string>());
                    _logger.Verbose($"Added favored enemy specifics: {string.Join(", ", feature.FeatureSpec ?? new List<string>())}");
                    break;

                case FeatureSpecificTypes.Terrain:
                    member.FeatureSpecifics.AddRange(feature.FeatureSpec ?? new List<string>());
                    _logger.Verbose($"Added favored terrain specifics: {string.Join(", ", feature.FeatureSpec ?? new List<string>())}");
                    break;

                case FeatureSpecificTypes.Subfeature:
                    AddSubfeatures(member, feature, newFeatures);
                    break;
            }
        }

        if (newFeatures.Count > 0)
        {
            member.Features.AddRange(newFeatures);
            _logger.Verbose($"Added {newFeatures.Count} subfeatures");
        }
    }

    public void CheckFeaturePrerequisites(PartyMember member, List<FeatureMapper> allFeatures)
    {
        _logger.Verbose($"Checking feature prerequisites for {member.Name}");

        var featuresToRemove = new List<string>();

        foreach (var feature in allFeatures)
        {
            if (feature.Prerequisites == null || feature.Prerequisites.Count == 0)
                continue;

            foreach (var prereq in feature.Prerequisites)
            {
                bool meetsPrereq = false;

                if (prereq.Type == "feature")
                {
                    var requiredFeature = prereq.Feature?.Split('/').Last();
                    if (member.Features.Any(f => f.Index == requiredFeature))
                        meetsPrereq = true;
                }
                else if (prereq.Type == "spell")
                {
                    var requiredSpell = prereq.Feature?.Split('/').Last();
                    if (member.Spells.Any(s => s.Index == requiredSpell) || 
                        member.Cantrips.Any(c => c.Index == requiredSpell))
                        meetsPrereq = true;
                }

                if (!meetsPrereq)
                {
                    featuresToRemove.Add(feature.Index);
                    _logger.Warning($"Feature '{feature.Index}' removed - prerequisite not met");
                }
            }
        }

        member.Features = member.Features.Where(f => !featuresToRemove.Contains(f.Index)).ToList();
    }

    #region Private Helper Methods

    private void ApplyMonkFeatures(PartyMember member, List<string> featureIndices)
    {
        // Unarmored Movement
        if (featureIndices.Contains("unarmored-movement-1") && 
            member.Armors.All(a => !a.IsEquipped))
        {
            member.Speed += 10;
            _logger.Verbose($"Applied Unarmored Movement: Speed +10 -> {member.Speed}");
        }

        // Diamond Soul
        if (featureIndices.Contains("diamond-soul"))
        {
            member.Constitution.Save += member.ProficiencyBonus;
            member.Intelligence.Save += member.ProficiencyBonus;
            member.Wisdom.Save += member.ProficiencyBonus;
            member.Charisma.Save += member.ProficiencyBonus;
            
            _logger.Verbose($"Applied Diamond Soul: All mental saves +{member.ProficiencyBonus}");
        }
    }

    private void ApplyExpertise(PartyMember member, Feature feature)
    {
        foreach (var choice in feature.FeatureSpec ?? new List<string>())
        {
            var skill = member.Skills.FirstOrDefault(s => s.Index == choice);
            if (skill != null)
            {
                skill.SetExpertise(true, member.ProficiencyBonus);
                _logger.Verbose($"Applied Expertise to {skill.Name}");
            }
        }
    }

    private void AddSubfeatures(PartyMember member, Feature feature, List<Feature> newFeatures)
    {
        foreach (var choice in feature.FeatureSpec ?? new List<string>())
        {
            // Note: This requires access to game data - will be addressed in refactored version
            _logger.Verbose($"Adding subfeature: {choice}");
        }
    }

    #endregion
}