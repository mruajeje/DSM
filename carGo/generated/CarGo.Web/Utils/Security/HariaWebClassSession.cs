// HARIA-WEB GENERATED sha256:e6761c2e2754d912eb227202d083964ecff42fb0828eba81e2af31e90b4f182d
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace CarGo.Web.Utils.Security;

// Snapshots auxiliares y tipados: nunca autentican ni conceden permisos.
// Utilizar ViewModels separados del dominio, sin grafos de EF/NHibernate.
public static class HariaWebClassSession
{
    private const string Prefix = "HariaWeb.Class:";
    public const string UserKey = "HariaWeb.User";
    private const string UserTypeKey = "HariaWeb.UserType";
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(info =>
        {
            foreach (var property in info.Properties.ToArray())
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var scalar = type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)
                    || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset)
                    || type == typeof(DateOnly) || type == typeof(TimeOnly) || type == typeof(TimeSpan) || type == typeof(Uri);
                if (!scalar || new[] { "password", "secret", "token", "credential", "hash" }.Any(name => property.Name.Contains(name, StringComparison.OrdinalIgnoreCase)))
                    info.Properties.Remove(property);
            }
        });
        return new JsonSerializerOptions(JsonSerializerDefaults.Web) { TypeInfoResolver = resolver };
    }

    public static IServiceCollection AddHariaWebClassSession(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(8);
            options.Cookie.Name = ".HariaWeb.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });
        return services;
    }

    public static void SetClass<TViewModel>(this ISession session, TViewModel model) where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(model);
        session.SetString(Prefix + typeof(TViewModel).FullName, JsonSerializer.Serialize(model, JsonOptions));
    }

    public static TViewModel? GetClass<TViewModel>(this ISession session) where TViewModel : class
    {
        var json = session.GetString(Prefix + typeof(TViewModel).FullName);
        return json is null ? null : JsonSerializer.Deserialize<TViewModel>(json, JsonOptions);
    }

    public static void ClearClass<TViewModel>(this ISession session) where TViewModel : class => session.Remove(Prefix + typeof(TViewModel).FullName);

    public static void ClearClasses(this ISession session)
    {
        foreach (var key in session.Keys.Where(key => key.StartsWith(Prefix, StringComparison.Ordinal)).ToArray()) session.Remove(key);
        session.Remove(UserKey);
        session.Remove(UserTypeKey);
    }

    // Compatibilidad con los snapshots de acceso de versiones anteriores.
    // La identidad de la petición procede exclusivamente de la cookie.
    public static void SetUser<TViewModel>(this ISession session, TViewModel model) where TViewModel : class
    {
        session.ClearUser();
        session.SetClass(model);
        session.SetString(UserKey, session.GetString(Prefix + typeof(TViewModel).FullName)!);
        session.SetString(UserTypeKey, typeof(TViewModel).FullName!);
    }

    public static TViewModel? GetUser<TViewModel>(this ISession session) where TViewModel : class =>
        session.GetString(UserTypeKey) == typeof(TViewModel).FullName ? session.GetClass<TViewModel>() : null;

    public static void ClearUser(this ISession session)
    {
        var type = session.GetString(UserTypeKey);
        if (type is not null) session.Remove(Prefix + type);
        session.Remove(UserKey);
        session.Remove(UserTypeKey);
    }
}
