using System.Text.RegularExpressions;

namespace CrusadeTracker.Application.Forces;

public sealed record BattleForgeForce(
    string Name,
    string Faction,
    int PointsLimit,
    string Detachment,
    IReadOnlyList<BattleForgeUnit> Units);

public sealed record BattleForgeUnit(
    string Name,
    string BattlefieldRole,
    int Points,
    IReadOnlyList<string> Equipment);

public static partial class BattleForgeParser
{
    public static BattleForgeForce Parse(string exportText)
    {
        if (string.IsNullOrWhiteSpace(exportText))
            throw new ArgumentException("Export text cannot be empty.", nameof(exportText));

        var lines = exportText
            .Split('\n')
            .Select(l => l.TrimEnd('\r'))
            .ToList();

        // Line 1: Force name with points, e.g. "Plague Wall (980 points)"
        string forceName = ParseNameFromLine(lines[0])
            ?? throw new FormatException("Could not parse force name from first line.");

        // Line 3: Faction
        string faction = lines.Count > 2
            ? lines[2].Trim()
            : throw new FormatException("Could not parse faction.");

        if (string.IsNullOrWhiteSpace(faction))
            throw new FormatException("Could not parse faction.");

        // Line 4: Point limit, e.g. "Incursion (1000 points)"
        int pointsLimit = lines.Count > 3
            ? ParsePointsFromLine(lines[3])
                ?? throw new FormatException("Could not parse points limit from line 4.")
            : throw new FormatException("Export text is too short.");

        // Line 5: Detachment name
        string detachment = lines.Count > 4
            ? lines[4].Trim()
            : "";

        // Parse units grouped by type sections
        var units = ParseUnits(lines);

        return new BattleForgeForce(forceName, faction, pointsLimit, detachment, units);
    }

    private static List<BattleForgeUnit> ParseUnits(List<string> lines)
    {
        var units = new List<BattleForgeUnit>();
        string currentBattlefieldRole = "";

        // Known section headers
        var sectionHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CHARACTERS", "BATTLELINE", "DEDICATED TRANSPORTS",
            "OTHER DATASHEETS", "ALLIED UNITS", "FORTIFICATIONS"
        };

        for (int i = 0; i < lines.Count; i++)
        {
            string trimmed = lines[i].Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue;

            // Check if this is a section header
            if (sectionHeaders.Contains(trimmed))
            {
                currentBattlefieldRole = trimmed;
                continue;
            }

            // Check if this is a unit line: "Unit Name (N points)"
            int? points = ParsePointsFromLine(trimmed);
            string? name = ParseNameFromLine(trimmed);

            if (points.HasValue && name is not null && !string.IsNullOrEmpty(currentBattlefieldRole))
            {
                // Collect equipment lines that follow this unit
                var equipment = new List<string>();
                int j = i + 1;
                while (j < lines.Count)
                {
                    string equipLine = lines[j].Trim();

                    // Stop at blank line, next section header, or next unit line
                    if (string.IsNullOrEmpty(equipLine))
                        break;
                    if (sectionHeaders.Contains(equipLine))
                        break;
                    if (ParsePointsFromLine(equipLine).HasValue)
                        break;

                    // Strip bullet characters and clean up the equipment entry
                    string cleaned = equipLine.TrimStart('•', ' ', '\t');
                    if (!string.IsNullOrWhiteSpace(cleaned))
                        equipment.Add(cleaned);

                    j++;
                }

                units.Add(new BattleForgeUnit(name, currentBattlefieldRole, points.Value, equipment));
                i = j - 1; // advance past equipment lines
            }
        }

        return units;
    }

    private static string? ParseNameFromLine(string line)
    {
        var match = PointsPattern().Match(line);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static int? ParsePointsFromLine(string line)
    {
        var match = PointsPattern().Match(line);
        return match.Success ? int.Parse(match.Groups[2].Value) : null;
    }

    [GeneratedRegex(@"^(.+?)\s*\((\d+)\s+points?\)", RegexOptions.IgnoreCase)]
    private static partial Regex PointsPattern();
}
