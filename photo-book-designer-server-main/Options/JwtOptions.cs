namespace photo_book_designer_server_main.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = "TEMPORARY_KEY_UNDER_TEST_VERY_VERY_VERY_VERY_VERY_LONG";
    public string Issuer { get; set; } = "photo-book-designer-server";
    public string Audience { get; set; } = "photo-book-designer-client";
    public int ExpiresMinutes { get; set; } = 60;
}

