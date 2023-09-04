public record AlgorithmResult
{
    public float originalNumber { get; init; }

    public string? originalNumberMantissaAsString { get; init; }

    public string? originalNumberExponentAsString { get; init; }

    public string? originalNumberSignAsString { get; init;  }
    public uint? uintOfOriginalNumber { get; init; }

    public float? M { get; init; }

    public string? patternAsString { get; init; }

    public List<string>? stringPatterns { get; init; }

    public List<Result>? results { get; init; }
}

public struct Result
{
    public float newNumber { get; set; }
    public string newMantissa { get; set; }
    public float delta { get; set; }
}