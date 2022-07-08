namespace SerilogJsonPrefixTest;

using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

/// <summary>
/// An <see cref="ITextFormatter"/> that writes events in a compact JSON format, for consumption in environments 
/// without message template support. Message templates are rendered into text and a hashed event id is included.
/// All properties are prefixed based on their type to help avoid clashes in ELK.
/// </summary>
public class TypePrefixedJsonFormatter : ITextFormatter
{
    readonly JsonValueFormatter _valueFormatter;

    public TypePrefixedJsonFormatter()
    {
        _valueFormatter = new TypePrefixedJsonValueFormatter();
    }

    /// <summary>
    /// Format the log event into the output. Subsequent events will be newline-delimited.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);

        output.Write("{\"@t\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));

        output.Write("\",\"@m\":");
        var message = logEvent.MessageTemplate.Render(logEvent.Properties);
        JsonValueFormatter.WriteQuotedJsonString(message, output);

        output.Write(",\"@i\":\"");
        var id = EventIdHash.Compute(logEvent.MessageTemplate.Text);
        output.Write(id.ToString("x8"));
        output.Write('"');

        if (logEvent.Level != LogEventLevel.Information)
        {
            output.Write(",\"@l\":\"");
            output.Write(logEvent.Level);
            output.Write('\"');
        }

        if (logEvent.Exception != null)
        {
            output.Write(",\"@x\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        foreach (var property in logEvent.Properties)
        {
            var name = $"{property.Value.GetTypePrefix()}{property.Key}";

            if (name.Length > 0 && name[0] == '@')
            {
                // Escape first '@' by doubling
                name = '@' + name;
            }

            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(name, output);
            output.Write(':');

            _valueFormatter.Format(property.Value, output);
        }

        output.Write('}');

        output.WriteLine();
    }
}
