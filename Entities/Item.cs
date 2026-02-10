public record Item (
    Guid Id,
    float Longitude,
    float Latitude,
    string City,
    string Street,
    string Number,
    string? Photo,
    string? Title,
    string? Description,
    int? UserId
);