namespace TrainDataGen.Entities;

public class Multiclass
{
    public List<string> PrerequisitiesAttribute { get; set; }
    public List<int> PrerequisitiesValue { get; set; }
    public int NumChoices { get; set; }
    public List<string> Proficiencies { get; set; }
    public List<Choice> ProficiencyChoices { get; set; }

    public Multiclass(Dictionary<string, object> classe)
    {
        PrerequisitiesAttribute = new List<string>();
        PrerequisitiesValue = new List<int>();
        Proficiencies = new List<string>();
        ProficiencyChoices = new List<Choice>();

        if (classe.ContainsKey("prerequisities"))
        {
            var prereqs = (List<object>)classe["prerequisities"];
            foreach (var itemObj in prereqs)
            {
                var item = (Dictionary<string, object>)itemObj;
                PrerequisitiesAttribute.Add(((Dictionary<string, object>)item["ability_score"])["index"].ToString());
                PrerequisitiesValue.Add((int)item["minimum_score"]);
            }
            NumChoices = 0;
        }
        else if (classe.ContainsKey("prerequisite_options"))
        {
            var options = (Dictionary<string, object>)classe["prerequisite_options"];
            var fromOptions = (List<object>)((Dictionary<string, object>)options["from"])["options"];
            foreach (var itemObj in fromOptions)
            {
                var item = (Dictionary<string, object>)itemObj;
                PrerequisitiesAttribute.Add(((Dictionary<string, object>)item["ability_score"])["index"].ToString());
                PrerequisitiesValue.Add((int)item["minimum_score"]);
            }
            NumChoices = (int)options["choose"];
        }

        if (classe.ContainsKey("proficiencies"))
        {
            var profs = (List<object>)classe["proficiencies"];
            foreach (var itemObj in profs)
            {
                var item = (Dictionary<string, object>)itemObj;
                Proficiencies.Add(item["index"].ToString());
            }
        }

        if (classe.ContainsKey("proficiency_choices"))
        {
            var profChoices = (List<object>)classe["proficiency_choices"];
            foreach (var profObj in profChoices)
            {
                var prof = (Dictionary<string, object>)profObj;
                int choose = (int)prof["choose"];
                var options = (List<object>)((Dictionary<string, object>)prof["from"])["options"];
                var indices = new List<string>();
                foreach (var optionObj in options)
                {
                    var option = (Dictionary<string, object>)optionObj;
                    indices.Add(((Dictionary<string, object>)option["item"])["index"].ToString());
                }
                ProficiencyChoices.Add(new Choice(choose, indices));
            }
        }
    }
}