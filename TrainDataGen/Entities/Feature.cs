namespace TrainDataGen.Entities;

public class Feature
{
    public string Name { get; set; }
    public string Classe { get; set; }
    public string? Subclass { get; set; }
    public int Level { get; set; }
    public List<object> Prerequisites { get; set; }
    public List<Choice> FeatureSpecificChoices { get; set; }
    public string FeatureSpecificType { get; set; }

    public Feature(Dictionary<string, object> feature)
    {
        Name = feature["index"].ToString();
        Classe = ((Dictionary<string, object>)feature["class"])["index"].ToString();
        Subclass = feature.ContainsKey("subclass") ? ((Dictionary<string, object>)feature["subclass"])["index"].ToString() : null;
        Level = Convert.ToInt32(feature["level"]);
        Prerequisites = feature.ContainsKey("prerequisites") ? (List<object>)feature["prerequisites"] : new List<object>();
        (FeatureSpecificChoices, FeatureSpecificType) = GetFeatureSpecific(feature);
    }

    private (List<Choice>, string) GetFeatureSpecific(Dictionary<string, object> feature)
    {
        var featureSpecificType = "";
        var featureSpecificChoices = new List<Choice>();

        if (feature.ContainsKey("feature_specific"))
        {
            var fs = (Dictionary<string, object>)feature["feature_specific"];
            if (fs.ContainsKey("expertise_options"))
            {
                featureSpecificType = "expertise";
                var expertiseOptions = (List<object>)((Dictionary<string, object>)fs["expertise_options"])["from"]["options"];
                if (expertiseOptions.Count > 1 &&
                    ((Dictionary<string, object>)expertiseOptions[1]).ContainsKey("items") &&
                    ((Dictionary<string, object>)expertiseOptions[0]).ContainsKey("choice"))
                {
                    var choice0 = (Dictionary<string, object>)((Dictionary<string, object>)expertiseOptions[0])["choice"];
                    var options0 = (List<object>)choice0["from"]["options"];
                    featureSpecificChoices.Add(new Choice(Convert.ToInt32(choice0["choose"]), options0.ConvertAll(item => ((Dictionary<string, object>)((Dictionary<string, object>)item)["item"])["index"].ToString())));

                    var items1 = (List<object>)((Dictionary<string, object>)expertiseOptions[1])["items"];
                    var choice1 = (Dictionary<string, object>)items1[0]["choice"];
                    var options1 = (List<object>)choice1["from"]["options"];
                    featureSpecificChoices.Add(new Choice(Convert.ToInt32(choice1["choose"]), options1.ConvertAll(item => ((Dictionary<string, object>)((Dictionary<string, object>)item)["item"])["index"].ToString())));
                }
                else
                {
                    var expertiseOptionsList = (List<object>)((Dictionary<string, object>)fs["expertise_options"])["from"]["options"];
                    featureSpecificChoices.Add(new Choice(Convert.ToInt32(((Dictionary<string, object>)fs["expertise_options"])["choose"]), expertiseOptionsList.ConvertAll(item => ((Dictionary<string, object>)((Dictionary<string, object>)item)["item"])["index"].ToString())));
                }
            }
            else if (fs.ContainsKey("subfeature_options"))
            {
                featureSpecificType = "subfeature_options";
                var subfeatureOptions = (Dictionary<string, object>)fs["subfeature_options"];
                var options = (List<object>)subfeatureOptions["from"]["options"];
                featureSpecificChoices.Add(new Choice(Convert.ToInt32(subfeatureOptions["choose"]), options.ConvertAll(item => ((Dictionary<string, object>)((Dictionary<string, object>)item)["item"])["index"].ToString())));
            }
            else if (fs.ContainsKey("enemy_type_options"))
            {
                featureSpecificType = "enemy_type_options";
                var enemyTypeOptions = (Dictionary<string, object>)fs["enemy_type_options"];
                var options = (List<object>)enemyTypeOptions["from"]["options"];
                featureSpecificChoices.Add(new Choice(Convert.ToInt32(enemyTypeOptions["choose"]), options.ConvertAll(item => item.ToString())));
            }
            else if (fs.ContainsKey("terrain_type_options"))
            {
                featureSpecificType = "terrain_type_options";
                var terrainTypeOptions = (Dictionary<string, object>)fs["terrain_type_options"];
                var options = (List<object>)terrainTypeOptions["from"]["options"];
                featureSpecificChoices.Add(new Choice(Convert.ToInt32(terrainTypeOptions["choose"]), options.ConvertAll(item => item.ToString())));
            }
            else if (fs.ContainsKey("invocations"))
            {
                featureSpecificType = "invocations";
                var invocations = (List<object>)fs["invocations"];
                featureSpecificChoices.Add(new Choice(GetInvocationsNum(Level), invocations.ConvertAll(item => ((Dictionary<string, object>)item)["index"].ToString())));
            }
        }

        return (featureSpecificChoices, featureSpecificType);
    }

    private int GetInvocationsNum(int level)
    {
        if (level > 17) return 8;
        if (level > 14) return 7;
        if (level > 11) return 6;
        if (level > 8) return 5;
        if (level > 6) return 4;
        if (level > 3) return 3;
        if (level > 1) return 2;
        return 0;
    }
}