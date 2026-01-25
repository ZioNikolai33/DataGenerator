using MongoDB.Bson.Serialization.Attributes;
using System;
using TrainingDataGenerator.Utilities;
using static TrainingDataGenerator.Entities.Mappers.FeatureMapper;

namespace TrainingDataGenerator.Entities.Mappers;

[BsonIgnoreExtraElements]
public class StartingEquipmentOptionMapper
{
    [BsonElement("desc")]
    public string Desc { get; set; } = string.Empty;
    [BsonElement("choose")]
    public byte Choose { get; set; }
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;
    [BsonElement("from")]
    public EquipmentFrom From { get; set; } = new EquipmentFrom();

    [BsonIgnoreExtraElements]
    public class EquipmentFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; } = string.Empty;
        [BsonElement("options")]
        public List<EquipmentOption> Options { get; set; } = new List<EquipmentOption>();
        [BsonElement("equipment_category")]
        public BaseEntity EquipmentCategory { get; set; } = new BaseEntity(); // For option_set_type == "equipment_category"
    }

    [BsonIgnoreExtraElements]
    public class EquipmentOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; } = string.Empty;
        [BsonElement("items")]
        public List<EquipmentOption> Items { get; set; } = new List<EquipmentOption>(); // For "multiple"
        [BsonElement("count")]
        public byte? Count { get; set; } // For "counted_reference"
        [BsonElement("of")]
        public BaseEntity Of { get; set; } = new BaseEntity(); // For "counted_reference"
        [BsonElement("choice")]
        public EquipmentChoice Choice { get; set; } = new EquipmentChoice(); // For "choice"
        [BsonElement("prerequisites")]
        public List<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>(); // For prerequisites
    }

    [BsonIgnoreExtraElements]
    public class EquipmentChoice
    {
        [BsonElement("desc")]
        public string Desc { get; set; } = string.Empty;
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("from")]
        public EquipmentFrom From { get; set; } = new EquipmentFrom();
    }

    [BsonIgnoreExtraElements]
    public class Prerequisite
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        [BsonElement("proficiency")]
        public BaseEntity Proficiency { get; set; } = new BaseEntity();
    }

    public List<ClassMapper.Equipment> GetRandomEquipment()
    {
        var random = new Random();
        var selectedEquipments = new List<ClassMapper.Equipment>();

        if (From.OptionSetType == "equipment_category")
            selectedEquipments.AddRange(GetEquipFromList());

        if (From.Options == null)
            return selectedEquipments;

        var options = From.Options;

        if (options.Count > 1)
            options = options.OrderBy(x => random.Next()).Take(Choose).ToList();

        foreach (var optionsChosen in options)
        {
            if (optionsChosen.OptionType == "multiple" && optionsChosen.Items != null)
            {
                foreach (var item in optionsChosen.Items)
                {
                    if (item.OptionType == "choice")
                    {
                        var choose = item.Choice.Choose;

                        if (item.Choice.Type == "equipment")
                        {
                            var equipIndex = item.Choice.From.EquipmentCategory.Index;
                            var equipments = Lists.GetEquipmentsList(equipIndex);
                            selectedEquipments.AddRange(equipments.OrderBy(x => random.Next()).Take(choose).Select(x => new ClassMapper.Equipment(new BaseEntity(x.Index, x.Name), item.Choice.Choose)).ToList());
                        }
                    }
                    else if (item.OptionType == "counted_reference" && item.Of != null && item.Count.HasValue)
                        selectedEquipments.Add(new ClassMapper.Equipment(new BaseEntity(item.Of.Index, item.Of.Name), (short)item.Count.Value));

                }
            }
            else if (optionsChosen.OptionType == "counted_reference" && optionsChosen.Of != null && optionsChosen.Count.HasValue)
                for (var i = 0; i < optionsChosen.Count.Value; i++)
                    selectedEquipments.Add(new ClassMapper.Equipment(new BaseEntity(optionsChosen.Of.Index, optionsChosen.Of.Name), (short)optionsChosen.Count));
            else if (optionsChosen.OptionType == "choice" && optionsChosen.Choice != null)
            {
                var choose = optionsChosen.Choice.Choose;

                if (optionsChosen.Choice.Type == "equipment")
                {
                    var equipIndex = optionsChosen.Choice.From.EquipmentCategory.Index;
                    var equipments = Lists.GetEquipmentsList(equipIndex);
                    selectedEquipments.AddRange(equipments.OrderBy(x => random.Next()).Take(choose).Select(x => new ClassMapper.Equipment(new BaseEntity(x.Index, x.Name), optionsChosen.Choice.Choose)).ToList());
                }
            }
        }

        return selectedEquipments;
    }

    private List<ClassMapper.Equipment> GetEquipFromList()
    {
        var random = new Random();
        var equipments = Lists.GetEquipmentsList(From.EquipmentCategory.Index);
        var selectedEquipments = new List<ClassMapper.Equipment>();

        if (equipments.Count == 0)
            selectedEquipments.Add(new ClassMapper.Equipment(new BaseEntity(From.EquipmentCategory.Index, From.EquipmentCategory.Name), 1));
        else
            selectedEquipments.AddRange(equipments.OrderBy(x => random.Next())
                .Take(Choose)
                .Select(x => new ClassMapper.Equipment(new BaseEntity(x.Index, x.Name), 1)));
        
        return equipments.Select(x => new ClassMapper.Equipment(new BaseEntity(x.Index, x.Name), 1)).ToList();
    }
}