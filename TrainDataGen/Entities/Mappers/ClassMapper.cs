using MongoDB.Bson.Serialization.Attributes;

namespace TrainDataGen.Entities.Mappers;

public class ClassMapper : BaseMapper
{
    [BsonElement("hit_die")]
    public short Hp { get; set; }
    [BsonElement("subclasses")]
    public List<BaseMapper> Subclasses { get; set; }
    [BsonElement("proficiency_choices")]
    public List<ProficiencyChoiceMapper> ProficiencyChoices { get; set; }
    [BsonElement("saving_throws")]
    public List<BaseMapper> SavingThrows { get; set; }
    [BsonElement("proficiencies")]
    public List<BaseMapper> Proficiencies { get; set; }
    [BsonElement("starting_equipment")]
    public List<EquipmentMapper> StartingEquipments { get; set; }
    [BsonElement("starting_equipment_options")]
    public List<StartingEquipmentOptionMapper> StartingEquipmentsOptions { get; set; }
    [BsonElement("multi_classing")]
    public Multiclass Multiclassing { get; set; }
    [BsonElement("spellcasting")]
    public SpellcastingMapper? SpellcastingAbility { get; set; }

    public ClassMapper(string index, string name) : base(index, name) {  }
}
