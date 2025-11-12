using System.Diagnostics;
using TrainDataGen.Entities.Mappers;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities;

public class TraitSpecific
{
    public BaseEntity? DamageType { get; set; }
    public BreathWeapon? Breath { get; set; }
    public List<Trait>? Subtraits { get; set; }
    public List<Spell>? Spells { get; set; }

    public class BreathWeapon
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public AreaOfEffect AreaEffect { get; set; }
        public Usage Use { get; set; }
        public DifficultyClass Dc { get; set; }
        public List<Damage> Damage { get; set; }
    }

    public class Damage
    {
        public BaseEntity DamageType { get; set; }
        public Dictionary<byte, string> DamageAtCharacterLevel { get; set; }
    }

    public TraitSpecific(string index, string name, TraitMapper.TraitSpecific traitSpec)
    {
        DamageType = traitSpec.DamageType;
        Breath = traitSpec.BreathWeapon != null ? new BreathWeapon
        {
            Name = traitSpec.BreathWeapon.Name,
            Desc = traitSpec.BreathWeapon.Desc,
            AreaEffect = traitSpec.BreathWeapon.AreaEffect,
            Use = traitSpec.BreathWeapon.Use,
            Dc = traitSpec.BreathWeapon.Dc,
            Damage = traitSpec.BreathWeapon.Damage.Select(d => new Damage
            {
                DamageType = d.DamageType,
                DamageAtCharacterLevel = d.DamageAtCharacterLevel
            }).ToList()
        } : null;
        Subtraits = new List<Trait>();

        foreach (var item in traitSpec.SubtraitOptions.GetRandomChoice())
            Subtraits.Add(new Trait(item.Index, item.Name, EntitiesFinder.GetEntityByIndex(Lists.traits, new BaseEntity(index, name), item)));

        foreach (var item in traitSpec.SpellOptions.GetRandomChoice())
            Spells.Add(new Spell(EntitiesFinder.GetEntityByIndex(Lists.spells, new BaseEntity(index, name), item)));
    }
}
