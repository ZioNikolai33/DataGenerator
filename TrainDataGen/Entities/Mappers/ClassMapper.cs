using MongoDB.Bson.Serialization.Attributes;
using TrainDataGen.Entities.Mappers.Equipment;

namespace TrainDataGen.Entities.Mappers;

public class ClassMapper : BaseEntity
{
    [BsonElement("hit_die")]
    public short Hp { get; set; }
    [BsonElement("subclasses")]
    public List<BaseEntity> Subclasses { get; set; }
    [BsonElement("proficiency_choices")]
    public List<ProficiencyChoiceMapper> ProficiencyChoices { get; set; }
    [BsonElement("saving_throws")]
    public List<BaseEntity> SavingThrows { get; set; }
    [BsonElement("proficiencies")]
    public List<BaseEntity> Proficiencies { get; set; }
    [BsonElement("starting_equipment")]
    public List<EquipmentMapper> StartingEquipments { get; set; }
    [BsonElement("starting_equipment_options")]
    public List<StartingEquipmentOptionMapper> StartingEquipmentsOptions { get; set; }
    [BsonElement("multi_classing")]
    public MulticlassMapper Multiclassing { get; set; }
    [BsonElement("spellcasting")]
    public SpellcastingMapper? SpellcastingAbility { get; set; }

    public ClassMapper(string index, string name) : base(index, name) {  }
}
