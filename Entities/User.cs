public record User (
    Guid Id,
    string Name,
    string Email,
    string Phone,
    Guid? ItemId
);