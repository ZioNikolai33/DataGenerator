using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using TrainDataGen.Utilities;

namespace TrainDataGen.Entities.Mappers;

public class StartingEquipmentOptionMapper
{
    [BsonElement("desc")]
    public string Desc { get; set; }
    [BsonElement("choose")]
    public byte Choose { get; set; }
    [BsonElement("type")]
    public string Type { get; set; }
    [BsonElement("from")]
    public EquipmentFrom From { get; set; }

    public class EquipmentFrom
    {
        [BsonElement("option_set_type")]
        public string OptionSetType { get; set; }
        [BsonElement("options")]
        public List<EquipmentOption> Options { get; set; }
        [BsonElement("equipment_category")]
        public BaseEntity EquipmentCategory { get; set; } // For option_set_type == "equipment_category"
    }

    public class EquipmentOption
    {
        [BsonElement("option_type")]
        public string OptionType { get; set; }
        [BsonElement("item")]
        public List<EquipmentOption> Items { get; set; } // For "multiple"
        [BsonElement("count")]
        public byte? Count { get; set; } // For "counted_reference"
        [BsonElement("of")]
        public BaseEntity Of { get; set; } // For "counted_reference"
        [BsonElement("choice")]
        public EquipmentChoice Choice { get; set; } // For "choice"
        [BsonElement("prerequisites")]
        public List<Prerequisite> Prerequisites { get; set; } // For prerequisites
    }

    public class EquipmentChoice
    {
        [BsonElement("desc")]
        public string Desc { get; set; }
        [BsonElement("choose")]
        public byte Choose { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("from")]
        public EquipmentFrom From { get; set; }
    }

    public class Prerequisite
    {
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("proficiency")]
        public BaseEntity Proficiency { get; set; }
    }

    public List<BaseEntity> GetRandomEquipment()
    {
        var random = new Random();
        var selectedEquipments = new List<BaseEntity>();

        if (From.OptionSetType == "equipment_category")
            selectedEquipments.Add(From.EquipmentCategory);

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
                            selectedEquipments.AddRange(equipments.OrderBy(x => random.Next()).Take(choose).ToList());
                        }
                    }

                }
            }
            else if (optionsChosen.OptionType == "counted_reference" && optionsChosen.Of != null && optionsChosen.Count.HasValue)
            {
                for (var i = 0; i < optionsChosen.Count.Value; i++)
                {
                    selectedEquipments.Add(optionsChosen.Of);
                }
            }
            else if (optionsChosen.OptionType == "choice" && optionsChosen.Choice != null)
            {
                var choose = optionsChosen.Choice.Choose;

                if (optionsChosen.Choice.Type == "equipment")
                {
                    var equipIndex = optionsChosen.Choice.From.EquipmentCategory.Index;
                    var equipments = Lists.GetEquipmentsList(equipIndex);
                    selectedEquipments.AddRange(equipments.OrderBy(x => random.Next()).Take(choose).ToList());
                }
            }
        }

        return selectedEquipments;
    }
}