namespace SerilogJsonPrefixTest;

using Serilog.Events;

public static class LogEventPropertyValueExtensions
{
    public static string GetTypePrefix(this LogEventPropertyValue propertyValue)
    {
        return propertyValue switch
        {
            StructureValue => "o_",
            ScalarValue sv => sv.Value switch
            {
                string => "s_",
                double => "f_",
                float => "f_",
                byte => "i_",
                short => "i_",
                int => "i_",
                long => "i_",
                DateTime => "dt_",
                DateTimeOffset => "dt_",
                _ => ""
            },
            _ => ""
        };
    }
}