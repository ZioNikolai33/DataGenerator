using TrainingDataGenerator.Entities;
using TrainingDataGenerator.Entities.Equip;
using TrainingDataGenerator.Entities.Mappers;
using TrainingDataGenerator.Interfaces;
using TrainingDataGenerator.Utilities;

namespace TrainingDataGenerator.Services;

public class EquipmentService : IEquipmentService
{
    private readonly ILogger _logger;
    private readonly IRandomProvider _random;

    public EquipmentService(ILogger logger, IRandomProvider random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void ManageEquipments(PartyMember member, ClassMapper classMapper)
    {
        _logger.Verbose($"Managing equipment for {member.Name}");

        var allEquipmentsBase = classMapper.StartingEquipments;
        var randomBaseEquipments = classMapper.StartingEquipmentsOptions
            .SelectMany(item => item.GetRandomEquipment(_random))
            .ToList();
        
        allEquipmentsBase.AddRange(randomBaseEquipments);

        var allEquipmentsMapper = allEquipmentsBase
            .Select(item => EntitiesFinder.GetEntityByIndex(Lists.equipments, item.Equip))
            .Where(item => item != null)
            .ToList();

        var (armors, meleeWeapons, rangedWeapons, ammunitions) = ConvertToTypedEquipment(allEquipmentsMapper);

        member.MeleeWeapons = meleeWeapons;
        member.RangedWeapons = rangedWeapons;
        member.Ammunitions = ammunitions;
        member.Armors = armors;

        // Auto-equip ammunition
        ammunitions.ForEach(item => item.IsEquipped = true);

        EquipRandomWeapons(member);
        ManageArmorRequirements(member);

        _logger.Verbose($"Equipped {member.Name} with {meleeWeapons.Count} melee weapons, " +
                       $"{rangedWeapons.Count} ranged weapons, and {armors.Count} armor pieces");
    }

    public void EquipRandomWeapons(PartyMember member)
    {
        // Equip melee weapon
        if (member.MeleeWeapons.Count > 0)
        {
            var meleeWeapon = _random.SelectRandom(member.MeleeWeapons);
            meleeWeapon.IsEquipped = true;
                        
            if (meleeWeapon.Properties.Contains("light")) // Handle dual-wielding light weapons
            {
                var secondLight = member.MeleeWeapons
                    .FirstOrDefault(w => w.Properties.Contains("light") && !w.IsEquipped);
                
                if (secondLight != null)
                {
                    secondLight.IsEquipped = true;
                    _logger.Verbose($"{member.Name} dual-wielding {meleeWeapon.Name} and {secondLight.Name}");
                }
                else
                {
                    EquipShield(member);
                }
            }            
            else if (meleeWeapon.Properties.Contains("two-handed")) // Handle two-handed weapons and shields
            {
                var shield = member.Armors.FirstOrDefault(a => a.Index == "shield");

                if (shield != null)
                {
                    shield.IsEquipped = false;
                    _logger.Verbose($"{member.Name} cannot use shield with two-handed weapon");
                }
            }
            else
            {
                EquipShield(member);
            }
        }

        // Equip ranged weapon.
        // It is assumed that ranged weapons can be equipped alongside melee weapons and a player can switch freely.
        if (member.RangedWeapons.Count > 0)
        {
            var rangedWeapon = _random.SelectRandom(member.RangedWeapons);
            rangedWeapon.IsEquipped = true;
            _logger.Verbose($"{member.Name} equipped ranged weapon: {rangedWeapon.Name}");
        }
    }

    public void ManageArmorRequirements(PartyMember member)
    {
        var nonShieldArmors = member.Armors.Where(a => a.Index != "shield").ToList();

        if (nonShieldArmors.Count == 0)
            return;

        // Filter armors that meet strength requirements
        var equippableArmors = nonShieldArmors
            .Where(a => a.StrengthMinimum <= (member.Strength?.Value ?? 0))
            .ToList();

        if (equippableArmors.Count > 0)
        {
            var chosenArmor = _random.SelectRandom(equippableArmors);
            chosenArmor.IsEquipped = true;
            
            _logger.Verbose($"{member.Name} equipped armor: {chosenArmor.Name} " +
                          $"(requires STR {chosenArmor.StrengthMinimum})");
        }
        else
        {
            _logger.Warning($"{member.Name} has insufficient strength for available armors");
        }
    }

    public bool IsProficient(PartyMember member, Weapon weapon)
    {
        // Direct proficiency with specific weapon
        if (member.Proficiencies.Contains(weapon.Index))
            return true;

        // Proficiency with weapon category + range (e.g., "simple-melee-weapons")
        var categoryRangeProficiency = $"{weapon.WeaponCategory.ToLower()}-{weapon.WeaponRange.ToLower()}-weapons";
        if (member.Proficiencies.Contains(categoryRangeProficiency))
            return true;

        // Proficiency with weapon category (e.g., "martial-weapons")
        var categoryProficiency = $"{weapon.WeaponCategory.ToLower()}-weapons";
        if (member.Proficiencies.Contains(categoryProficiency))
            return true;

        // Proficiency with weapon range (e.g., "melee-weapons")
        var rangeProficiency = $"{weapon.WeaponRange.ToLower()}-weapons";
        if (member.Proficiencies.Contains(rangeProficiency))
            return true;

        return false;
    }

    public List<BaseEntity> GetEquipmentsByCategory(string equipmentIndex)
    {
        return equipmentIndex switch
        {
            "melee-weapons" => Lists.meleeWeapons,
            "ranged-weapons" => Lists.rangedWeapons,
            "simple-weapons" => Lists.simpleWeapons,
            "simple-melee-weapons" => Lists.simpleMeleeWeapons,
            "simple-ranged-weapons" => Lists.simpleRangedWeapons,
            "martial-weapons" => Lists.martialWeapons,
            "martial-melee-weapons" => Lists.martialMeleeWeapons,
            "martial-ranged-weapons" => Lists.martialRangedWeapons,
            "light-armor" => Lists.lightArmors,
            "medium-armor" => Lists.mediumArmors,
            "heavy-armor" => Lists.heavyArmors,
            "shields" => Lists.shields,
            _ => new List<BaseEntity>()
        };
    }

    public (List<Armor>, List<MeleeWeapon>, List<RangedWeapon>, List<Ammunition>) ConvertToTypedEquipment(List<EquipmentMapper> equipments)
    {
        var armors = equipments
            .Where(e => e.EquipmentCategory.Index == "armor")
            .Select(e => new Armor(e))
            .ToList();

        var meleeWeapons = equipments
            .Where(e => e.EquipmentCategory.Index == "weapon" && e.WeaponRange == "Melee")
            .Select(e => new MeleeWeapon(e))
            .ToList();

        var rangedWeapons = equipments
            .Where(e => e.EquipmentCategory.Index == "weapon" && e.WeaponRange == "Ranged")
            .Select(e => new RangedWeapon(e))
            .ToList();

        var ammunitions = equipments
            .Where(e => e.GearCategory?.Index == "ammunition")
            .Select(e => new Ammunition(e))
            .ToList();

        return (armors, meleeWeapons, rangedWeapons, ammunitions);
    }

    #region Private Helpers Methods

    private void EquipShield(PartyMember member)
    {
        var shield = member.Armors.FirstOrDefault(a => a.Index.Equals("shield"));

        if (shield != null)
        {
            shield.IsEquipped = true;
            _logger.Verbose($"{member.Name} shield equipped");
        }
    }

    #endregion
}