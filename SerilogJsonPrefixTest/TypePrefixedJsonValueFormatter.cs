namespace SerilogJsonPrefixTest;

using Serilog.Events;
using Serilog.Formatting.Json;

public class TypePrefixedJsonValueFormatter : JsonValueFormatter
{
    protected override bool VisitStructureValue(TextWriter state, StructureValue structure)
    {
        state.Write('{');

        var delim = "";

        for (var i = 0; i < structure.Properties.Count; i++)
        {
            state.Write(delim);
            delim = ",";

            var property = structure.Properties[i];
            string name = $"{property.Value.GetTypePrefix()}{property.Name}";

            WriteQuotedJsonString(name, state);
            state.Write(':');
            Visit(state, property.Value);
        }

        if (structure.TypeTag != null)
        {
            state.Write(delim);
            WriteQuotedJsonString("_typeTag", state);
            state.Write(':');
            WriteQuotedJsonString(structure.TypeTag, state);
        }

        state.Write('}');
        return false;
    }
}
